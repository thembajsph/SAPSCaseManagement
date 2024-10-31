using Microsoft.AspNetCore.Mvc;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using System.Threading.Tasks;

public class OffencesController : Controller
{
    private readonly ApplicationDbContext _context;

    public OffencesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Display form to add a new offence
    [HttpGet]
    public IActionResult AddOffence()
    {
        return View();
    }

    // POST: Add new offence
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOffence(Offense offence)
    {
        if (ModelState.IsValid)
        {
            _context.Offense.Add(offence);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Offense added successfully!";
            return RedirectToAction("AddOffence"); // Or redirect to the desired page
        }

        return View(offence);
    }
}




