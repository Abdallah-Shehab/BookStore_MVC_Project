using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Project.Models;
using Project.ViewModels;

namespace Project.Controllers
{
    public class DashboardController : Controller
    {

        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(BookStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                    Author = _context.Authors.FirstOrDefault(s => s.ID == x.Author_id),
                    Category = _context.Categories.FirstOrDefault(s => s.ID == x.Category_id),
                    Discount = _context.Discounts.FirstOrDefault(s => s.ID == x.Discount_id),
                    Admin = _context.Users.FirstOrDefault(s => s.Id == x.Admin_id)
                }).ToList();



            return View("Books", books);
        }
        [HttpGet]
        public IActionResult AddAdmin()

        {

            List<KeyValuePair<int, string>> users_id_name = new List<KeyValuePair<int, string>>();
            var users = _context.AspNetUsers.ToList();

            foreach (var user in users)
            {
                users_id_name.Add(new KeyValuePair<int, string>(user.Id, user.FirstName + " " + user.LastName));
            }

            return View("Admin", users_id_name);
        }
        //public IActionResult getUsers(int roleID) {

        //    // Get the list of user IDs who have the specific role
        //    List<int> usersWithSpecificRoleID = _context.AspNetUserRoles
        //                                            .Where(ur => ur.RoleId == roleID)
        //                                            .Select(ur => ur.UserId)
        //                                            .ToList();

        //    // Get the list of all user IDs
        //    List<int> allUserIDs = _context.AspNetUsers
        //                                .Select(u => u.Id)
        //                                .ToList();

        //    // Get the list of user IDs who do not have the specific role
        //    List<int> usersWithoutSpecificRoleID = allUserIDs.Except(usersWithSpecificRoleID).ToList();

        //    // Get the users who do not have the specific role
        //    var usersWithoutSpecificRole = _context.AspNetUsers
        //                                        .Where(u => usersWithoutSpecificRoleID.Contains(u.Id))
        //                                        .ToList();
        //    List<KeyValuePair<int, string>> userId_Name = new List<KeyValuePair<int, string>>().ToList();
        //    foreach (var user in usersWithoutSpecificRole)
        //    {
        //        userId_Name.Add(new KeyValuePair<int, string>(user.Id, user.FirstName + " " + user.LastName));
        //    }
        //    return Json(userId_Name);

        //}

        public async Task<IActionResult> CheckAdminRole(int UserID)
        {
            var user = await _context.Users.FindAsync(UserID);
            // Check if the user has the "Admin" role
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return Json(new { isAdmin });
        }
        [HttpPost]
        public IActionResult AddAdmin(bool isAdmin, int UserID)
        {
            int roleID = _context.Roles.Where(n => n.Name == "Admin").Select(n => n.Id).FirstOrDefault();

            IdentityUserRole<int> obj = new IdentityUserRole<int>();
            obj.UserId = UserID;
            obj.RoleId = roleID;
            if (isAdmin)
            {
                _context.AspNetUserRoles.Remove(obj);
            }
            else
            {
                _context.AspNetUserRoles.Add(obj);
            }
            _context.SaveChanges();
            return RedirectToAction("AddAdmin");
            //if (ModelState.IsValid)
            //{
            //    IdentityUserRole<int> obj = new IdentityUserRole<int>();
            //    obj.UserId = vm.userID;
            //    obj.RoleId = vm.roleID;
            //    _context.AspNetUserRoles.Add(obj);
            //    _context.SaveChanges();
            //    return RedirectToAction("AddAdmin");
            //}
            //else {
            //    return View("Error");
            //}

        }
        public IActionResult GetAllUsers()
        {
            int? roleID = _context.AspNetRoles.FirstOrDefault(r => r.Name == "user")?.Id;
            if (roleID != null)
            {
                // Get the list of user IDs who have the specific role
                List<int> usersWithSpecificRoleID = _context.AspNetUserRoles
                                                    .Where(ur => ur.RoleId == roleID)
                                                    .Select(ur => ur.UserId)
                                                    .ToList();
                // Get the list of all user IDs
                List<int> allUserIDs = _context.AspNetUsers
                                            .Select(u => u.Id)
                                            .ToList();
                // Get the users who do not have the specific role
                List<ApplicationUser> usersWithoutSpecificRole = _context.AspNetUsers.Include(n => n.Orders)
                                                    .Where(u => usersWithSpecificRoleID.Contains(u.Id))
                                                    .ToList();
                return View("Users", usersWithoutSpecificRole);
            }
            return View("Error");

        }

        public IActionResult GetOrdersUser(int UserID)
        {
            List<Order> orders = _context.Orders.Where(o => o.user_id == UserID).ToList();
            return Json(orders);
        }

    }
}
