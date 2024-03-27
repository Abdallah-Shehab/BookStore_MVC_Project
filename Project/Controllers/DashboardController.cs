using Microsoft.AspNetCore.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class DashboardController : Controller
    {

        private readonly BookStoreContext _context;
        public DashboardController(BookStoreContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }


        public IActionResult GetAll()
        {
            ///Action that display all users in site(Admin & normal user)

            var users = _context.ApplicationUsers.ToList();


            return View("GetAll", users);
        }
    }
}
