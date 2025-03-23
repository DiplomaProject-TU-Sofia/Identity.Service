using Xunit;

namespace Identity.Service.Tests
{
	[CollectionDefinition("WebAppFactory")]
	public class WebAppFactoryCollection : ICollectionFixture<CustomWebApplicationFactory>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
