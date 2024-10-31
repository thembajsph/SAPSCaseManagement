using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAPSCaseManagement5.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SAPSCaseManagement5.Models;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with SQL Server connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services, specify ApplicationUser and IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure Identity options
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Use ApplicationDbContext for identity persistence
.AddDefaultTokenProviders(); // Default token providers for email confirmation, password reset, etc.

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page if unauthorized
    options.LogoutPath = "/Account/Logout"; // Redirect to logout page
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect for access denied
    options.SlidingExpiration = true; // Enable sliding expiration for cookies
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set cookie expiration time
});

// Register Razor Pages (optional, for default identity views like Login, Register, etc.)
builder.Services.AddRazorPages().AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});

builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Define default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages for Identity
app.MapRazorPages();

// Seed roles, users, and case managers at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await SeedRoles(roleManager);
    await SeedUsers(userManager);
    await SeedCaseManagers(dbContext);
}

app.Run();

// Seeding roles
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "Police Officer", "Case Manager", "Station Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Seeding users
async Task SeedUsers(UserManager<ApplicationUser> userManager)
{
    await CreateUserIfNotExists(userManager, "admin1@saps.com", "Admin@1234", "Admin");
    await CreateUserIfNotExists(userManager, "police1@saps.com", "Police@1234", "Police Officer");
    await CreateUserIfNotExists(userManager, "caseManager1@saps.com", "Manager@1234", "Case Manager");
    await CreateUserIfNotExists(userManager, "stationManager1@saps.com", "Station@1234", "Station Manager");
}

// Seeding case managers
async Task SeedCaseManagers(ApplicationDbContext dbContext)
{
    if (!dbContext.CaseManagers.Any())
    {
        dbContext.CaseManagers.AddRange(
            new CaseManager { ManagerName = "John Doe", CaseCount = 1, Email = "johndoe@saps.com" },
            new CaseManager { ManagerName = "Jane Smith", CaseCount = 2, Email = "janesmith@saps.com" },
            new CaseManager { ManagerName = "Emily Davis", CaseCount = 3, Email = "emilydavis@saps.com" }
        );
        await dbContext.SaveChangesAsync();
    }
}

// Helper method to create users if they do not exist
// Helper method to create users if they do not exist
async Task CreateUserIfNotExists(UserManager<ApplicationUser> userManager, string email, string password, string role)
{
    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            // Set CaseManagerId to null or some default value since we don't have currentUser context
            CaseManagerId = null // Adjust as necessary; replace with a default if applicable
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        else
        {
            // Log or handle creation errors if needed
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating user: {error.Description}");
            }
        }
    }
}

