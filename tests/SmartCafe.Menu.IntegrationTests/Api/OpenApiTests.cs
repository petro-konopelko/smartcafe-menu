using System.Net;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api;

public class OpenApiTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task OpenApi_Json_ShouldBeAccessible()
    {
        var ct = Ct;

        // Act
        var response = await Client.GetAsync("/openapi/v1.json", ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApi_Json_ShouldContainApiMetadata()
    {
        var ct = Ct;

        // Act
        var response = await Client.GetAsync("/openapi/v1.json", ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("\"openapi\":", content);
        Assert.Contains("\"paths\":", content);
        Assert.Contains("/api/", content);
    }

    [Fact]
    public async Task Scalar_UI_ShouldBeAccessible()
    {
        var ct = Ct;

        // Act
        var response = await Client.GetAsync("/scalar/v1", ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }
}
