using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CriminalRecordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Suspect ID is required.")]
        public int SuspectId { get; set; }

        [Required(ErrorMessage = "The Offense field is required.")]
        [MaxLength(100, ErrorMessage = "Offense cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Offense can only contain letters and spaces.")]
        public string Offense { get; set; }

        [Required(ErrorMessage = "The Sentence field is required.")]
        [Range(0, 100, ErrorMessage = "Sentence must be between 0 and 100 years.")]
        public int Sentence { get; set; }

        [Required(ErrorMessage = "The Issue Date is required.")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "The Case Manager ID is required.")]
        public int CaseManagerId { get; set; }

        [Required(ErrorMessage = "The Status field is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Status can only contain letters and spaces.")]
        public string Status { get; set; }

        // Optionally, include dropdowns for case managers
        // public List<SelectListItem>? CaseManagers { get; set; }
    }
}
