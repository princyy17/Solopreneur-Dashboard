using System.ComponentModel.DataAnnotations;

namespace FreelanceManager.ViewModels.Account
{
	public class FreelancerLoginVM
	{
		public string UserName { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		
		public bool RememberMe { get; set; }

	}
}
