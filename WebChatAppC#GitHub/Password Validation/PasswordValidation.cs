using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

// Dokumentacja do walidacji https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute?view=net-8.0

namespace WebChatAppC_GitHub.Password_Validation
{
    //Tworzymy walidacje korzystajac z ValidationAttribute
    public class PasswordValidation : ValidationAttribute
    {
        //Nadpisujemy zeby storzyc wlasna walidacje
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Password is required.");
            }

            var password = value.ToString();

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new ValidationResult("Password must contain at least one number.");
            }

            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }
    }
}
