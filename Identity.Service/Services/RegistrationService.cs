using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Identity.Service.Data.Models;

namespace Identity.Service.Data.Repositories
{
	public class RegistrationService
	{
		private readonly ILogger<RegistrationService> _logger;
		private readonly IConfiguration _configuration;

		public RegistrationService(ILogger<RegistrationService> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public IServiceCollection AddIdentityServices(IServiceCollection services)
		{
			services.AddDbContext<IdentityServiceDbContext>(options =>
				options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<IdentityServiceDbContext>()
				.AddDefaultTokenProviders();

			// Log identity service registration
			_logger.LogInformation("✅ Identity Services Registered Successfully!");

			return services;
		}
	}
}
