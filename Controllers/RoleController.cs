using FreelanceManager.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceManager.Controllers
{
	[Authorize(Roles ="Admin")]
	public class RoleController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;

		public RoleController(RoleManager<IdentityRole> roleManager)
		{
			this.roleManager = roleManager;
		}
		public IActionResult AddNewRole()
		{
			return View();	
		}
		[HttpPost]
		public async Task<IActionResult> AddNewRole(RoleVM roleVM)
		{
			if (ModelState.IsValid)
			{
				IdentityRole role = new IdentityRole();
				role.Name = roleVM.RoleName;
				IdentityResult result =  await roleManager.CreateAsync(role);
				if (result.Succeeded)
				{
					// can add notification
					return Content("Role Added Successfully!");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
			}
			return View("AddNewRole");
		}
	}
}
