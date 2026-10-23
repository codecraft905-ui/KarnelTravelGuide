using System.ComponentModel.DataAnnotations;

namespace KarnelTravelGuide.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your full name."), StringLength(100)] public string Name { get; set; }
    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address."), StringLength(150)] public string Email { get; set; }
    [Phone(ErrorMessage = "Please enter a valid phone number."), StringLength(30)] public string Phone { get; set; }
    [Required(ErrorMessage = "Please choose a password."), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")] public string Password { get; set; }
    [Required(ErrorMessage = "Please confirm your password."), Compare(nameof(Password), ErrorMessage = "The two passwords do not match.")] public string ConfirmPassword { get; set; }
    public string ReturnUrl { get; set; }
}
