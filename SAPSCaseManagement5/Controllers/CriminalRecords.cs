using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using SAPSCaseManagement5.ViewModels;
using SAPSCaseManagement5.ViewModels; // This should be present


namespace SAPSCaseManagement5.Controllers
{
    //[Authorize(Roles = "Police Officer")]
    public class CriminalRecordsController : Controller
    {
      
        private readonly UserManager<ApplicationUser> _userManager; // Replace with your actual user class
       
        private readonly ILogger<CriminalRecordsController> _logger;
        private readonly ApplicationDbContext _context;


        public CriminalRecordsController(ApplicationDbContext context, ILogger<CriminalRecordsController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int suspectId)
        {
            // Set the suspectId in the view data and view bag
            ViewData["SuspectId"] = suspectId;

            // Get the CaseManagers and pass them as a SelectList to the view
            var caseManagers = _context.CaseManagers.ToList();
            ViewBag.CaseManagers = new SelectList(caseManagers, "CaseManagerId", "ManagerName");

            // Pass the SuspectId as a parameter to the CriminalRecord model
            return View(new CriminalRecord { SuspectId = suspectId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CriminalRecord criminalRecord)
        {
            // Ensure SuspectId is provided in the model
            if (criminalRecord.SuspectId == 0)
            {
                ModelState.AddModelError("SuspectId", "The Suspect2 is required.");
            }

            // Check if the CaseManagerId has been selected; it is required
            if (criminalRecord.CaseManagerId == 0)
            {
                ModelState.AddModelError("CaseManagerId", "The Case Manager2 is required.");
            }

            // If the model state is valid, assign the Case Manager and save the record
            if (ModelState.IsValid)
            {
                // Auto-allocate the manager with the least cases
                var manager = _context.CaseManagers.OrderBy(m => m.CaseCount).FirstOrDefault();
                if (manager != null)
                {
                    // Assign the CaseManagerId to the record
                    criminalRecord.CaseManagerId = manager.CaseManagerId;

                    // Increment the CaseCount for the selected manager
                    manager.CaseCount++;
                }
                else
                {
                    ModelState.AddModelError("", "No available Case Manager found.");
                }

                // If model state is still valid, save the criminal record to the database
                if (ModelState.IsValid)
                {
                    _context.Add(criminalRecord);
                    await _context.SaveChangesAsync();

                    // Provide success message and redirect to Search page
                    TempData["SuccessMessage"] = "Criminal record added successfully!";
                    return RedirectToAction("Search", "Home");
                }
            }

            // Repopulate Case Managers dropdown if validation fails
            ViewBag.CaseManagers = new SelectList(_context.CaseManagers, "CaseManagerId", "ManagerName");

            // Return to the view with the model data and validation errors
            return View(criminalRecord);
        }

        // GET: Edit a Criminal Record for a specific suspect
        // GET: Edit a Criminal Record for a specific suspect
        // GET: Edit a Criminal Record for a specific suspect
        //[HttpGet]
        //public IActionResult Edit(int suspectId)
        //{
        //    var criminalRecord = _context.CriminalRecords
        //                                .Include(cr => cr.Suspect)
        //                                .Include(cr => cr.CaseManager)
        //                                .FirstOrDefault(cr => cr.SuspectId == suspectId);

        //    if (criminalRecord == null)
        //    {
        //        return NotFound();
        //    }

        //    // Populate the CaseManagers for the dropdown in the view
        //    var viewModel = new CriminalRecordFormViewModel
        //    {
        //        Id = criminalRecord.CriminalRecordId,
        //        SuspectId = criminalRecord.SuspectId,
        //        Offense = criminalRecord.Offense,
        //        Sentence = criminalRecord.Sentence,
        //        IssueDate = criminalRecord.IssueDate,
        //        CaseManagerId = criminalRecord.CaseManagerId,
        //        Status = criminalRecord.Status,
        //        CaseManagers = _context.CaseManagers
        //                               .Select(cm => new SelectListItem
        //                               {
        //                                   Value = cm.CaseManagerId.ToString(),
        //                                   Text = cm.ManagerName
        //                               }).ToList()
        //    };

        //    // Set ViewBag.CaseManagers so that it's accessible in the view
        //    ViewBag.CaseManagers = viewModel.CaseManagers;

        //    return View(viewModel);

        // Method to return a static list of offenses
        private List<SelectListItem> GetStaticOffences()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Assault" },
        new SelectListItem { Value = "2", Text = "Burglary" },
        new SelectListItem { Value = "3", Text = "Drug" },
        new SelectListItem { Value = "4", Text = "Hijacking" },
        new SelectListItem { Value = "5", Text = "Murder" },
        new SelectListItem { Value = "6", Text = "Offences of Dishonesty" },
        new SelectListItem { Value = "7", Text = "Other offences" },
        new SelectListItem { Value = "8", Text = "Public Drinking" },
        new SelectListItem { Value = "9", Text = "Rape" },
        new SelectListItem { Value = "10", Text = "Robbery" },
        new SelectListItem { Value = "11", Text = "Sexual Harassment" },
        new SelectListItem { Value = "12", Text = "Sexual Offences" },
        new SelectListItem { Value = "13", Text = "Violence" }
    };
        }

