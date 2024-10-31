namespace SAPSCaseManagement5.Models
{
    public class CaseManager
    {
        public int CaseManagerId { get; set; }
        public string ManagerName { get; set; }
        public int CaseCount { get; set; } // Tracks number of cases assigned to this manager

        // Add the UserId property
        public string UserId { get; set; }

        public string Email { get; set; }

        public ICollection<CriminalRecord> CriminalRecords { get; set; }
    }
}
