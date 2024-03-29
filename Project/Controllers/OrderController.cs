using Microsoft.AspNetCore.Mvc;
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
                .Select(b => new CommentVM { Comment = b.comment, Date = b.Date, Rate = b.rate, UserFName = b.user.FirstName, UserLName = b.user.LastName }).ToList();

            BookDetailsVM bookvm = new BookDetailsVM()
            {
                ID = book.ID,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                Rate = book.Rate,
                Image = book.Image,
                Quantity = book.Quantity,
                CategoryID = book.Category_id,
                CommentsNum = comments.Count,
                Author = _db.Authors.FirstOrDefault(x => x.ID == book.Author_id),
                Category = _db.Categories.FirstOrDefault(x => x.ID == book.Category_id),
                Discount = _db.Discounts.FirstOrDefault(x => x.ID == book.Discount_id),
                AuthorBooks = _db.Books.Where(x => x.Author_id == book.Author_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Category = x.Category, CommentsNum = _db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
                CategoryBooks = _db.Books.Where(x => x.Category_id == book.Category_id && x.ID != book.ID).Select(x => new BookDetailsVM { ID = x.ID, Name = x.Name, Price = x.Price, Rate = x.Rate, Image = x.Image, Quantity = x.Quantity, Author = x.Author, CommentsNum = _db.Comments.Where(s => s.book_id == x.ID).Count() }).ToList(),
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
            var orderDetails = db.OrdersDetails
                .Include(od => od.Book_id) 
                .Include(od => od.Order)
                .ToList();

            var orderDetailsViewModel = orderDetails.Select(od => new OrderDetailsViewModel
            {
                OrderId = od.Order_id,
                BookId = od.Book_id,
                SubTotal = od.Sub_total,
                Quantity = od.Quantity,
                BookName = od.Book.Name,
                BookDescription = od.Book.Description,
            }).ToList();

            return View("OrderConfirmation", orderDetailsViewModel);
        }



    }
}