        private async Task<List<SelectListItem>> GetOffencesAsync()
        {
            return await _context.Offense  // Assuming Offense is the correct DbSet name
                                 .Select(o => new SelectListItem
                                 {
                                     Value = o.OffenseId.ToString(), // Use the OffenseId as the value
                                     Text = o.OffenseName // Use the OffenseName as the display text
                                 })
                                 .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int suspectId)
        {
            // Fetch the criminal record for the given suspectId
            var criminalRecord = await _context.CriminalRecords
                                               .Include(cr => cr.Suspect)
                                               .Include(cr => cr.CaseManager)
                                               .FirstOrDefaultAsync(cr => cr.SuspectId == suspectId);

            // If no criminal record exists, create a new instance
            if (criminalRecord == null)
            {
                criminalRecord = new CriminalRecord
                {
                    SuspectId = suspectId,
                    Status = "Pending" // Default status
                };
            }

            // Correctly await the asynchronous method
            var offenseList = await GetOffencesAsync(); // Await the async method

            // Fetch the Case Manager with the least cases
            var leastBusyManager = await _context.CaseManagers
                                                 .OrderBy(cm => cm.CaseCount)
                                                 .FirstOrDefaultAsync();

            // If no case manager found, handle the error
            if (leastBusyManager == null)
            {
                ModelState.AddModelError("CaseManagerId", "No available Case Manager found.");
                var viewModel = new CriminalRecordFormViewModel
                {
                    SuspectId = suspectId,
                    OffenseList = offenseList, // Use awaited result here
                    CaseManagers = new List<SelectListItem>(), // Empty list for case managers
                    //StatusList = new List<SelectListItem>() // Initialize StatusList here if needed
                };
                return View(viewModel);
            }

            // Fetch the list of available Case Managers
            var caseManagers = await _context.CaseManagers
                                             .Select(cm => new SelectListItem
                                             {
                                                 Value = cm.CaseManagerId.ToString(),
                                                 Text = cm.ManagerName,
                                                 Selected = cm.CaseManagerId == leastBusyManager.CaseManagerId
                                             }).ToListAsync();

            //        // Populate the status list
            //        var statusList = new List<SelectListItem>
            //{
            //    new SelectListItem { Value = "Pending", Text = "Pending" },
            //    new SelectListItem { Value = "Completed", Text = "Completed" },
            //    new SelectListItem { Value = "In Progress", Text = "In Progress" },
            //    new SelectListItem { Value = "Closed", Text = "Closed" }
            //};

            // Log the number of Case Managers fetched
            _logger.LogInformation("Case Managers Count: " + caseManagers.Count);

            // Create the view model and assign properties
            var editViewModel = new CriminalRecordFormViewModel
            {
                Id = criminalRecord.CriminalRecordId,  // 0 if it's a new record
                SuspectId = criminalRecord.SuspectId,
                OffenseId = criminalRecord.OffenseId,  // Set the offense ID
                Sentence = criminalRecord.Sentence,
                IssueDate = criminalRecord.IssueDate,
                CaseManagerId = leastBusyManager.CaseManagerId,  // Assign least busy manager
                Status = criminalRecord.Status,
                OffenseList = offenseList,  // Populate the dropdown list for offenses
                CaseManagers = caseManagers, // Populate the Case Manager dropdown list
                                             //    StatusList = statusList // Assign the status list to the view model
            };

            // Return the view with the populated ViewModel
            return View(editViewModel);
        }










        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, CriminalRecordFormViewModel updatedRecord)
        //{
        //    // Check if the ID matches
        //    if (id != updatedRecord.Id)
        //    {
        //        return NotFound();
        //    }

