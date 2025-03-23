namespace Identity.Service.API.Models
{
	public class RegisterModel
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string ConfirmPassword { get; set; } = null!;
	}
}
