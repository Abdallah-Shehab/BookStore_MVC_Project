using Microsoft.AspNetCore.Identity;
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
                    IsAvailable = x.IsAvailable
                }).ToList();

            return View("Books", books);
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
                    IsAvailable = x.IsAvailable
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
            book.IsAvailable = false;
            _context.Update(book);
            _context.SaveChanges();

            return Json("Done");
        }
        public IActionResult AddBook(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.ID == id);
            book.IsAvailable = true;
            _context.Update(book);
            _context.SaveChanges();

            return Json("Done");
        }

        public IActionResult EditBook(int id)
        {
            var adminsIds = GetAdminsIDs();
            BookVM bookVM = new BookVM()
            {
                book = _context.Books.FirstOrDefault(x => x.ID == id),
                authors = _context.Authors.ToList(),
                admins = _context.AspNetUsers.Where(u => adminsIds.Contains(u.Id)).ToList(),
                categories = _context.Categories.ToList(),
                discounts = _context.Discounts.ToList(),
            };

            return View("EditBook", bookVM);
        }

        public List<int> GetAdminsIDs()
        {
            var adminRoleId = _context.AspNetRoles
                                      .Where(r => r.Name == "ADMIN")
                                      .Select(r => r.Id)
                                      .FirstOrDefault();
            return _context.AspNetUserRoles.Where(ur => ur.RoleId == adminRoleId)
                                            .Select(ur => ur.UserId)
                                            .ToList();
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

        public IActionResult AddNewBook()
        {
            var adminsIds = GetAdminsIDs();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Discounts = _context.Discounts.ToList();
            ViewBag.Admins = _context.AspNetUsers.Where(u => adminsIds.Contains(u.Id)).ToList();

            return View("AddNewBook");
        }

        [HttpPost]
        public async Task<IActionResult> SaveNewBook(Book book, IFormFile imageFile)
        {
            //if (ModelState.IsValid == true)//C#
            if (book.Admin_id != null || book.Author_id != null || book.Category_id != null || book.Description != null || book.Name != null || book.Price != null)//C#
            {
                try
                {
                    var fileName = "";
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Save the uploaded image to the wwwroot folder
                        var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "assets", "Images", "books");
                        if (!Directory.Exists(uploadsDirectory))
                        {
                            Directory.CreateDirectory(uploadsDirectory);
                        }

                        // Generate a unique file name to avoid overwriting existing files
                        fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsDirectory, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                    }
                    book.Image = fileName;

                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Books");
                }
                catch (Exception ex)
                {
                    //send ex message to view as error inside modelstate
                    //ModelState.AddModelError("DepartmentId", "Please Select Department");
                    //ModelState.AddModelError("", ex.Message);
                    // ModelState.AddModelError("", ex.InnerException.Message);
                }
            }

            var adminsIds = GetAdminsIDs();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Discounts = _context.Discounts.ToList();
            ViewBag.Admins = _context.AspNetUsers.Where(u => adminsIds.Contains(u.Id)).ToList();
            
            return View("AddNewBook", book);
        }



        //***** Category *****//

        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();

            return View("Categories", categories);
        }

        public IActionResult AddCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.ID == id);
            category.IsAvailable = true;
            _context.Update(category);
            _context.SaveChanges();

            return Json("Done");
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.ID == id);
            category.IsAvailable = false;
            _context.Update(category);
            _context.SaveChanges();

            return Json("Done");
        }

        public IActionResult EditCategory(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.ID == id);

            return View("EditCategory", category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category _category)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.ID == _category.ID);
            if (category != null)
            {
                category.Name = _category.Name;
                category.Description = _category.Description;
                category.IsAvailable = _category.IsAvailable;

                _context.Update(category);
                _context.SaveChanges();

                return RedirectToAction("Categories");
            }

            return NotFound();
        }
        
        public IActionResult AddNewCategory()
        {
            return View("AddNewCategory");
        }
        [HttpPost]
        public IActionResult SaveNewCategory(Category _category)
        {
            //if (ModelState.IsValid == true)//C#
            if (_category.Name != null || _category.Description != null)//C#
            {
                try
                { 
                    _context.Categories.Add(_category);
                    _context.SaveChanges();
                    return RedirectToAction("Categories");
                }
                catch (Exception ex)
                {
                    //send ex message to view as error inside modelstate
                    //ModelState.AddModelError("DepartmentId", "Please Select Department");
                    //ModelState.AddModelError("", ex.Message);
                    // ModelState.AddModelError("", ex.InnerException.Message);
                }
            }

            return View("AddNewCategory", _category);
        }


    }
}