        //    // Ensure that the CaseManagerId is selected
        //    if (updatedRecord.CaseManagerId == 0)
        //    {
        //        ModelState.AddModelError("CaseManagerId", "The Case Manager2222 is required.");
        //    }

        //    // If the model state is valid, update the criminal record
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var criminalRecord = _context.CriminalRecords.FirstOrDefault(cr => cr.CriminalRecordId == id);
        //            if (criminalRecord == null)
        //            {
        //                return NotFound();
        //            }

        //            criminalRecord.Offense = updatedRecord.Offense;
        //            criminalRecord.Sentence = updatedRecord.Sentence;
        //            criminalRecord.IssueDate = updatedRecord.IssueDate;
        //            criminalRecord.CaseManagerId = updatedRecord.CaseManagerId;
        //            criminalRecord.Status = updatedRecord.Status;

        //            _context.Update(criminalRecord);
        //            await _context.SaveChangesAsync();

        //            TempData["SuccessMessage"] = "Criminal record updated successfully!";
        //            //return RedirectToAction("ListSuspects", "Suspects");

        //            // Redirect to the same URL with the query string (GET action)
        //            return RedirectToAction("Edit", new { suspectId = updatedRecord.SuspectId });
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!_context.CriminalRecords.Any(cr => cr.CriminalRecordId == id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }

        //    // Repopulate Case Managers dropdown if validation fails
        //    updatedRecord.CaseManagers = _context.CaseManagers
        //                                         .Select(cm => new SelectListItem
        //                                         {
        //                                             Value = cm.CaseManagerId.ToString(),
        //                                             Text = cm.ManagerName
        //                                         }).ToList();

        //    // Return the full CriminalRecordViewModel, not an anonymous object
        //    return View(updatedRecord);  // Pass the full model back to the view
        //}
        private async Task<List<SelectListItem>> GetCaseManagersAsync()
        {
            return await _context.CaseManagers
                                 .Select(cm => new SelectListItem
                                 {
                                     Value = cm.CaseManagerId.ToString(),
                                     Text = $"{cm.ManagerName} ({cm.CaseCount} cases)" // Include current CaseCount
                                 })
                                 .ToListAsync();
        }


