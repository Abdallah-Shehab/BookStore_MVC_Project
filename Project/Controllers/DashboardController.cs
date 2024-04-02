using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.ViewModels;

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

        public IActionResult Books()
        {
            var books = _context.Books.
                Select(x => new BookDetailsVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Rate = x.Rate,
                    Image = x.Image,
                    Quantity = x.Quantity,
                    Author = x.Author,
                    Category = x.Category,
                    Discount = x.Discount,
                    Admin = x.Admin
                }).ToList();

            return View("Books",books);
        }

        public IActionResult GetBookDeatils(int id)
        {
            var book = _context.Books.Where(x => x.ID == id).
                Select(x => new BookDetailsVM
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Rate = x.Rate,
                    Image = x.Image,
                    Quantity = x.Quantity,
                    Author = x.Author,
                    Category = x.Category,
                    Discount = x.Discount,
                    Admin = x.Admin
                }).FirstOrDefault();



            return Json(new
            {
                bookobj = book,
                author = book.Author.Name,
                category = book.Category.Name,
                admin = book.Admin.FirstName + " " + book.Admin.LastName,
                discount = book.Discount.Percantage
            });
        }

        public IActionResult BookComments(int id)
        {
            List<CommentVM> comments = _context.Comments.Where(x => x.book_id == id).
                Select(x => new CommentVM
                {
                    Comment = x.comment,
                    userFName = x.user.FirstName,
                    userLName = x.user.LastName,
                    rate = x.rate,
                    Date = x.Date,
                    user_id = x.user_id
                }).ToList();

            ViewBag.BookName = _context.Books.Where(x => x.ID == id).Select(x => x.Name).FirstOrDefault();


            return View("BookComments", comments);
        }
        
        
        public IActionResult redirectToDeleteBook(int id)
        {
            List<CommentVM> comments = _context.Comments.Where(x => x.book_id == id).
                Select(x => new CommentVM
                {
                    Comment = x.comment,
                    userFName = x.user.FirstName,
                    userLName = x.user.LastName,
                    rate = x.rate,
                    Date = x.Date,
                    user_id = x.user_id
                }).ToList();

            ViewBag.BookName = _context.Books.Where(x => x.ID == id).Select(x => x.Name).FirstOrDefault();


            return View("BookComments", comments);
        }
    }
}
