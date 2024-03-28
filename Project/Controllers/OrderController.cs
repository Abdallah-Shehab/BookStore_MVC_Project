using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;

namespace Project.Controllers
{
    public class OrderController : Controller
    {


        private readonly ILogger<OrderController> _logger;
        IBookRepository bookRepository;

        BookStoreContext db;
        public OrderController(ILogger<OrderController> logger, IBookRepository bookRepository, BookStoreContext db)
        {
            _logger = logger;
            this.bookRepository = bookRepository;




        }

        public IActionResult Cart()
        {
            return View("Cart");
        }


       

    }
}
