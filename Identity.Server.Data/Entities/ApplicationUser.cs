using Identity.Service.Data.Entities.Enumerations;
using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.Service.Data.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public string? FirstName { get; set; }  // Nullable

		public string? LastName { get; set; }   // Nullable
		
		public Role Role { get; set; }

		// Time of registration
		public DateTime RegistrationTime { get; set; }
	}
}
