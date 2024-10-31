using System;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CompletedCaseViewModel
    {
        [Required(ErrorMessage = "The Suspect Name is required.")]
        [MaxLength(100, ErrorMessage = "Suspect Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Suspect Name can only contain letters and spaces.")]
        public string SuspectName { get; set; }

        [Required(ErrorMessage = "The Offense Name is required.")]
        [MaxLength(100, ErrorMessage = "Offense Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Offense Name can only contain letters and spaces.")]
        public string OffenseName { get; set; }

        [Required(ErrorMessage = "The Case Manager's Name is required.")]
        [MaxLength(100, ErrorMessage = "Case Manager's Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Case Manager's Name can only contain letters and spaces.")]
        public string CaseManager { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Sentence must be a non-negative number.")]
        public int Sentence { get; set; } // Assuming Sentence is a non-negative integer

        [Required(ErrorMessage = "The Issue Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "The Issue Date must be a valid date.")]
        public DateTime IssueDate { get; set; }
    }
}

