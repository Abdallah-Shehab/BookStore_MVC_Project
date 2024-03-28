using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(BookStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult GetAll()
        {
            // Action to display only admins 
            var adminRoleId = _context.AspNetRoles
                                      .Where(r => r.Name == "ADMIN")
                                      .Select(r => r.Id)
                                      .FirstOrDefault();

            if (adminRoleId != null)
            {

                var adminUserIds = _context.AspNetUserRoles
                                            .Where(ur => ur.RoleId == adminRoleId)
                                            .Select(ur => ur.UserId)
                                            .ToList();

                var adminUsers = _context.AspNetUsers
                                         .Where(u => adminUserIds.Contains(u.Id))
                                         .ToList();

                return View("GetAll", adminUsers);
            }

            return View("Error");
        }



        public IActionResult EditAdmin(int id)
        {
            var admin = _context.AspNetUsers.FirstOrDefault(u => u.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View("EditAdmin", admin);
        }

        [HttpPost]
        public IActionResult SaveAdmin(ApplicationUser admin)
        {
            var existingAdmin = _context.AspNetUsers.FirstOrDefault(u => u.Id == admin.Id);

            if (existingAdmin == null)
            {
                return NotFound();
            }

            existingAdmin.FirstName = admin.FirstName;
            existingAdmin.LastName = admin.LastName;
            existingAdmin.Email = admin.Email;


            _context.SaveChanges();

            return RedirectToAction("GetAll");
        }


        public IActionResult DeleteAdmin(int id)
        {
            var admin = _context.AspNetUsers.FirstOrDefault(u => u.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View("DeleteAdmin", admin);
        }

        [HttpPost]
        public IActionResult ConfirmDeleteAdmin(int id)
        {
            var admin = _context.AspNetUsers.FirstOrDefault(u => u.Id == id);

            if (admin == null)
            {
                return NotFound();
            }

            var orderIds = _context.Orders.Where(o => o.user_id == id).Select(o => o.ID).ToList();
            var orderDetails = _context.OrdersDetails.Where(od => orderIds.Contains(od.Order_id));
            _context.OrdersDetails.RemoveRange(orderDetails);

            var orders = _context.Orders.Where(o => o.user_id == id);
            _context.Orders.RemoveRange(orders);

            var comments = _context.Comments.Where(c => c.user_id == id);
            _context.Comments.RemoveRange(comments);

            var userRoles = _context.AspNetUserRoles.Where(ur => ur.UserId == id);
            _context.AspNetUserRoles.RemoveRange(userRoles);

            var books = _context.Books.Where(b => b.Admin_id == id);
            _context.Books.RemoveRange(books);

            // Then delete the admin
            _context.AspNetUsers.Remove(admin);
            _context.SaveChanges();

            return RedirectToAction("GetAll");
        }


        public IActionResult AddAdmin()
        {
            return View("AddAdmin");
        }




    }
}
