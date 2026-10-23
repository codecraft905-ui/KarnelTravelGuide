using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using KarnelTravelGuide.Models;
using KarnelTravelGuide.Models.Data;

namespace KarnelTravelGuide.Services;

// Customer accounts, saved in the UserAccounts table (they now survive an app restart).
// Passwords are never stored: only a salted PBKDF2-SHA256 hash. Same methods as before, so AccountController is unchanged.
// Registered as SCOPED in Program.cs (it uses the DbContext).
public class UserStore
{
    const int Iter = 100_000;
    readonly ApplicationDbContext _db;
    public UserStore(ApplicationDbContext db) { _db = db; }

    static string Key(string email) => (email ?? "").Trim();

    public bool Exists(string email)
    {
        var k = Key(email);
        return _db.UserAccounts.AsNoTracking().Any(u => u.Email == k);
    }

    public bool TryRegister(string name, string email, string phone, string password)
    {
        var k = Key(email);
        if (_db.UserAccounts.AsNoTracking().Any(u => u.Email == k)) return false;

        var salt = RandomNumberGenerator.GetBytes(16);
        var u = new UserAccount
        {
            Id = Guid.NewGuid().ToString("N"),
            CreatedAtUtc = DateTime.UtcNow,
            Name = name.Trim(),
            Email = k,
            Phone = phone?.Trim(),
            Salt = Convert.ToBase64String(salt),
            Iterations = Iter,
            PasswordHash = Convert.ToBase64String(Hash(password, salt, Iter))
        };
        _db.UserAccounts.Add(u);
        try { _db.SaveChanges(); return true; }
        catch (DbUpdateException)
        {
            _db.Entry(u).State = EntityState.Detached;
            if (_db.UserAccounts.AsNoTracking().Any(x => x.Email == k)) return false;   // someone registered the same email at the same moment
            throw;                                                                      // a real database problem: do not hide it
        }
    }

    public UserAccount Validate(string email, string password)
    {
        var k = Key(email);
        var u = _db.UserAccounts.AsNoTracking().FirstOrDefault(x => x.Email == k);
        if (u == null) { Hash(password ?? "", new byte[16], Iter); return null; }   // same cost either way
        var actual = Hash(password ?? "", Convert.FromBase64String(u.Salt), u.Iterations);
        return CryptographicOperations.FixedTimeEquals(actual, Convert.FromBase64String(u.PasswordHash)) ? u : null;
    }

