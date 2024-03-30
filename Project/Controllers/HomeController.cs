using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;
using System.Diagnostics;

namespace Project.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		IBookRepository bookRepository;
		private readonly UserManager<ApplicationUser> userManager;



		BookStoreContext db;
		public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository, BookStoreContext db, UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			this.bookRepository = bookRepository;
			this.db = db;
			this.userManager = userManager;
		}


		//[Authorize]
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
				.Select(b => new CommentVM { comment = b.comment, Date = b.Date, rate = b.rate, userFName = b.user.FirstName, userLName = b.user.LastName }).OrderByDescending(x => x.Date).ToList();

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

		[HttpPost]
		public JsonResult addReview(int bookID, string comment, int rate)
		{
			// Get the current user's username or email
			string userName = User.Identity.Name;

			// Get the user object from the database using UserManager
			ApplicationUser user = userManager.FindByNameAsync(userName).Result;
			if (user != null)
			{
                // Get the user's ID
                int userId = user.Id;
                var userData = db.Users.FirstOrDefault(x => x.Id == userId);


				//check if there is comment before or not
				var test = db.Comments.FirstOrDefault(x => x.book_id == bookID && x.user_id == userId);
				
				
				if(test == null)
				{

                    CommentVM CommentVM = new CommentVM()
                    {
                        comment = comment,
                        rate = rate / 10M,    // update the final rate of the book after adding each rate
                        user_id = userId,
                        book_id = bookID,
                        Date = DateTime.Now,
                        userFName = userData.FirstName.ToString(),
                        userLName = userData.LastName.ToString()
                    };

                    Comment finalComment = new Comment(CommentVM);

                    db.Comments.Add(finalComment);
                    db.SaveChanges();

                    //to update  book's total rate
                    var book = db.Books.Where(x => x.ID == bookID).FirstOrDefault();
                    var peopleCount = db.Comments.Where(x => x.book_id == bookID).Count();
                    var ExactRate = (book.Rate ?? 0.0m + (rate / 10M)) / peopleCount;
                    book.Rate = ExactRate;
                    db.Update(book);
                    db.SaveChanges();


                    return Json(CommentVM);
                }
				else
                {
                    return Json("no more than one");
                }

            }
			else
			{
				return Json("no user");
			}
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
