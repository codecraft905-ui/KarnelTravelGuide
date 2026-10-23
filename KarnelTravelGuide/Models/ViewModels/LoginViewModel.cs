using System.ComponentModel.DataAnnotations;

namespace KarnelTravelGuide.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your email address.")] public string Email { get; set; }
    [Required(ErrorMessage = "Please enter your password.")] public string Password { get; set; }
    public string ReturnUrl { get; set; }
}
