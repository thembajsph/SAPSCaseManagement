using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class SuspectOffenseCountViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Offense count cannot be negative.")]
        public int OffenseCount { get; set; }
    }
}
