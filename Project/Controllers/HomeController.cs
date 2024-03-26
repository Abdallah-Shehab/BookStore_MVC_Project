using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;
using System.Diagnostics;

namespace Project.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IBookRepository bookRepository;

        BookStoreContext db;
        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository, BookStoreContext db)
        {
            _logger = logger;
            this.bookRepository = bookRepository;
            this.db = db;
        }


        public IActionResult Index()
        {
            var books = bookRepository.GetAll();
            return View("index");
            //return Json(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View("Cart");
        }
        public IActionResult AllCategories()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        //public IActionResult ContactUs()
        //{
        //    return View();
        //}
        public IActionResult BookDetails(int id)
        {
            Book book = bookRepository.GetById(id);
            var comments = db.Comments.Where(x => x.book_id == book.ID)
                .Select(b => new CommentVM { Comment = b.comment, Date = b.Date, rate = b.rate, userFName = b.user.FirstName, userLName = b.user.LastName }).ToList();

            BookDetailsVM bookvm = new BookDetailsVM()
            {
                ID = book.ID,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                Rate = book.Rate,
                Image = book.Image,
                Quantity = book.Quantity,
                categoryID = book.Category_id,
                commentsNum = comments.Count,
                Author = db.Authors.FirstOrDefault(x => x.ID == book.Author_id),
                Category = db.Categories.FirstOrDefault(x => x.ID == book.Category_id),
                Discount = db.Discounts.FirstOrDefault(x => x.ID == book.Discount_id),
                authorBooks = db.Books.Where(x => x.Author_id == book.Author_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Category = x.Category, commentsNum = db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                categoryBooks = db.Books.Where(x => x.Category_id == book.Category_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Author = x.Author, commentsNum = db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                Comments = comments
            };
            return View("BookDetails", bookvm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
