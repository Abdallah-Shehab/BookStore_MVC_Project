﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;

namespace Project.Controllers
{
    public class TestController : Controller
    {
        BookStoreContext db;
        UserManager<ApplicationUser> userManager;
        IBookRepository bookRepository;
        public TestController(BookStoreContext db, IBookRepository bookRepository, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.bookRepository = bookRepository;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var comments = db.Comments.Where(i => i.user_id == 5).Select(i => i.user.FirstName).ToList();
            string name = "";
            foreach (var comment in comments)
            {
                name += comment;
            }
            return Content(name);
        }

        public IActionResult BookDetail()
        {
            //BookDetailsVM bookDetail = bookRepository.GetBookDetails(6);
            return View("BookDetail");
        }

        public async Task<IActionResult> Test()
        {
            ApplicationUser user = await userManager.FindByIdAsync("5");
            return Json(user);
        }
    }
}
