using System;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ValidationAttributes
{
    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var idNumber = value as string;

            // Check if the ID number is null or empty
            if (string.IsNullOrEmpty(idNumber))
            {
                return new ValidationResult("ID Number is required.");
            }

            // Check if the ID number is exactly 13 digits long
            if (idNumber.Length != 13)
            {
                return new ValidationResult("The South African ID number must be exactly 13 digits long.");
            }

            // Extract the first six digits for date of birth
            string dobPart = idNumber.Substring(0, 6);
            if (dobPart.Length != 6 || !int.TryParse(dobPart, out _))
            {
                return new ValidationResult("Date of Birth is invalid.");
            }

            // Extract year, month, day from the ID number
            int year = int.Parse(dobPart.Substring(0, 2));
            int month = int.Parse(dobPart.Substring(2, 2));
            int day = int.Parse(dobPart.Substring(4, 2));

            // Adjust the year based on the current year
            year += (year < 22) ? 2000 : 1900; // Example: Assuming '22' means 2022 and '99' means 1999

            // Validate the date
            DateTime dateOfBirth;
            if (!DateTime.TryParse($"{year}-{month}-{day}", out dateOfBirth) || dateOfBirth > DateTime.Today)
            {
                return new ValidationResult("Date of Birth is invalid or cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
