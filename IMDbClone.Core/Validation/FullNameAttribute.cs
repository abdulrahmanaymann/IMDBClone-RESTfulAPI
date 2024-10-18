using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IMDbClone.Core.Validation
{
    public class FullNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var fullName = value as string;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                return new ValidationResult("Full name is required.");
            }

            // Check for special characters using a regex
            if (Regex.IsMatch(fullName, @"[^\w\s]"))
            {
                return new ValidationResult("Full name cannot contain special characters.");
            }

            // Check for other conditions (e.g., minimum length)
            if (fullName.Split(' ').Length < 2)
            {
                return new ValidationResult("Full name must contain at least a first name and a last name.");
            }

            return ValidationResult.Success;
        }
    }
}
