using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class UserRolesViewModel
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "At least one role must be assigned.")]
        public IList<string> Roles { get; set; } = new List<string>(); // Ensure it's initialized
    }
}

