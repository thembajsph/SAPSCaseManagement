using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data; // Assuming ApplicationDbContext is in the Data namespace
using SAPSCaseManagement5.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SAPSCaseManagement5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search(string idNumber)
        {
            if (string.IsNullOrEmpty(idNumber))
            {
                ViewData["ErrorMessage"] = "ID Number is required.";
                return View();
            }

            var suspect = _context.Suspects
                .Include(s => s.CriminalRecords)
                .ThenInclude(cr => cr.CaseManager)
                .Include(s => s.CriminalRecords)
        .ThenInclude(cr => cr.Offense) // Include the Offense navigation property
                .FirstOrDefault(s => s.IDNumber == idNumber);

    

            if (suspect == null)
            {
                ViewData["ErrorMessage"] = "No suspect found with the provided ID number.";
                return View();
            }

            return View(suspect);
        }


        // Original actions are retained for other views like Index and Privacy
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Action method to display the test navbar
        public IActionResult TestNavbar()
        {
            return View(); // This will look for Views/TestNavbar.cshtml
        }
    }
}

