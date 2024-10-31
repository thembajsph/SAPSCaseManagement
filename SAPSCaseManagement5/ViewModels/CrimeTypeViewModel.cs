using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CrimeTypeViewModel
    {
        [Required(ErrorMessage = "The Offense Name is required.")]
        [MaxLength(100, ErrorMessage = "Offense Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Offense Name can only contain letters and spaces.")]
        public string OffenseName { get; set; }

        [Required(ErrorMessage = "The Count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Count must be a non-negative number.")]
        public int Count { get; set; }
    }
}
