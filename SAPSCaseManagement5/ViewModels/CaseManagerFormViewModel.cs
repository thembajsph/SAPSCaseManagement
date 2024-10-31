using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CaseManagerFormViewModel
    {
        public int CaseManagerId { get; set; }

        [Required(ErrorMessage = "The Manager Name is required.")]
        [MaxLength(100, ErrorMessage = "Manager Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Manager Name can only contain letters and spaces.")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "The Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Case Count must be a non-negative number.")]
        public int CaseCount { get; set; } = 0;  // Default to 0 cases

        [Required(ErrorMessage = "The Offense is required.")]
        public int OffenseId { get; set; }

        // Consider adding a list of offenses if you are selecting from a dropdown
        public IEnumerable<SelectListItem> Offenses { get; set; }
    }
}

