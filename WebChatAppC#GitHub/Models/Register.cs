using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebChatAppC_GitHub.Password_Validation;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [PasswordValidation()]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; }
}
