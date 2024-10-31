using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.Models
{
    public class Offense
    {
        public int OffenseId { get; set; }

        [Required(ErrorMessage = "The Offense Name field is required.")]
        [MaxLength(100, ErrorMessage = "Offense Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Offense Name can only contain letters and spaces.")]
        public string OffenseName { get; set; } // Name of the offense
    }
}



