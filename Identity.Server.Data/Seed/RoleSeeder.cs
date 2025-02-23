using Identity.Service.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class RoleSeeder
{
	public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

		// Define roles you want to seed
		var roles = new string[] { "User", "Worker", "Admin" };

		foreach (var role in roles)
		{
			var roleExist = await roleManager.RoleExistsAsync(role);
			if (!roleExist)
			{
				await roleManager.CreateAsync(new IdentityRole<Guid>(role));
			}
		}

		// Seed Admin user if not already present
		var adminUser = await userManager.FindByEmailAsync("admin@email.com");
		if (adminUser == null)
		{
			var admin = new ApplicationUser
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
				UserName = "admin@email.com",
				Email = "admin@email.com",
				FirstName = "Admin",
				LastName = null,
				RegistrationTime = DateTime.UtcNow
			};
			var result = await userManager.CreateAsync(admin, "Passw0rd*/*");
			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(admin, "Admin");
			}
		}
	}
}
