using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.Service.Data.Models
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		// FirstName and LastName are now nullable
		public string? FirstName { get; set; }  // Nullable
		public string? LastName { get; set; }   // Nullable

		// Time of registration
		public DateTime RegistrationTime { get; set; }
	}
}
