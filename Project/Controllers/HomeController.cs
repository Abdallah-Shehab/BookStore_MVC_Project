using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Repositories;
using System.Diagnostics;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IBookRepository IbookRepository;
        public HomeController(ILogger<HomeController> logger, IBookRepository IbookRepo)
        {
            _logger = logger;
            IbookRepository = IbookRepo; 
        }


        public IActionResult Index()
        {

            return View("index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult AllCategories()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult BookDetails(int id)
        {
            return View(IbookRepository.GetBookDetails(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
