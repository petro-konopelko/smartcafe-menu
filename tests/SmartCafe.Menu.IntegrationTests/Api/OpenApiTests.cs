using System.Net;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api;

public class OpenApiTests : IClassFixture<MenuApiFactory>
{
    private readonly HttpClient _client;

    public OpenApiTests(MenuApiFactory factory)
    {
        _client = factory?.CreateClient() ?? throw new ArgumentNullException(nameof(factory));
    }

    [Fact]
    public async Task OpenApi_Json_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/openapi/v1.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApi_Json_ShouldContainApiMetadata()
    {
        // Act
        var response = await _client.GetAsync("/openapi/v1.json");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("\"openapi\":", content);
        Assert.Contains("\"paths\":", content);
        Assert.Contains("/api/", content);
    }

    [Fact]
    public async Task Scalar_UI_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/scalar/v1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Api_Root_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Api_Menus_Endpoints_ShouldBeAccessible()
    {
        // Arrange
        var testCafeId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/cafes/{testCafeId}/menus/active");

        // Assert - Should respond (either 200 with data or 404 if no menu, but not 500 or routing error)
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound,
            $"Expected OK or NotFound, but got {response.StatusCode}");
    }
}
