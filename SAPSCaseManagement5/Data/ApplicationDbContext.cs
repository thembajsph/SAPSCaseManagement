using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Models;



namespace SAPSCaseManagement5.Data
{
    // Inherit from IdentityDbContext to include IdentityUser, IdentityRole, and related tables
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor to pass DbContextOptions to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<Suspect> Suspects { get; set; }
        public DbSet<CriminalRecord> CriminalRecords { get; set; }
        public DbSet<CaseManager> CaseManagers { get; set; }

        // Add the following line if it doesn't exist
        public DbSet<Offense> Offense { get; set; } // Define the DbSet for Offens

        public DbSet<AuditLog> AuditLogs { get; set; }

        // Configure the model (optional but good practice for complex relationships and constraints)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // Custom configurations can be added here
            // For example:
            // modelBuilder.Entity<Suspect>().HasKey(s => s.SuspectId);
            // modelBuilder.Entity<CriminalRecord>().HasOne(cr => cr.Suspect).WithMany(s => s.CriminalRecords);
        }
    }
}

