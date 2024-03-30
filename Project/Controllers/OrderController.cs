using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;

namespace Project.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly BookStoreContext _db;

        public OrderController(ILogger<OrderController> logger, IBookRepository bookRepository, BookStoreContext db)
        {
            _logger = logger;
            _bookRepository = bookRepository;
            _db = db;
        }

        public IActionResult BookDetails(int id)
        {
            Book book = _bookRepository.GetById(id);
            var comments = _db.Comments.Where(x => x.book_id == book.ID)
                .Select(b => new CommentVM { comment = b.comment, Date = b.Date, rate = b.rate, userFName = b.user.FirstName, userLName = b.user.LastName }).ToList();

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
                Author = _db.Authors.FirstOrDefault(x => x.ID == book.Author_id),
                Category = _db.Categories.FirstOrDefault(x => x.ID == book.Category_id),
                Discount = _db.Discounts.FirstOrDefault(x => x.ID == book.Discount_id),
                authorBooks = _db.Books.Where(x => x.Author_id == book.Author_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Category = x.Category, commentsNum = _db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                categoryBooks = _db.Books.Where(x => x.Category_id == book.Category_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Author = x.Author, commentsNum = _db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                Comments = comments
            };
            return View("BookDetails", bookvm);
        }


        [HttpPost]
        public IActionResult AddToOrder(int bookId, int quantity)
        {

            return RedirectToAction("BookDetails", new { id = bookId });
        }


        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            var orderDetails = _db.OrdersDetails
                .Include(od => od.Book_id)
                .Include("Order")
                .ToList();

            var orderDetailsViewModel = orderDetails.Select(od => new OrderDetailsViewModel
            {
                OrderId = od.Order_id,
                BookId = od.Book_id,
                SubTotal = od.Sub_total,
                Quantity = od.Quantity,
                BookName = od.book.Name,
                BookDescription = od.book.Description,
            }).ToList();

            return View("OrderConfirmation", orderDetailsViewModel);
        }



    }
}