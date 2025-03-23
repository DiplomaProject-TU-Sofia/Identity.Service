using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.Data
{
	public class IdentityServiceDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
	{
		public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options)
			: base(options)
		{
		}
	}
}
