using SAPSCaseManagement5.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class CaseManagerCasesViewModel
    {
        [Required(ErrorMessage = "The Manager Name is required.")]
        [MaxLength(100, ErrorMessage = "Manager Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Manager Name can only contain letters and spaces.")]
        public string ManagerName { get; set; }

        public List<CriminalRecord> CriminalRecords { get; set; } = new List<CriminalRecord>();
    }
}

