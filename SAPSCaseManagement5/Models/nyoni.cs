using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.Models
{
    public class nyoni
    {
        public int CriminalRecordId { get; set; }
        [Required]
        public string Offense { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sentence must be a number.")]
        public int Sentence { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }
        public int SuspectId { get; set; }
        public Suspect Suspect { get; set; }
    }
}
