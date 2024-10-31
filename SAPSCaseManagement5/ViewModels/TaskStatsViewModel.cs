using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class TaskStatsViewModel
    {
        [Range(1900, int.MaxValue, ErrorMessage = "Year must be a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count cannot be negative.")]
        public int Count { get; set; }
    }
}

