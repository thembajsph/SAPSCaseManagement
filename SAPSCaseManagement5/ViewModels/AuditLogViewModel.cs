namespace SAPSCaseManagement5.ViewModels
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}
