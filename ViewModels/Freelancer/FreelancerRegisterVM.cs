using System.ComponentModel.DataAnnotations;

namespace FreelanceManager.ViewModels.Account
{
	public class FreelancerRegisterVM
	{
		[MinLength(3), MaxLength(50), ]
		
		
		public string UserName { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }
		[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
	ErrorMessage = "Please enter a valid email address.")]
		public string Email { get; set; }
		public string PhoneNumber { get; set; }

	}
}
