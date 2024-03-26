using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Filters;
using Project.Models;
using Project.ViewModels;

namespace Project.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectAuthenticatedUsersAttribute]

        public async Task<IActionResult> Register(UserRegisterViewModel userRegisterVM)
        {
            if (ModelState.IsValid)
            {
                //Mapping viewModel
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = userRegisterVM.Email,
                    FirstName = userRegisterVM.FirstName,
                    LastName = userRegisterVM.LastName,
                    Email = userRegisterVM.Email,
                    PasswordHash = userRegisterVM.Password,
                    Address = userRegisterVM.Address,
                };

                //create user in DB using userManger(service controls the operations on user)
                IdentityResult result = await userManager.CreateAsync(user, userRegisterVM.Password);

                if (result.Succeeded)
                {
                    //make cookie foe the user
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("index", "Home");
                }

                //add errors to the modelState
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }

            return View("Register", userRegisterVM);
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RedirectAuthenticatedUsersAttribute]

        public async Task<IActionResult> Login(UserLoginViewModel userLoginVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = await userManager.FindByEmailAsync(userLoginVM.Email);

                if (userFromDb != null)
                {
                    bool founded = await userManager.CheckPasswordAsync(userFromDb, userLoginVM.Password);
                    if (founded)
                    {
                        await signInManager.SignInAsync(userFromDb, userLoginVM.RememberMe);
                        return RedirectToAction("index", "Home");
                    }
                    else
                        ModelState.AddModelError("Password", "Incorrect Password, Check your Password.");
                }
                else
                    ModelState.AddModelError("Email", "Incorrect Email, Check Your Email.");
            }
            return View("Login", userLoginVM);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            //Destroy cookie
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
