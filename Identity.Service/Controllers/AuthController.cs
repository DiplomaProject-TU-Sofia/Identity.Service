using Identity.Service.API.Models;
using Identity.Service.Data.Entities;
using Identity.Service.Data.Entities.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Service.API.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthController> _logger;

		public AuthController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IConfiguration configuration,
			ILogger<AuthController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			_logger.LogInformation("🔵 Register request received for {Email}", model.Email);

			// Check if a user with the given email already exists
			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				_logger.LogWarning("⚠️ There is an existing user with email {Email}", model.Email);
				return BadRequest(new { message = "Email is already taken" });
			}


			if (!ModelState.IsValid)
			{
				_logger.LogWarning("⚠️ Invalid registration model");
				return BadRequest(ModelState);
			}

			if (model.Password != model.ConfirmPassword)
			{
				_logger.LogWarning("⚠️ Passwords do not match for {Email}", model.Email);
				return BadRequest(new { message = "Passwords do not match" });
			}

			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				RegistrationTime = DateTime.UtcNow,
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, "User");
			}


			if (!result.Succeeded)
			{
				_logger.LogError("❌ Registration failed for {Email}: {Errors}", model.Email, result.Errors);
				return BadRequest(result.Errors);
			}

			_logger.LogInformation("✅ User {Email} registered successfully", model.Email);
			return Ok(new { message = "User registered successfully" });
		}
		
		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user); // 🔥

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.Email, user.Email)
				};

				// Add role claims
				foreach (var role in userRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role)); // 🔥 This is key
				}

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(
					issuer: _configuration["JwtSettings:Issuer"],
					audience: _configuration["JwtSettings:Audience"],
					claims: claims,
					expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"])),
					signingCredentials: creds
				);

				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					role = userRoles.FirstOrDefault()
				});
			}

			return Unauthorized();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("register-admin")]
		public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
		{
			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				_logger.LogWarning("⚠️ There is an existing user with email {Email}", model.Email);
				return BadRequest(new { message = "Email is already taken" });
			}

			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				RegistrationTime = DateTime.UtcNow,
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				// Assign "Admin" role to the new user
				await _userManager.AddToRoleAsync(user, "Admin");
				return Ok(new { message = "Admin user registered successfully" });
			}

			return BadRequest(result.Errors);
		}

		[Authorize(Roles = "Admin")] // Only Admins can access this endpoint
		[HttpPost("register-worker")]
		public async Task<IActionResult> RegisterWorker([FromBody] RegisterModel model)
		{
			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				_logger.LogWarning("⚠️ There is an existing user with email {Email}", model.Email);
				return BadRequest(new { message = "Email is already taken" });
			}

			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				RegistrationTime = DateTime.UtcNow,
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				// Assign "Worker" role to the new user
				await _userManager.AddToRoleAsync(user, "Worker");
				return Ok(new { message = "Worker user registered successfully" });
			}

			return BadRequest(result.Errors);
		}

	}

}
