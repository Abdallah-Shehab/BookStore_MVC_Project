using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project.Mapper;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;
using System.Security.Claims;

namespace Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BookStoreContext bookStore;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository userProfileRepository;

        public ProfileController(BookStoreContext bookStore, IMapper _mapper, IUserProfileRepository userProfileRepository)
        {
            this.bookStore = bookStore;
            this._mapper = _mapper;
            this.userProfileRepository = userProfileRepository;
        }
        public IActionResult Index()
        {



            UserDetails userInfo = userProfileRepository.UserDetails(getUserID());
            //return Json(result);
            return View("Profile", userInfo);
            //return Json(userInfo);
        }

        [HttpGet]
        public IActionResult Edit()
        {


            UserDetails userInfo = userProfileRepository.UserDetails(getUserID());

            return View("EditTabs", userInfo);
            //return Json(userInfo);
        }

        [HttpPost]
        public IActionResult Edit(UserDetails newUserDetails)
        {


            //UserDetails userInfo = userProfileRepository.UserDetails(getUserID());

            //return View("EditTabs", userInfo);
            return Json(newUserDetails);
        }

        public IActionResult Billing()
        {
            return View("EditTabs");
        }
        public IActionResult Security()
        {
            return View("EditTabs");
        }

        public string getUserID()
        {
            Claim idclaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string id = idclaim.Value;

            return id;
        }
    }
}
