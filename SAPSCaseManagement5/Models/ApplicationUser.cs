using Microsoft.AspNetCore.Identity;


namespace SAPSCaseManagement5.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ContactNumber { get; set; }

    public string? CaseManagerId { get; set; } // Ensure this property exists

    // Nullable
    //    public int? CaseManagerId { get; set; } // Assuming CaseManagerId is nullable
}
