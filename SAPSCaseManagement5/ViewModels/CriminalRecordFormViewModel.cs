using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CriminalRecordFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Suspect ID is required.")]
        public int SuspectId { get; set; }

        [Required(ErrorMessage = "The Sentence field is required.")]
        [Range(0, 100, ErrorMessage = "Sentence must be between 0 and 100 years.")]
        public int Sentence { get; set; }

        [Required(ErrorMessage = "The Issue Date is required.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "The Offense is required.")]
        public int OffenseId { get; set; } // Holds the selected offense ID

        [Required(ErrorMessage = "The Case Manager is required.")]
        public int CaseManagerId { get; set; }

        [Required(ErrorMessage = "The Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Status can only contain letters and spaces.")]
        public string Status { get; set; }

        // Only needed for forms where a dropdown is required
        public IEnumerable<SelectListItem>? CaseManagers { get; set; }

        // Dropdown list for offenses
        public List<SelectListItem>? OffenseList { get; set; }
    }
}

