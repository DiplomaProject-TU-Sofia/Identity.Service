using Identity.Service.Data;
using Identity.Service.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Identity.Service.Tests
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
	{
		private IHost _host = default!;
		public IdentityServiceDbContext DbContext => CreateIdentityServiceDbContext();

		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityServiceDbContext>));
				if (descriptor != null)
					services.Remove(descriptor);

				services.AddDbContext<IdentityServiceDbContext>(options =>
					options.UseInMemoryDatabase("Identity_MockDb"));

				services.AddIdentityCore<ApplicationUser>()
						.AddEntityFrameworkStores<IdentityServiceDbContext>()
						.AddDefaultTokenProviders();
			});

			_host = base.CreateHost(builder);
			return _host;
		}

		public IdentityServiceDbContext CreateIdentityServiceDbContext()
		{
			var options = new DbContextOptionsBuilder<IdentityServiceDbContext>()
				.UseInMemoryDatabase("Identity_MockDb")
				.Options;

			return new IdentityServiceDbContext(options);
		}
	}
}
