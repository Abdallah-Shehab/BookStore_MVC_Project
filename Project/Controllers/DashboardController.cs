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
                Select(x=>new BookDetailsVM { ID=x.ID,Name=x.Name,Description=x.Description,
                    Price=x.Price,Rate=x.Rate,Image=x.Image,Quantity=x.Quantity,
                    Author=_context.Authors.FirstOrDefault(s=>s.ID==x.Author_id),
                    Category=_context.Categories.FirstOrDefault(s=>s.ID==x.Category_id),
                    Discount=_context.Discounts.FirstOrDefault(s=>s.ID==x.Discount_id),
                    Admin=_context.Users.FirstOrDefault(s=>s.Id==x.Admin_id)
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
                    Author = _context.Authors.FirstOrDefault(s => s.ID == x.Author_id),
                    Category = _context.Categories.FirstOrDefault(s => s.ID == x.Category_id),
                    Discount = _context.Discounts.FirstOrDefault(s => s.ID == x.Discount_id),
                    Admin = _context.Users.FirstOrDefault(s => s.Id == x.Admin_id)
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
    }
}
