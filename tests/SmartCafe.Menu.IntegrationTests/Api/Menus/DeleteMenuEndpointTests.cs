using System.Net;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class DeleteMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Theory]
    [InlineData(MenuState.New)]
    [InlineData(MenuState.Published)]
    public async Task DeleteMenu_ShouldReturn204_WhenMenuStateValid(MenuState state)
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, state: state, ct: ct);

        // Act
        var response = await Client.DeleteAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getAfter = await Client.GetAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", ct);
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task DeleteMenu_ShouldReturn404_WhenCafeIsDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        // Delete cafe
        await Factory.DeleteCafeAsync(cafeId, ct);

        // Act
        var response = await Client.DeleteAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
