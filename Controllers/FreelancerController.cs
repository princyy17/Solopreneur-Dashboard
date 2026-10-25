using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceManager.Controllers
{
	public class FreelancerController : Controller
	{
		private readonly UserManager<Freelancer> userManager;
		private readonly SignInManager<Freelancer> signInManager;

		public FreelancerController(UserManager<Freelancer> userManager, SignInManager<Freelancer> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}
		[HttpGet]
        ///FreelancerController/Register
        public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(FreelancerRegisterVM modelFromReq)
		{
			if (ModelState.IsValid)
			{
				Freelancer user = new Freelancer
				{
					UserName = modelFromReq.UserName,
					Email = modelFromReq.Email,
					PhoneNumber = modelFromReq.PhoneNumber
				};
				IdentityResult result = await userManager.CreateAsync(user, modelFromReq.Password); // saved to database
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, "Freelancer"); // add role as Freelancer
					return RedirectToAction("Login");
				}
				else {
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}

				}
			}
			return View("Register", modelFromReq);
		}

		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			TempData["InfoMessage"] = "You have been logged out, We hope see you again!";
			return RedirectToAction("Login");
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(FreelancerLoginVM modelFromReq)
		{
			if (ModelState.IsValid)
			{
				// check
				Freelancer freelancer = await userManager.FindByNameAsync(modelFromReq.UserName);
				if (freelancer != null) { 
					bool found = await userManager.CheckPasswordAsync(freelancer, modelFromReq.Password);
					if (found)
					{
						await signInManager.SignInAsync(freelancer, modelFromReq.RememberMe); // create cookie

						TempData["SuccessMessage"] = $"Login successful! Welcome, {User.Identity.Name}";

						return RedirectToAction("Index", "Project");

					}
					else
					{
						ModelState.AddModelError("", "Invalid Account!");
						TempData["ErrorMessage"] = "Login failed. Please check your username or password again!.";
					}
				}
				else
				{
					ModelState.AddModelError("", "UserName or Password is Invalid!");
				}

					return View();
			}
			else
			{
				return View("Login", modelFromReq);
			}

		} 

	}
}
