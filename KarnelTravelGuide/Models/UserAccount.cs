namespace KarnelTravelGuide.Models;

// A customer account (needed to book, track bookings and send messages). The admin/owner uses appsettings.json.
public class UserAccount
{
    public string Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Salt { get; set; }            // base64
    public string PasswordHash { get; set; }    // base64 PBKDF2-SHA256, never the password itself
    public int Iterations { get; set; }

    public string PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiryUtc { get; set; }
    public int PasswordResetAttempts { get; set; }          // wrong OTP guesses for the current OTP
    public bool PasswordResetVerified { get; set; }         // true only after the OTP was entered correctly


}
