using Microsoft.AspNetCore.Mvc;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;

namespace SAPSCaseManagement5.Controllers
{
    public class nyoni : Controller
    {
        private readonly ApplicationDbContext _context;

        public nyoni(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CriminalRecords/Create
        public IActionResult Create(int suspectId)
        {
            ViewBag.SuspectId = suspectId;
            return View();
        }

        // POST: CriminalRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Offense,Sentence,IssueDate,SuspectId")] CriminalRecord criminalRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(criminalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Suspects");
            }
            return View(criminalRecord);
        }
    }

}