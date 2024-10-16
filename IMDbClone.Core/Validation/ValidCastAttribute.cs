using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.Validation
{
    public class ValidCastAttribute : ValidationAttribute
    {
        public ValidCastAttribute() : base("Cast members must contain at least one member.") { }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<string> cast)
            {
                return new ValidationResult("Cast must be a list of strings.");
            }

            // If the list is empty, return an error message.
            if (!cast.Any())
            {
                return new ValidationResult(ErrorMessage);
            }

            if (cast.Any(item => string.IsNullOrWhiteSpace(item)))
            {
                return new ValidationResult("Cast members cannot be empty.");
            }

            return ValidationResult.Success ?? new ValidationResult("An unknown error occurred.");
        }
    }
}