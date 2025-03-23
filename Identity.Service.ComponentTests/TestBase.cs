using Identity.Service.Data.Entities;
using Identity.Service.Data;
using Identity.Service.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public abstract class TestBase : IDisposable
{
	protected readonly CustomWebApplicationFactory Factory;
	protected readonly IdentityServiceDbContext DbContext;
	protected IServiceScope Scope;

	public TestBase(CustomWebApplicationFactory factory)
	{
		Factory = factory;
		DbContext = factory.DbContext;

		// Create a scope for the test
		Scope = factory.Services.CreateScope();
	}

	// Retrieve UserManager within the scope
	protected UserManager<ApplicationUser> GetUserManager()
	{
		return Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
	}

	public void Dispose()
	{
		// Dispose the scope at the end of the test
		Scope.Dispose();
		DbContext.Database.EnsureDeleted();
		DbContext.ChangeTracker.Clear();
	}
}
