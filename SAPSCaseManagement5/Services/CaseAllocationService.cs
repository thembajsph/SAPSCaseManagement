using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using Microsoft.EntityFrameworkCore;


namespace SAPSCaseManagement5.Services
{
    public class CaseAllocationService
    {
        private readonly ApplicationDbContext _context;

        public CaseAllocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AssignCaseToManager(CriminalRecord record)
        {
            var caseManager = await _context.CaseManagers
                .OrderBy(cm => cm.CriminalRecords.Count())
                .FirstOrDefaultAsync();

            if (caseManager != null)
            {
                record.CaseManagerId = caseManager.CaseManagerId;
                _context.CriminalRecords.Update(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
