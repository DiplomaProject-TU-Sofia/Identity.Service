using Identity.Service.Data;
using Identity.Service.Data.Entities;
using Identity.Service.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		// Register RegistrationService to DI container
		services.AddScoped<RegistrationService>();

		// Add DbContext and Identity services
		services.AddDbContext<IdentityServiceDbContext>(options =>
	   options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

		services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
				.AddEntityFrameworkStores<IdentityServiceDbContext>()
				.AddDefaultTokenProviders();

		// Register Identity services using RegistrationService
		services.AddScoped<RegistrationService>(sp =>
			new RegistrationService(
				sp.GetRequiredService<ILogger<RegistrationService>>(),
				_configuration
			));

		// Continue with other services
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IServiceProvider serviceProvider)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		// Apply any pending migrations at startup
		using (var scope = app.ApplicationServices.CreateScope())

		{
			var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
			if (dbContext.Database.IsRelational())
			{
				dbContext.Database.Migrate();  // This applies any pending migrations
			}
		}
		
		// Seed roles
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		RoleSeeder.SeedRolesAndAdminAsync(serviceProvider, userManager).Wait();
		
		// Log information when the application starts
		logger.LogInformation("✅ Identity Service is running!");

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
