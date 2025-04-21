using Identity.Service.Data;
using Identity.Service.Data.Entities;
using Identity.Service.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		// Database and Identity
		services.AddDbContext<IdentityServiceDbContext>(options =>
			options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

		services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
			.AddEntityFrameworkStores<IdentityServiceDbContext>()
			.AddDefaultTokenProviders();

		services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequireDigit = false;
			options.Password.RequireLowercase = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequiredLength = 3;
			options.Password.RequiredUniqueChars = 0;
		});

		// Register services
		services.AddScoped<RegistrationService>();

		// Add Authentication using JWT Bearer
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _configuration["JwtSettings:Issuer"],
				ValidateAudience = true,
				ValidAudience = _configuration["JwtSettings:Audience"],
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])
				),
				RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
			};
		});
		services.AddCors(options =>
		{
			options.AddPolicy("AllowAll", builder =>
			{
				builder
					.AllowAnyOrigin() 
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});

		// Authorization must be added explicitly
		services.AddAuthorization();

		// Controllers and Swagger
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
		app.UseCors("AllowAll");
		// 🔐 These must be in this order
		app.UseAuthentication();
		app.UseAuthorization();

		// Apply any pending EF migrations
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
			if (dbContext.Database.IsRelational())
			{
				dbContext.Database.Migrate();
			}
		}

		// Seed roles/admin user
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		RoleSeeder.SeedRolesAndAdminAsync(serviceProvider, userManager).Wait();

		// Swagger (optional but useful for testing)
		app.UseSwagger();
		app.UseSwaggerUI();

		// 🚀 Map endpoints
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});

		logger.LogInformation("✅ Identity Service is running!");
	}
}
