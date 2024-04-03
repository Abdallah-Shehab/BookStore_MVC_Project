using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.ViewModels;

namespace Project.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly BookStoreContext _context;
        public DashboardController(BookStoreContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
                    Admin = x.Admin,
                    IsAvailable=x.IsAvailable
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
                    Admin = x.Admin,
                    IsAvailable=x.IsAvailable
                }).FirstOrDefault();



            return Json(new
            {
                bookobj = book,
                author = book.Author.Name,
                category = book.Category.Name,
                admin = book.Admin.FirstName + " " + book.Admin.LastName,
                discount = book.Discount?.Percantage ?? null   //book.Discount?.Percentage checks if book.Discount is not null. If it's not null, it accesses the Percentage property.
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
        
        
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.ID == id);
            book.IsAvailable=false;
            _context.Update(book);
            _context.SaveChanges();
            
            return Json("Done");
        }
        public IActionResult AddBook(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.ID == id);
            book.IsAvailable=true;
            _context.Update(book);
            _context.SaveChanges();
            
            return Json("Done");
        }
        
        public IActionResult EditBook(int id)
        {
            var adminRoleId = _context.AspNetRoles
                                      .Where(r => r.Name == "ADMIN")
                                      .Select(r => r.Id)
                                      .FirstOrDefault();
            var adminsIds = _context.AspNetUserRoles
                                            .Where(ur => ur.RoleId == adminRoleId)
                                            .Select(ur => ur.UserId)
                                            .ToList();
            BookVM bookVM = new BookVM()
            {
                book = _context.Books.FirstOrDefault(x => x.ID == id),
                authors = _context.Authors.ToList(),
                admins = _context.AspNetUsers.Where(u => adminsIds.Contains(u.Id)).ToList(),
                categories = _context.Categories.ToList(),
                discounts = _context.Discounts.ToList(),
            };
            
            return View("EditBook",bookVM);
        }


        [HttpPost]
        public async Task<IActionResult> EditBook(BookVM _book, IFormFile imageFile)
        {
            Book book = _context.Books.FirstOrDefault(x => x.ID == _book.book.ID);
            if (book != null)
            {
                book.Name = _book.book.Name;
                book.Description = _book.book.Description;
                book.Price = _book.book.Price;
                book.Quantity = _book.book.Quantity;
                book.IsAvailable = _book.book.IsAvailable;
                book.Discount_id = _book.book.Discount_id;
                book.Admin_id = _book.book.Admin_id;
                book.Author_id = _book.book.Author_id;
                book.Category_id = _book.book.Category_id;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the uploaded image to the wwwroot folder
                    var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "assets", "Images", "books");
                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    // Generate a unique file name to avoid overwriting existing files
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Update the book's image file name in the database
                    book.Image = fileName; // Store only the file name in the database
                }

                _context.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction("Books");
            }

            return NotFound();
        }

    }
}
