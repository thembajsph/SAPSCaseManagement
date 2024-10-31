using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.ViewModels;
using System.Linq;
using System.Threading.Tasks;

public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(); // This could be a landing page for statistics
    }

    public async Task<IActionResult> CrimeTypes()
    {
        // Fetch data related to crime types and their counts
        var crimeTypes = await _context.CriminalRecords
            .GroupBy(cr => cr.Offense.OffenseName) // Group by the offense name
            .Select(g => new CrimeTypeViewModel
            {
                OffenseName = g.Key, // Get the offense name as a string
                Count = g.Count()    // Count occurrences
            })
            .ToListAsync();

        return View(crimeTypes);
    }


    public async Task<IActionResult> CompletedCases()
    {
        // Fetch data related to completed cases
        var completedCases = await _context.CriminalRecords
            .Where(cr => cr.Status == "Completed") // Assuming a Status property exists
            .Include(cr => cr.Offense) // Include offense details
            .Include(cr => cr.Suspect) // Include suspect details
            .Include(cr => cr.CaseManager) // Include case manager details
            .Select(cr => new CompletedCaseViewModel // Using a view model for clarity
            {
                SuspectName = cr.Suspect.FirstName, // Extract the name from the Suspect object
                OffenseName = cr.Offense.OffenseName, // Get the offense name
                CaseManager = cr.CaseManager.ManagerName, // Extract the name from the CaseManager object
                Sentence = cr.Sentence,
                IssueDate = cr.IssueDate
            })
            .ToListAsync();

        return View(completedCases);
    }

    //public async Task<IActionResult> TaskStats()
    //{
    //    // Group tasks by year of completion and status, then count each group
    //    var taskStats = await _context.Tasks
    //        .GroupBy(t => new { Year = t.CompletionDate.Year, t.Status }) // Group by year and status
    //        .Select(g => new
    //        {
    //            Year = g.Key.Year,      // Year of task completion
    //            Status = g.Key.Status,  // Task status (e.g., Completed, In Progress)
    //            Count = g.Count()       // Number of tasks for each group
    //        })
    //        .ToListAsync();

    //    return View(taskStats); // Pass data to the view
    //}
}

