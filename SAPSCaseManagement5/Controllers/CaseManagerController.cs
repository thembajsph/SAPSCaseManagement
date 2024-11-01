using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using SAPSCaseManagement5.Services;
using SAPSCaseManagement5.ViewModels;
using System.Linq; // Ensure you have this namespace for ToList(
using System.Security.Claims;

namespace SAPSCaseManagement5.Controllers
{
    //[Authorize(Roles = "Case Manager")]
    public class CaseManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CaseManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //var userId = _userManager.GetUserId(User); // Get the currently logged-in user's ID

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return NotFound("User not found.");
            //}

            //// Fetch the case manager associated with the current user
            //var caseManager = await _context.CaseManagers.FirstOrDefaultAsync(cm => cm.UserId == userId);

            //if (caseManager == null)
            //{
            //    return NotFound("Case Manager not found for the logged-in user.");
            //}

            //// Pass the CaseManagerId to the view
            //ViewBag.CaseManagerId = caseManager.CaseManagerId;
            //caseManager

            return View(); // Pass the case manager to the view
        }


        [HttpGet]
        public async Task<IActionResult> MyCases()
        {
            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            // Find the case manager associated with the current user
            var caseManager = await _context.CaseManagers
                .FirstOrDefaultAsync(cm => cm.Email == currentUser.Email);

            if (caseManager == null)
            {
                return NotFound("Case manager not found for the logged-in user.");
            }

            // Fetch the criminal records assigned to this case manager
            var assignedCases = await _context.CriminalRecords
                .Where(cr => cr.CaseManagerId == caseManager.CaseManagerId) // Use the caseManager's Id
                .Include(cr => cr.Offense) // Include offense details
                .Include(cr => cr.Suspect) // Include related suspect data
                .ToListAsync();

            // Check if any assigned cases were found
            if (assignedCases == null || !assignedCases.Any())
            {
                // Return a message or view indicating no cases were found
                return View("NoCases"); // Create a "NoCases" view for better user experience
            }

            return View(assignedCases);
        }


        public async Task<IActionResult> Dashboard()
        {
            var suspects = await _context.Suspects
                .Select(s => new SuspectOffenseCountViewModel
                {
                    FullName = $"{s.FirstName} {s.LastName}",
                    OffenseCount = s.CriminalRecords.Count()
                })
                .ToListAsync();

            return View(suspects);
        }
        public async Task<IActionResult> LogOffense(CriminalRecord record)
        {
            var allocationService = new CaseAllocationService(_context);
            await allocationService.AssignCaseToManager(record);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CasesByManager()
        {
            // Get the current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            // Find the case manager associated with the current user
            var caseManager = await _context.CaseManagers
                .FirstOrDefaultAsync(cm => cm.Email == currentUser.Email); // Assuming Email is used to associate

            if (caseManager == null)
            {
                return NotFound("Case manager not found for the logged-in user.");
            }

            // Fetch the case manager with their associated criminal records and offenses
            var caseManagerWithRecords = await _context.CaseManagers
                .Include(cm => cm.CriminalRecords) // Eager load CriminalRecords
                    .ThenInclude(cr => cr.Offense) // Eager load Offense related to each CriminalRecord
                .FirstOrDefaultAsync(cm => cm.CaseManagerId == caseManager.CaseManagerId); // Use the found case manager's ID

            // Check if the case manager exists
            if (caseManagerWithRecords == null)
            {
                return NotFound($"No case manager found with ID {caseManager.CaseManagerId}.");
            }

            // Create view model
            var viewModel = new CaseManagerCasesViewModel
            {
                ManagerName = caseManagerWithRecords.ManagerName,
                CriminalRecords = caseManagerWithRecords.CriminalRecords.ToList() // Explicitly convert to List
            };

            // Debug: Confirm the manager's name and the number of criminal records retrieved
            Console.WriteLine($"Retrieved Case Manager: {viewModel.ManagerName} with {viewModel.CriminalRecords.Count} records.");

            // Return the view with the view model
            return View(viewModel);
        }


        public IActionResult Create()
        {
            // Fetch CaseManagers from the database
            var caseManagers = _context.CaseManagers
                .Select(cm => new SelectListItem
                {
                    Value = cm.CaseManagerId.ToString(), // Use CaseManagerId as the value
                    Text = cm.ManagerName // Display the manager's name
                })
                .ToList();

            // Pass the list to the view via ViewBag
            ViewBag.CaseManagers = new SelectList(caseManagers, "Value", "Text");

            return View();
        }

        // GET: Display form to add a new Case Manager
        [HttpGet]
        public IActionResult AddCaseManager()
        {
            return View();
        }

        // POST: Handle form submission and add the Case Manager
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCaseManager(CaseManagerFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var caseManager = new CaseManager
                {
                    ManagerName = viewModel.ManagerName,
                    Email = viewModel.Email,
                    CaseCount = viewModel.CaseCount,
                    UserId = "defaultUserId" // Assign a default UserId if necessary

                };

                _context.CaseManagers.Add(caseManager);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Case Manager added successfully!";
                return RedirectToAction("AddCaseManager");
            }

            return View(viewModel);
        }
    }

    }