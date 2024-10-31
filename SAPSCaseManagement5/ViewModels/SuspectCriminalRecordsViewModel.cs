using SAPSCaseManagement5.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAPSCaseManagement5.ViewModels
{
    public class SuspectCriminalRecordsViewModel
    {
        [Required(ErrorMessage = "Suspect information is required.")]
        public Suspect Suspect { get; set; }

        [Required(ErrorMessage = "At least one criminal record is required.")]
        public List<CriminalRecord> CriminalRecords { get; set; } = new List<CriminalRecord>(); // Initialize to avoid null reference

        public string CurrentUserId { get; set; } // Add this property for the current user's ID
    }
}