        private async Task<List<SelectListItem>> GetOffenseListAsync()
        {
            return await _context.Offense
                                 .Select(o => new SelectListItem
                                 {
                                     Value = o.OffenseId.ToString(),
                                     Text = o.OffenseName
                                 })
                                 .ToListAsync();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CriminalRecordFormViewModel updatedRecord)
        {
            // Check if the ID matches
            if (id != updatedRecord.Id)
            {
                return NotFound();
            }

            // Validate the updated record
            if (updatedRecord == null || !ModelState.IsValid)
            {
                updatedRecord.OffenseList = await GetOffenseListAsync();
                updatedRecord.CaseManagers = await GetCaseManagersAsync();
                return View(updatedRecord);
            }

            // Validate the suspect exists
            var suspectExists = await _context.Suspects.AnyAsync(s => s.SuspectId == updatedRecord.SuspectId);
            if (!suspectExists)
            {
                ModelState.AddModelError("SuspectId", "The selected suspect does not exist.");
                updatedRecord.OffenseList = await GetOffenseListAsync();
                updatedRecord.CaseManagers = await GetCaseManagersAsync();
                return View(updatedRecord);
            }

            // Validate the offense selection
            var offense = await _context.Offense.FirstOrDefaultAsync(o => o.OffenseId == updatedRecord.OffenseId);
            if (offense == null)
            {
                ModelState.AddModelError("Offense", "The selected offense is invalid.");
                updatedRecord.OffenseList = await GetOffenseListAsync();
                updatedRecord.CaseManagers = await GetCaseManagersAsync();
                return View(updatedRecord);
            }

            // Automatically assign the Case Manager if no selection was made
            if (updatedRecord.CaseManagerId <= 0)
            {
                var caseManager = await _context.CaseManagers
                                                .OrderBy(cm => cm.CaseCount)
                                                .FirstOrDefaultAsync();
                if (caseManager == null)
                {
                    ModelState.AddModelError("CaseManagerId", "No available Case Manager found.");
                    updatedRecord.OffenseList = await GetOffenseListAsync();
                    updatedRecord.CaseManagers = await GetCaseManagersAsync();
                    return View(updatedRecord);
                }

                updatedRecord.CaseManagerId = caseManager.CaseManagerId;
                caseManager.CaseCount++;
                _context.Update(caseManager);
            }
            else // Handle when a valid Case Manager is selected
            {
                var existingRecord = await _context.CriminalRecords
                                                   .AsNoTracking()
                                                   .FirstOrDefaultAsync(cr => cr.CriminalRecordId == updatedRecord.Id);

                if (existingRecord != null && existingRecord.CaseManagerId != updatedRecord.CaseManagerId)
                {
                    var previousManager = await _context.CaseManagers
                                                        .FirstOrDefaultAsync(cm => cm.CaseManagerId == existingRecord.CaseManagerId);
                    if (previousManager != null)
                    {
                        previousManager.CaseCount--;
                        _context.Update(previousManager);
                    }
                }
            }

            // Check if the ModelState is valid before updating
            if (ModelState.IsValid)
            {
                try
                {
                    var criminalRecord = new CriminalRecord
                    {
                        CriminalRecordId = updatedRecord.Id,
                        SuspectId = updatedRecord.SuspectId,
                        OffenseId = updatedRecord.OffenseId,
                        Sentence = updatedRecord.Sentence,
                        IssueDate = updatedRecord.IssueDate,
                        CaseManagerId = updatedRecord.CaseManagerId,
                        Status = updatedRecord.Status,
                    };

                    _context.Update(criminalRecord);
                    await _context.SaveChangesAsync();

                    // Update CaseManager counts
                    await UpdateCaseManagersCountAsync();

                    TempData["SuccessMessage"] = "Criminal record updated successfully!";
                    return RedirectToAction("Search", "Home");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CriminalRecordExists(updatedRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Handle concurrency exceptions
                    }
                }
            }

            // Repopulate the dropdowns if validation fails
            updatedRecord.OffenseList = await GetOffenseListAsync(); // Ensure the offense dropdown is populated
            updatedRecord.CaseManagers = await GetCaseManagersAsync(); // Ensure the CaseManager dropdown is populated




            return View(updatedRecord); // Return to the form if validation failed
        }


        // Helper method to update CaseManager counts based on CriminalRecords
        private async Task UpdateCaseManagersCountAsync()
        {
            var caseManagers = await _context.CaseManagers.ToListAsync();

            foreach (var manager in caseManagers)
            {
                // Count how many records are assigned to this case manager
                manager.CaseCount = await _context.CriminalRecords.CountAsync(cr => cr.CaseManagerId == manager.CaseManagerId);
            }

            _context.UpdateRange(caseManagers); // Update all case managers
            await _context.SaveChangesAsync(); // Commit the updates
        }