    static byte[] Hash(string password, byte[] salt, int iterations) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);


    public UserAccount FindByEmail(string email)
    {
        var k = Key(email);
        return _db.UserAccounts.FirstOrDefault(u => u.Email == k);
    }

    public bool UpdatePassword(string email, string newPassword)
    {
        var u = _db.UserAccounts.FirstOrDefault(x => x.Email == Key(email));

        if (u == null)
            return false;

        var salt = RandomNumberGenerator.GetBytes(16);

        u.Salt = Convert.ToBase64String(salt);
        u.Iterations = Iter;
        u.PasswordHash = Convert.ToBase64String(
            Hash(newPassword, salt, Iter));

        _db.SaveChanges();
        return true;
    }

    public void SaveChanges()
    {
        _db.SaveChanges();
    }

    // ---------------- Forgot-password OTP ----------------
    // Reuses PasswordResetTokenHash / PasswordResetTokenExpiryUtc:
    //   step 1 (OTP sent):     hash = "salt.PBKDF2(otp)", expiry = now + 5 min, Verified = false
    //   step 2 (OTP verified): hash = SHA256(random reset token), expiry = now + 10 min, Verified = true
    public const int OtpMinutes = 5;
    public const int ResetWindowMinutes = 10;
    public const int MaxOtpAttempts = 5;
    const int ResendCooldownSeconds = 60;
    const int OtpIterations = 10_000;

    public enum OtpResult { Ok, Invalid, Expired, TooManyAttempts }

    static string OtpHash(string otp, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(otp, salt, OtpIterations, HashAlgorithmName.SHA256, 32));

    // Creates a new 6-digit OTP for the account and stores ONLY its salted hash.
    // Returns null when the account does not exist or an OTP was issued less than 60 seconds ago.
    public string IssueResetOtp(string email)
    {
        var u = FindByEmail(email);
        if (u == null) return null;

        var now = DateTime.UtcNow;
        if (!u.PasswordResetVerified && u.PasswordResetTokenExpiryUtc != null &&
            u.PasswordResetTokenExpiryUtc.Value.AddMinutes(-OtpMinutes).AddSeconds(ResendCooldownSeconds) > now)
            return null;

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var salt = RandomNumberGenerator.GetBytes(16);
        u.PasswordResetTokenHash = Convert.ToBase64String(salt) + "." + OtpHash(otp, salt);
        u.PasswordResetTokenExpiryUtc = now.AddMinutes(OtpMinutes);
        u.PasswordResetAttempts = 0;
        u.PasswordResetVerified = false;
        _db.SaveChanges();
        return otp;
    }

    // Checks the OTP. On success the OTP is destroyed (single use) and a one-time reset token is returned.
    public OtpResult VerifyResetOtp(string email, string otp, out string resetToken)
    {
        resetToken = null;
        var u = FindByEmail(email);
        if (u == null || u.PasswordResetVerified || string.IsNullOrEmpty(u.PasswordResetTokenHash) || u.PasswordResetTokenExpiryUtc == null)
            return OtpResult.Invalid;

        if (u.PasswordResetTokenExpiryUtc <= DateTime.UtcNow) { ClearReset(u); return OtpResult.Expired; }
        if (u.PasswordResetAttempts >= MaxOtpAttempts) { ClearReset(u); return OtpResult.TooManyAttempts; }

        // count the attempt BEFORE comparing, so it cannot be skipped
        u.PasswordResetAttempts++;
        _db.SaveChanges();

        var parts = u.PasswordResetTokenHash.Split('.');
        bool ok = false;
        if (parts.Length == 2 && otp != null && otp.Length == 6 && otp.All(char.IsAsciiDigit))
        {
            var actual = Convert.FromBase64String(OtpHash(otp, Convert.FromBase64String(parts[0])));
            ok = CryptographicOperations.FixedTimeEquals(actual, Convert.FromBase64String(parts[1]));
        }

        if (!ok)
        {
            if (u.PasswordResetAttempts >= MaxOtpAttempts) { ClearReset(u); return OtpResult.TooManyAttempts; }
            return OtpResult.Invalid;
        }

        resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        u.PasswordResetTokenHash = Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(resetToken)));
        u.PasswordResetTokenExpiryUtc = DateTime.UtcNow.AddMinutes(ResetWindowMinutes);
        u.PasswordResetAttempts = 0;
        u.PasswordResetVerified = true;
        _db.SaveChanges();
        return OtpResult.Ok;
    }

    // Used when the email could not be sent, so no usable OTP is left behind.
    public void CancelResetOtp(string email)
    {
        var u = FindByEmail(email);
        if (u != null && !u.PasswordResetVerified) ClearReset(u);
    }

    // True only if this account has a verified, unexpired reset and the token matches.
    public bool IsResetTokenValid(string email, string token)
    {
        var u = FindByEmail(email);
        return u != null && ResetTokenMatches(u, token);
    }

    // Sets the new password (existing PBKDF2-SHA256 hashing) only after a verified OTP; token is single use.
    public bool CompletePasswordReset(string email, string token, string newPassword)
    {
        var u = FindByEmail(email);
        if (u == null || !ResetTokenMatches(u, token)) return false;

        var salt = RandomNumberGenerator.GetBytes(16);
        u.Salt = Convert.ToBase64String(salt);
        u.Iterations = Iter;
        u.PasswordHash = Convert.ToBase64String(Hash(newPassword, salt, Iter));
        ClearReset(u);   // saves
        return true;
    }

    static bool ResetTokenMatches(UserAccount u, string token)
    {
        if (!u.PasswordResetVerified || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(u.PasswordResetTokenHash) ||
            u.PasswordResetTokenExpiryUtc == null || u.PasswordResetTokenExpiryUtc <= DateTime.UtcNow)
            return false;
        var given = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        try { return CryptographicOperations.FixedTimeEquals(given, Convert.FromBase64String(u.PasswordResetTokenHash)); }
        catch (FormatException) { return false; }
    }

    void ClearReset(UserAccount u)
    {
        u.PasswordResetTokenHash = null;
        u.PasswordResetTokenExpiryUtc = null;
        u.PasswordResetAttempts = 0;
        u.PasswordResetVerified = false;
        _db.SaveChanges();
    }


}
