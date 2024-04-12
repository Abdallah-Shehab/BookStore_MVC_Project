using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Project.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IBookRepository bookRepository;
        private readonly BookStoreContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderController(ILogger<OrderController> logger, IBookRepository bookRepository, BookStoreContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.bookRepository = bookRepository;
            this.db = db;
            this.userManager = userManager;
        }




        [HttpGet]
        public IActionResult BookDetails(int id)
        {
            Book book = bookRepository.GetById(id);
            var comments = db.Comments.Where(x => x.book_id == book.ID)
                .Select(b => new CommentVM { Comment = b.comment, Date = b.Date, rate = b.rate, userFName = b.user.FirstName, userLName = b.user.LastName }).ToList();


            decimal discountedPrice = book.Price;
            if (book.Discount != null)
            {
                discountedPrice = book.Price - (book.Price * (book.Discount.Percantage / 100));
            }

            BookDetailsVM bookvm = new BookDetailsVM()
            {
                ID = book.ID,
                Name = book.Name,
                Description = book.Description,
                Price = discountedPrice,
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




        [HttpPost]
        public IActionResult AddToCart(BookDetailsVM order, int quantity, decimal price)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ConfirmOrder", "Order") });
            }

            var userId = userManager.GetUserId(User);

            if (!int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Error");
            }


            var newOrder = new Order
            {
                Date = DateTime.Now,
                Time = DateTime.Now.TimeOfDay,
                Total_Price = order.Price * quantity,
                user_id = userIdInt
            };

            db.Orders.Add(newOrder);
            db.SaveChanges();

            var orderDetails = new OrderDetails
            {
                Order_id = newOrder.ID,
                Book_id = order.ID,
                Sub_total = order.Price * quantity,
                Quantity = quantity
            };



            db.OrdersDetails.Add(orderDetails);
            db.SaveChanges();

            return RedirectToAction("OrderSummary");
        }



        [HttpGet]
        public IActionResult OrderSummary()
        {
            var orderDetails = db.OrdersDetails
                .Include(od => od.book)
                .ToList();

            var bookDetailsVMs = orderDetails.Select((od, index) =>
            {
                try
                {
                    var bookDetailsVM = new BookDetailsVM
                    {
                        ID = od.Order_id,
                        Name = od.book != null ? od.book.Name : "N/A",
                        // Fetch price from the Book entity
                        Price = od.book != null ? od.book.Price : 0.0m,
                        Quantity = od.Quantity != null ? od.Quantity : 0,
                        Image = od.book != null ? od.book.Image : null
                    };

                    return bookDetailsVM;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error processing item at index {index}: {ex.Message}", ex);
                }
            }).ToList();

            return View(bookDetailsVMs);
        }




    }
}
