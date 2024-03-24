using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;

namespace Project.Controllers
{
    public class TestController : Controller
    {
        BookStoreContext db;

        IBookRepository bookRepository;
        public TestController(BookStoreContext db, IBookRepository bookRepository)
        {
            this.db = db;
            this.bookRepository = bookRepository;
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

        public IActionResult BookDetail()
        {
            //BookDetailsVM bookDetail = bookRepository.GetBookDetails(6);
            return View("BookDetail");
        }
    }
}
