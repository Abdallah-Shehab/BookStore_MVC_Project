using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Filters;
using Project.Models;
using Project.ViewModels;
using System.Text.Encodings.Web;

namespace Project.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ISenderEmail emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ISenderEmail emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
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
                    //send confirmation email
                    await SendConfirmationEmail(userRegisterVM.Email,user);

                    /*//make cookie foe the user
                    await signInManager.SignInAsync(user, false);*/
                    return View("RegistrationSuccessful");
                }

                //add errors to the modelState
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);
            }

            return View("Register", userRegisterVM);
        }

        //Method Which will send confirmation email to user
        private async Task SendConfirmationEmail(string? email, ApplicationUser user)
        {
            //Generate token 
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            //Build the Email Confirmation Link
            var ConfirmationLink = Url.Action("ConfirmEmail", "Account",
                new { UserId = user.Id, Token = token }, protocol: HttpContext.Request.Scheme);

            //Send the Confirmation Email to the User Email
            await emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please Confirm Your Account by <a href='{HtmlEncoder.Default.Encode(ConfirmationLink)}'>Clicking here</a>.", true);
        }

        //Action of Confirmation result
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(int? UserId, string Token)
        {
            if (UserId == null || Token == null)
            {
                ViewBag.Message = "The link is Invalid or Expired";
                return View();
            }


            //Find user by id
            ApplicationUser user = await userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User Id {UserId} is Invalid";
                return NotFound();
            }

            //Call the ConfirmEmailAsync Method which will mark the Email as Confirmed
            var result = await userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email";
                return View();
            }

            ViewBag.Message = "Email cannot be confirmed";
            return View();
        }

        //action opens the view(form) of resending confiramtion Email
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendConfirmationEmail(bool IsResend = true)
        {
            if (IsResend)
            {
                ViewBag.Message = "Resend Confirmation Email";
            }
            else
            {
                ViewBag.Message = "Send Confirmation Email";
            }
            return View();
        }

        //resend confirmation Email 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string Email)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(Email);

            if(user == null || await userManager.IsEmailConfirmedAsync(user))
            {
                return View("ConfirmationEmailSent");
            }

            await SendConfirmationEmail(Email, user);
            return View("ConfirmationEmailSent");
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
                    if(userFromDb.EmailConfirmed == false)
                    {
                        ModelState.AddModelError("Email", "Email not confirmed yet.");
                        return View("Login", userLoginVM);
                    }
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
