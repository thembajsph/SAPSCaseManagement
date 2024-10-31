using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.Models
{
    public class CriminalRecord
    {
        [Required]
        public int CriminalRecordId { get; set; }

        
        [Required(ErrorMessage = "The Offense field is required.")]
        // Navigation Property for Offense
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Use the Offense navigation property

        //[Required(ErrorMessage = "The Offense1 field is required.")]
        ////public string Offense { get; set; }

        [Required(ErrorMessage = "Sentence is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sentence must be a positive number.")]
        public int Sentence { get; set; }

        [Required(ErrorMessage = "The Issue Date1 field is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "The Suspect1 field1 is required.")]
        public int SuspectId { get; set; }
        public Suspect Suspect { get; set; }

        public int CaseManagerId { get; set; }

        //[Required(ErrorMessage = "The Case Manager1 field is required.")]
        public CaseManager? CaseManager { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        //// Foreign key reference to the Suspect
        //public int SuspectId { get; set; }
        //public Suspect Suspect { get; set; } // Navigation property


    }
}
