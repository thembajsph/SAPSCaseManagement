using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;
using SAPSCaseManagement5.Models;
using SAPSCaseManagement5.ViewModels;

namespace SAPSCaseManagement5.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.Roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }

        // Admin Dashboard Landing Page
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Index");
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AuditLogs()
        {
            // Sample data for testing
            var logs = new List<AuditLog>
        {
            new AuditLog { Action = "Created", User = "JohnDoe", Timestamp = DateTime.UtcNow, Details = "Created a new case" },
            new AuditLog { Action = "Updated", User = "JaneSmith", Timestamp = DateTime.UtcNow.AddMinutes(-30), Details = "Updated case #1234" }
        };
            return View(logs);
        }

        public async Task<IActionResult> ManageRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction(nameof(ManageRoles));
        }

        [HttpPost]
        public async Task<IActionResult> UndoRoleChange(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get the last assigned role (Assuming you have this logic in place)
                var lastAssignedRole = await GetLastAssignedRoleAsync(userId);

                if (lastAssignedRole != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, lastAssignedRole);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = $"Role {lastAssignedRole} removed from {user.Email}.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Failed to remove role {lastAssignedRole} from {user.Email}.";
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // Assuming this method will fetch the last assigned role, implement your own logic
        private async Task<string> GetLastAssignedRoleAsync(string userId)
        {
            // Logic to get the last role assigned (perhaps from a database or memory)
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
            return roles.LastOrDefault();  // Simplified, you may need to fetch this from a log or history
        }

        public IActionResult CurrentLogs()
        {
            var logs = _context.AuditLogs
                .Select(a => new AuditLogViewModel
                {
                    Id = a.Id,
                    Action = a.Action,
                    User = a.User,
                    Timestamp = a.Timestamp,
                    Details = a.Details
                })
                .ToList();

            return View(logs);
        }
    }

}
