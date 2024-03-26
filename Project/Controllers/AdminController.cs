using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult GetAll()
        {
            return View ("GetAll");
        }


        public IActionResult GetById(int id)
        {
            return View("GetById");
        }
    }
}
