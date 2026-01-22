using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Cafes;

public class GetCafeEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task GetCafe_ShouldReturn200_WhenCafeExists()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}", ct);
        var body = await response.Content.ReadFromJsonAsync<CafeDto>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(cafeId, body.Id);
        Assert.Null(body.UpdatedAt);
    }

    [Fact]
    public async Task GetCafe_ShouldReturn404_WhenCafeDoesNotExist()
    {
        // Arrange
        var cafeId = Guid.NewGuid();
        var ct = Ct;

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCafe_ShouldReturn404_WhenCafeIsDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Delete cafe
        await Client.DeleteAsync($"/api/cafes/{cafeId}", ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
