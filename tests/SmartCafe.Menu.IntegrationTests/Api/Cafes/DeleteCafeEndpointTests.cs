using System.Net;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Cafes;

public class DeleteCafeEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task DeleteCafe_ShouldReturn204_WhenCafeExists()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.DeleteAsync($"/api/cafes/{cafeId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCafe_ShouldReturn404_WhenCafeDoesNotExist()
    {
        // Arrange
        var cafeId = Guid.NewGuid();
        var ct = Ct;

        // Act
        var response = await Client.DeleteAsync($"/api/cafes/{cafeId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCafe_ShouldReturn404_WhenCafeAlreadyDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Delete cafe first time
        await Client.DeleteAsync($"/api/cafes/{cafeId}", ct);

        // Act - try to delete again
        var response = await Client.DeleteAsync($"/api/cafes/{cafeId}", ct);

        // Assert
        // Should return 404 because GetActiveByIdAsync filters deleted cafes
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
