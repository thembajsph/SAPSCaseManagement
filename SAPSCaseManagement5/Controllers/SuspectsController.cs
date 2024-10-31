using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using SAPSCaseManagement5.ViewModels;


namespace SAPSCaseManagement5.Controllers
{
    public class SuspectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuspectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Suspect suspect)
        {
            var idNumber = suspect.IDNumber;

            // Check if the ID number is null or empty
            if (string.IsNullOrEmpty(idNumber))
            {
                ModelState.AddModelError(nameof(suspect.IDNumber), "ID Number is required.");
            }
            // Check if the ID number is exactly 13 digits long
            else if (idNumber.Length != 13)
            {
                ModelState.AddModelError(nameof(suspect.IDNumber), "The South African ID number must be exactly 13 digits long.");
            }
            // Check if the ID number is all digits
            else if (!idNumber.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(suspect.IDNumber), "ID Number must contain only digits.");
            }
            else
            {
                // Extract the first six digits for date of birth
                string dobPart = idNumber.Substring(0, 6);

                // Extract year, month, day from the ID number
                int year = int.Parse(dobPart.Substring(0, 2));
                int month = int.Parse(dobPart.Substring(2, 2));
                int day = int.Parse(dobPart.Substring(4, 2));

                // Adjust the year based on the current year
                year += (year < 22) ? 2000 : 1900; // Assuming '22' means 2022 and '99' means 1999

                // Validate the date
                DateTime dateOfBirth;
                if (!DateTime.TryParse($"{year}-{month}-{day}", out dateOfBirth) || dateOfBirth > DateTime.Today)
                {
                    ModelState.AddModelError(nameof(suspect.IDNumber), "Date of Birth is invalid or cannot be in the future.");
                }
            }

            // Check if the suspect's ID number already exists in the database
            var existingSuspect = await _context.Suspects
                .FirstOrDefaultAsync(s => s.IDNumber == suspect.IDNumber);

            if (existingSuspect != null)
            {
                ModelState.AddModelError(nameof(suspect.IDNumber), "A suspect with this ID number already exists.");
            }

            // Log validation errors for debugging
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Error in {modelState.Key}: {error.ErrorMessage}");
                    }
                }
                // If validation fails, redisplay the form
                return View(suspect);
            }

            // If everything is valid, add the suspect
            _context.Add(suspect);
            await _context.SaveChangesAsync();
            await LogAudit("Created Suspect", $"Suspect {suspect.FirstName} {suspect.LastName} added.");
            TempData["SuccessMessage"] = "Suspect added successfully!";
            return RedirectToAction("ListSuspects", "Suspects", new { suspectId = suspect.SuspectId });
        }


        private async Task LogAudit(string action, string details)
        {
            var user = User.Identity.Name;
            var auditLog = new AuditLog
            {
                Action = action,
                User = user,
                Timestamp = DateTime.UtcNow,
                Details = details
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        // GET: List of Suspects
        public IActionResult ListSuspects()
        {
            // Fetch all suspects from the database
            var suspects = _context.Suspects.ToList();

            // Pass the list of suspects to the View via ViewBag
            ViewBag.Suspects = suspects;

            // Return the view that will display the list
            
            return View();
        }

        public async Task<IActionResult> SuspectDetails(int suspectId)
        {
            var suspect = await _context.Suspects.FirstOrDefaultAsync(s => s.SuspectId == suspectId);
            if (suspect != null)
            {
                suspect.CriminalRecords = await _context.CriminalRecords
                    .Where(cr => cr.SuspectId == suspectId)
                    .Include(cr => cr.Offense)
                    .Include(cr => cr.CaseManager)
                    .ToListAsync();
            }

            if (suspect == null)
            {
                TempData["ErrorMessage"] = "Suspect not found.";
                return RedirectToAction("Index");
            }

            var viewModel = new SuspectCriminalRecordsViewModel
            {
                Suspect = suspect,
                CriminalRecords = suspect.CriminalRecords.ToList()
            };

            return View(viewModel);
        }



    }
}