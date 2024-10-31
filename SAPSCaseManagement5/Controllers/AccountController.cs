using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAPSCaseManagement5.Models;
using SAPSCaseManagement5.ViewModels;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new ApplicationUser instead of IdentityUser
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // Sign in the user with the provided password
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Redirect based on user role
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin"); // Admin Dashboard
                    }
                    else if (roles.Contains("Police Officer"))
                    {
                        return RedirectToAction("Index", "Suspects"); // Police Officer Dashboard
                    }
                    else if (roles.Contains("Station Manager"))
                    {
                        return RedirectToAction("Index", "StationManager"); // Station Manager Dashboard
                    }
                    else if (roles.Contains("Case Manager"))
                    {
                        return RedirectToAction("MyCases", "CaseManager"); // Case Manager Dashboard
                    }
                    else
                    {
                        // Fallback to home if no role matches
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            // If we reach this point, the login failed
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        // If we got this far, something failed; redisplay form
        return View(model);
    }

    // Other actions (Register, Logout, etc.)


    // POST: Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}




//[HttpPost]
//public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
//{
//    var user = await _userManager.GetUserAsync(User);
//    if (user != null)
//    {
//        user.FullName = model.FullName;
//        user.ContactNumber = model.ContactNumber;
//        await _userManager.UpdateAsync(user);
//    }

//    return RedirectToAction("Profile");
//}