        // Check if a CriminalRecord exists by its ID
        private bool CriminalRecordExists(int id)
        {
            return _context.CriminalRecords.Any(cr => cr.CriminalRecordId == id);
        }




        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CriminalRecordFormViewModel updatedRecord)
        {
            if (id != updatedRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var criminalRecord = _context.CriminalRecords.FirstOrDefault(cr => cr.CriminalRecordId == id);
                if (criminalRecord == null)
                {
                    return NotFound();
                }

                // Update the fields of the existing record
                criminalRecord.OffenseId = updatedRecord.OffenseId; // Use the foreign key ID
                criminalRecord.Sentence = updatedRecord.Sentence;
                criminalRecord.IssueDate = updatedRecord.IssueDate;
                criminalRecord.CaseManagerId = updatedRecord.CaseManagerId;
                criminalRecord.Status = updatedRecord.Status;
                //CriminalRecordId = updatedRecord.Id,
                //SuspectId = updatedRecord.SuspectId,






                _context.Update(criminalRecord);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Criminal record updated successfully!";
                return RedirectToAction("Search", "Home");
            }

            return View(updatedRecord);
        }

        // Controller: CriminalRecordsController
        [HttpGet]
        public IActionResult GetCaseManagerName(int caseManagerId)
        {
            // Fetch the CaseManager by its ID
            var caseManager = _context.CaseManagers
                                     .FirstOrDefault(cm => cm.CaseManagerId == caseManagerId);

            if (caseManager == null)
            {
                return Json(new { success = false, message = "Case Manager not found." });
            }

            // Return the CaseManager name as a JSON object
            return Json(new { success = true, name = caseManager.ManagerName });
        }

        // Controller: CriminalRecordsController

        [HttpGet]
        public async Task<IActionResult> Revise(int id)
        {
            _logger.LogInformation("Received ID: {Id}", id);

            if (_context == null)
            {
                _logger.LogError("Database context is null.");
                return StatusCode(500, "Internal server error: Database context is not initialized.");
            }

            try
            {
                var record = await _context.CriminalRecords
                    .Include(r => r.Offense)
                    .Include(r => r.Suspect)
                    .Include(r => r.CaseManager) // Include CaseManager for retrieving manager details
                    .FirstOrDefaultAsync(r => r.CriminalRecordId == id);

                if (record == null)
                {
                    _logger.LogWarning("Record not found with ID: {Id}", id);
                    return NotFound();
                }

                // Fetch all CaseManagers for the dropdown (if needed)
                var caseManagers = await _context.CaseManagers.ToListAsync();
                if (caseManagers == null || !caseManagers.Any())
                {
                    _logger.LogWarning("No CaseManagers found.");
                    ViewBag.CaseManagers = new List<CaseManager>(); // Ensure ViewBag is not null
                }
                else
                {
                    ViewBag.CaseManagers = caseManagers;
                }

                // Get the current logged-in user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user is not found.");
                    return NotFound("User not found.");
                }

                // Assuming currentUser has a CaseManagerId property
                var currentCaseManagerId = currentUser.CaseManagerId;

                // Log relevant details
                _logger.LogInformation("Found record: {Record}", record);
                _logger.LogInformation("Record Details - ID: {Id}, Status: {Status}, Sentence: {Sentence}, CaseManager: {CaseManager}, Offense: {Offense}, Suspect: {Suspect}",
                    record.CriminalRecordId, record.Status, record.Sentence, record.CaseManager, record.Offense, record.Suspect);
                _logger.LogInformation("Current logged-in CaseManager ID: {CaseManagerId}", currentCaseManagerId);

                // Pass the current CaseManagerId and name to the view via ViewBag
                ViewBag.CurrentCaseManagerId = currentCaseManagerId;
                ViewBag.CurrentCaseManagerName = record.CaseManager?.ManagerName;

                return View(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the record with ID: {Id}", id);
                return StatusCode(500, "Internal server error: An error occurred while processing your request.");
            }
        }



        [HttpPost]
        public async Task<IActionResult> Revise(CriminalRecord model)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Log all model state errors
            //    var errorMessages = ModelState.Values
            //                        .SelectMany(v => v.Errors)
            //                        .Select(e => e.ErrorMessage);

            //    _logger.LogWarning("Model state is invalid. Errors: {Errors}", string.Join(", ", errorMessages));

            //    // Return error response to the client
            //    return Json(new { success = false, message = "Invalid data provided." });
            //}

            if (_context == null)
            {
                _logger.LogError("Database context is null.");
                return StatusCode(500, "Internal server error: Database context is not initialized.");
            }

            try
            {
                // Fetch the existing record
                var record = await _context.CriminalRecords
                    .Include(r => r.Offense)
                    .Include(r => r.Suspect)
                    .FirstOrDefaultAsync(r => r.CriminalRecordId == model.CriminalRecordId);

                if (record == null)
                {
                    _logger.LogWarning("Record not found with ID: {Id}", model.CriminalRecordId);
                    return Json(new { success = false, message = "Record not found." }); // Return JSON for not found
                }

                // Update the properties of the existing record
                record.Status = model.Status;
                record.Sentence = model.Sentence;

                // Optionally fetch and update CaseManager details
                if (record.CaseManagerId != 0)
                {
                    record.CaseManager = await _context.CaseManagers.FindAsync(record.CaseManagerId);
                    if (record.CaseManager == null)
                    {
                        _logger.LogWarning("CaseManager not found with ID: {Id}", record.CaseManagerId);
                        return Json(new { success = false, message = "Case Manager not found." }); // Return JSON for case manager not found
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Criminal record updated successfully!";
                return Json(new { success = true, message = "Criminal record updated successfully!" }); // Return JSON for success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the record with ID: {Id}", model.CriminalRecordId);
                return StatusCode(500, "Internal server error: An error occurred while processing your request.");
            }
        }
    }
    }