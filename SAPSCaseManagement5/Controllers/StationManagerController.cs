using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;

namespace SAPSCaseManagement5.Controllers
{
    //[Authorize(Roles = "Station Manager")]
    public class StationManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StationManagerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllCases()
        {
            var allCases = await _context.CriminalRecords
                .Include(cr => cr.Offense)
                .Include(cr => cr.Suspect)
                .Include(cr => cr.CaseManager)
                .ToListAsync();

      

            return View(allCases);
        }
        //[Authorize(Roles = "Station Manager")]
        public async Task<IActionResult> CaseReport()
        {
            var report = _context.CriminalRecords
       .Include(cr => cr.CaseManager)
       .GroupBy(cr => cr.CaseManager.ManagerName)
       .Select(g => new
       {
           CaseManager = g.Key,
           TotalCases = g.Count()
       })
       .ToList();

            return View(report);
        }

        // GET: StationManager / Tasks
        public async Task<IActionResult> Tasks()
        {
            var caseManagers = await _context.CaseManagers
         .Include(cm => cm.CriminalRecords) // Include CriminalRecords
             .ThenInclude(cr => cr.Offense) // Include Offense
         .Include(cm => cm.CriminalRecords) // If you need Suspect as well
             .ThenInclude(cr => cr.Suspect) // Include Suspect
         .ToListAsync();

            return View(caseManagers);
        }

        // GET: StationManager/Terminate//

        public async Task<IActionResult> Terminate(int id)
        {
            var caseManager = await _context.CaseManagers.FindAsync(id);
            if (caseManager == null)
            {
                return NotFound();
            }

            _context.CaseManagers.Remove(caseManager);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tasks));
        }



    }
}


        



    