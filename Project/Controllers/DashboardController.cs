using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }
    }
}
