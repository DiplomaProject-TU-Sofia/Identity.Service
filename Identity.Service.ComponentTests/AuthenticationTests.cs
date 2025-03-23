using FluentAssertions;
using Identity.Service.API.Models;
using Identity.Service.Data.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Identity.Service.Tests
{
	[Collection("WebAppFactory")]
	public class AuthenticationTests : TestBase
	{
		private readonly HttpClient _client;

		public AuthenticationTests(CustomWebApplicationFactory factory) : base(factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task Register_User_ShouldReturnOk()
		{
			// Arrange
			var newUser = new RegisterModel
			{
				FirstName = "Test",
				Email = "test@example.com",
				Password = "Passw0rd*/*",
				ConfirmPassword = "Passw0rd*/*"
			};

			var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");

			// Act
			var response = await _client.PostAsync("/api/auth/register", content);

			// Assert
			response.EnsureSuccessStatusCode(); 
		}

		[Fact]
		public async Task Register_User_With_Existing_Email_ShouldReturnBadRequest()
		{
			// Arrange
			var userManager = GetUserManager();
			var existingUser = new ApplicationUser { Email = "existing_email@example.com", UserName = "existing_email@example.com" };
			await userManager.CreateAsync(existingUser, "TestPassword123!");
			var newUser = new RegisterModel
			{
				FirstName = "Test",
				Email = "existing_email@example.com",
				Password = "Passw0rd*/*",
				ConfirmPassword = "Passw0rd*/*"
			};

			var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");

			// Act
			var response = await _client.PostAsync("/api/auth/register", content);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

	}
}
