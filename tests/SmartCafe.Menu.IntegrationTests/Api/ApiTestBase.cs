using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api;

public abstract class ApiTestBase : IAsyncDisposable
{

    protected ApiTestBase(DatabaseFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        Factory = new MenuApiTestFactory(fixture.ConnectionString);
        Client = Factory.CreateClient();
    }

    protected MenuApiTestFactory Factory { get; }

    protected HttpClient Client { get; }

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return Factory.DisposeAsync();
    }
}
