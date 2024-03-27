using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookStoreContext _context;
        public AdminController(BookStoreContext context)
        {
            _context = context;
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
            return View("EditAdmin");
        }
        public IActionResult DeleteAdmin(int id)
        {
            return View("DeletAdmin");
        }
        public IActionResult AddAdmin()
        {
            return View("AddAdmin");
        }
    }
}
