using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.Models
{
    public class Suspect
    {
        public int SuspectId { get; set; }

        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(13, ErrorMessage = "The South African ID number must be exactly 13 digits long.", MinimumLength = 13)]
        [RegularExpression(@"^\d{6}\d{7}$", ErrorMessage = "Invalid South African ID format.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Last name can only contain letters and spaces.")]
        public string LastName { get; set; }

        // Navigation property to link Criminal Records
        public ICollection<CriminalRecord> CriminalRecords { get; set; }
    }
}
