using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using KarnelTravelGuide.Services;
using KarnelTravelGuide.ViewModels;

namespace KarnelTravelGuide.Controllers;

public class AccountController : Controller
{
    readonly UserStore _users;
    readonly IConfiguration _cfg;
    readonly EmailService _email;
    readonly ILogger<AccountController> _log;

    public AccountController(
        UserStore users,
        IConfiguration cfg,
        EmailService email,
        ILogger<AccountController> log)
    {
        _users = users;
        _cfg = cfg;
        _email = email;
        _log = log;
    }

    // Only ever go back to a page on this site (prevents open-redirect attacks)
    string Safe(string returnUrl) => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

    // Sign Up is the first page a visitor sees when they try to book, check a booking or send feedback
    [HttpGet("Register")]
    public IActionResult Register(string returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true) return LocalRedirect(Safe(returnUrl) ?? "/");
        return View(new RegisterViewModel { ReturnUrl = Safe(returnUrl) });
    }

    [HttpPost("Register"), ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel vm)
    {
        vm.ReturnUrl = Safe(vm.ReturnUrl);
        if (!ModelState.IsValid) return View(vm);
        if (!_users.TryRegister(vm.Name, vm.Email, vm.Phone, vm.Password))
        {
            ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists. Please log in instead.");
            return View(vm);
        }
        // Step 1 done (Sign Up) -> step 2 (Login) -> then straight to the page they wanted
        return RedirectToAction(nameof(Login), new { returnUrl = vm.ReturnUrl, registered = true });
    }


    // ---------------- Forgot password (Gmail OTP) ----------------
    // 1) ForgotPassword: email -> 6-digit OTP mailed   2) VerifyOtp: enter OTP   3) ResetPassword: new password
    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword() => View();

    [HttpPost("ForgotPassword"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        email = email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
        {
            ViewBag.Error = "Please enter a valid registered email address.";
            return View();
        }

        var otp = _users.IssueResetOtp(email);   // null = no such account, or an OTP was sent less than a minute ago
        if (otp != null)
        {
            try
            {
                await _email.SendAsync(email, "Karnel Travel Guide - Your verification code",
                    $"Your Karnel Travel Guide password reset code is: {otp}\n\n" +
                    $"It expires in {UserStore.OtpMinutes} minutes and can be used only once.\n" +
                    "If you did not request this, you can ignore this email.");
            }
            catch (Exception ex)
            {
                // the real reason (e.g. missing App Password, Gmail rejected the login) goes to the server log only
                _log.LogError(ex, "Could not send the password-reset OTP email.");
                _users.CancelResetOtp(email);
                ViewBag.Error = "We could not send the email right now. Please try again in a few minutes.";
                return View();
            }
        }

        // Same response whether or not the account exists
        return RedirectToAction(nameof(VerifyOtp), new { email });
    }

    [HttpGet("VerifyOtp")]
    public IActionResult VerifyOtp(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return RedirectToAction(nameof(ForgotPassword));
        ViewBag.Email = email;
        return View();
    }

    [HttpPost("VerifyOtp"), ValidateAntiForgeryToken]
    public IActionResult VerifyOtp(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(email)) return RedirectToAction(nameof(ForgotPassword));
        ViewBag.Email = email;

        var result = _users.VerifyResetOtp(email, otp?.Trim(), out var resetToken);
        switch (result)
        {
            case UserStore.OtpResult.Ok:
                ViewBag.Token = resetToken;
                return View("ResetPassword");
            case UserStore.OtpResult.Expired:
                ViewBag.Error = "This code has expired. Please request a new one.";
                break;
            case UserStore.OtpResult.TooManyAttempts:
                ViewBag.Error = "Too many incorrect attempts. Please request a new code.";
                break;
            default:
                ViewBag.Error = "That code is not correct. Please check it and try again.";
                break;
        }
        return View();
    }

    // The password form is only ever reached from a successful VerifyOtp; opening this URL directly gets nothing.
    [HttpGet("ResetPassword")]
    public IActionResult ResetPassword() => RedirectToAction(nameof(ForgotPassword));

    [HttpPost("ResetPassword"), ValidateAntiForgeryToken]
    public IActionResult ResetPassword(string email, string token, string password, string confirmPassword)
    {
        if (!_users.IsResetTokenValid(email, token))
        {
            ViewBag.Error = "Your verification has expired. Please start again.";
            return View("ForgotPassword");
        }

        ViewBag.Email = email;
        ViewBag.Token = token;

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 100)
        {
            ViewBag.Error = "Password must be 6 to 100 characters.";
            return View();
        }
        if (password != confirmPassword)
        {
            ViewBag.Error = "The two passwords do not match.";
            return View();
        }

        if (!_users.CompletePasswordReset(email, token, password))
        {
            ViewBag.Error = "Your verification has expired. Please start again.";
            return View("ForgotPassword");
        }
        return RedirectToAction(nameof(Login), new { reset = true });
    }

    [HttpGet("Login")]
    public IActionResult Login(string returnUrl, bool registered = false)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(Safe(returnUrl) ?? "/");

        ViewBag.JustRegistered = registered;

        ViewBag.PasswordReset = Request.Query["reset"] == "true";

        return View(new LoginViewModel { ReturnUrl = Safe(returnUrl) });
    }

    [HttpPost("Login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        vm.ReturnUrl = Safe(vm.ReturnUrl);
        if (!ModelState.IsValid) return View(vm);

        string name = null, email = null; var admin = false;
        var ae = _cfg["Admin:Email"]; var ap = _cfg["Admin:Password"];
        if (!string.IsNullOrEmpty(ae) && !string.IsNullOrEmpty(ap)
            && string.Equals(vm.Email?.Trim(), ae, StringComparison.OrdinalIgnoreCase) && Same(vm.Password, ap))
        { name = "Admin"; email = ae; admin = true; }
        else
        {
            var u = _users.Validate(vm.Email, vm.Password);
            if (u != null) { name = u.Name; email = u.Email; }
        }
        if (email == null)
        {
            ModelState.AddModelError("", "Incorrect email or password.");
            vm.Password = null;
            return View(vm);
        }

        var id = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name), new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, admin ? "Admin" : "Customer")
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id),
            new AuthenticationProperties { IsPersistent = true });

        return LocalRedirect(admin ? "/Admin/Dashboard" : (vm.ReturnUrl ?? "/"));
    }

    [HttpPost("Logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    static bool Same(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(a ?? "")), SHA256.HashData(Encoding.UTF8.GetBytes(b)));
}
