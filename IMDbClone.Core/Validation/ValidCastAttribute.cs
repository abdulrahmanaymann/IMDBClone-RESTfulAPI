using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.Validation
{
    public class ValidCastAttribute : ValidationAttribute
    {
        public ValidCastAttribute()
            : base("Cast members must contain at least one member.")
        { }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // Check if the value is a List<string>
            if (value is not List<string> cast)
            {
                return new ValidationResult("Cast must be a list of strings.");
            }

            // Check if the list is empty
            if (cast.Count == 0)
            {
                return new ValidationResult("Cast members must contain at least one member.");
            }

            // Check for empty or whitespace strings in the list
            if (cast.Any(item => string.IsNullOrWhiteSpace(item)))
            {
                return new ValidationResult("Cast members cannot be empty.");
            }

            return ValidationResult.Success ?? new ValidationResult("An error occurred while validating the cast members.");
        }
    }
}