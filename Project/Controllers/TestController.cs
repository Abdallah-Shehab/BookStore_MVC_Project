using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class TestController : Controller
    {
        BookStoreContext db;
        public TestController(BookStoreContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var comments = db.Comments.Where(i => i.user_id == 5).Select(i => i.user.FirstName).ToList();
            string name = "";
            foreach (var comment in comments)
            {
                name += comment;
            }
            return Content(name);
        }
    }
}
