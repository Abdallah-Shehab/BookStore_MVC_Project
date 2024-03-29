
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Filters;
using Project.Models;
using Project.ViewModels;

using System.Security.Claims;

using System.Text.Encodings.Web;

namespace Project.Controllers
{

	public class AccountController : Controller
	{
		#region Inject Services
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ISenderEmail emailSender;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ISenderEmail emailSender)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.emailSender = emailSender;
		}
		#endregion

		#region Rgisteration
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
					image = "Defualt.png"
				};

				//create user in DB using userManger(service controls the operations on user)
				IdentityResult result = await userManager.CreateAsync(user, userRegisterVM.Password);

				if (result.Succeeded)
				{
					//send confirmation email
					await SendConfirmationEmail(userRegisterVM.Email, user);

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
		#endregion




		#region Resend Confirmation Email
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

			if (user == null || await userManager.IsEmailConfirmedAsync(user))
			{
				return View("ConfirmationEmailSent");
			}

			await SendConfirmationEmail(Email, user);
			return View("ConfirmationEmailSent");
		}
		#endregion

		#region Log in
		[HttpGet]
		public IActionResult LogIn(int? bookID)
		{
			ViewBag.Confirm = true;
			ViewBag.bookID = bookID;
			return View("Login");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RedirectAuthenticatedUsersAttribute]

		public async Task<IActionResult> Login(UserLoginViewModel userLoginVM, int? bookID)
		{
			ViewBag.Confirm = true;
			if (ModelState.IsValid)
			{
				ApplicationUser userFromDb = await userManager.FindByEmailAsync(userLoginVM.Email);

				if (userFromDb != null)
				{
					if (userFromDb.EmailConfirmed == false)
					{
						ModelState.AddModelError("Email", "Email not confirmed yet, Press Confirm Email.");
						ViewBag.Confirm = false;
						return View("Login", userLoginVM);
					}
					bool founded = await userManager.CheckPasswordAsync(userFromDb, userLoginVM.Password);
					if (founded)
					{
						//add cookie extra info

						List<Claim> claims = [new Claim("image", userFromDb.image)];



						await signInManager.SignInWithClaimsAsync(userFromDb, userLoginVM.RememberMe, claims);
						if (bookID == null)
						{
							return RedirectToAction("index", "Home");
						}
						else
							return RedirectToActionPermanent("BookDetails", "Home", new { id = bookID });
					}
					else
						ModelState.AddModelError("Password", "Incorrect Password, Check your Password.");
				}
				else
					ModelState.AddModelError("Email", "Incorrect Email, Check Your Email.");
			}
			return View("Login", userLoginVM);
		}
		#endregion

		#region Log Out
		[HttpGet]
		public async Task<IActionResult> LogOut()
		{
			//Destroy cookie
			await signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}
		#endregion

		#region Forgot Password Confirmation
		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View("ForgotPassword");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
				if (user != null && user.EmailConfirmed == true)
				{
					await SendForgotPasswordEmail(user.Email, user);
					return RedirectToAction("ForgotPasswordConfirmation", "Account");
				}

				return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}
		#endregion

		#region Reset Password Confirmation
		// This action method will be invoked when the user clicks on the Password Reset Link in his/her email. and takes email and token from the link
		[HttpGet]
		public IActionResult ResetPassword(string? Email, string? Token)
		{
			if (Email == null && Token == null)
			{
				ViewBag.ErrorTitle = "Invalid Password Reset Token";
				ViewBag.ErrorMessage = "The Link is Expired or Invalid";
				return View("Error");
			}
			else
			{
				ResetPasswordViewModel viewModel = new ResetPasswordViewModel();
				viewModel.Email = Email;
				viewModel.Token = Token;
				return View(viewModel);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

					if (result.Succeeded)
					{
						return RedirectToAction("ResetPasswordConfirmation", "Account");
					}

					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}

					return View(model);
				}

				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}
		#endregion

		#region Two Private Method two send EmailConfirmation And ResetPasswordConfirmation

		//Private Method Which will send confirmation email to user
		private async Task SendConfirmationEmail(string? email, ApplicationUser user)
		{
			//Generate token 
			var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

			//Build the Email Confirmation Link
			var ConfirmationLink = Url.Action("ConfirmEmail", "Account",
				new { Email = user.Email, Token = token }, protocol: HttpContext.Request.Scheme);

			//Send the Confirmation Email to the User Email
			await emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please Confirm Your Account by <a href='{HtmlEncoder.Default.Encode(ConfirmationLink)}'>Clicking here</a>.", true);
		}

		//private method which will send reset password email
		private async Task SendForgotPasswordEmail(string? email, ApplicationUser? user)
		{
			//Generate Reset Password Token
			var token = await userManager.GeneratePasswordResetTokenAsync(user);

			//Generate Reset Password Link
			var ResetPasswordLink = Url.Action("ResetPassword", "Account"
				, new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);

			//send reset password Email to user
			await emailSender.SendEmailAsync(email, "Reset Your Password"
				, $"Please Reset Your Passowrd by <a href='{HtmlEncoder.Default.Encode(ResetPasswordLink)}'>Clicking here</a>", true);
		}
		#endregion
	}
}

