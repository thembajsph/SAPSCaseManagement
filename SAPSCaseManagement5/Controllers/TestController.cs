using Microsoft.AspNetCore.Mvc;

namespace SAPSCaseManagement5.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: Test/TestNavbar
        public IActionResult TestNavbar()
        {
            return View(); // Looks for Views/TestNavbar.cshtml
        }
    }
}
