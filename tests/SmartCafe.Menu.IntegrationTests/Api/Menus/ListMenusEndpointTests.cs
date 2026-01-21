using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class ListMenusEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task ListMenus_ShouldReturn200AndMenus_WhenCafeHasMenus()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu1 = await Factory.SeedMenuAsync(cafeId, name: "Menu 1", ct: ct);
        var menu2 = await Factory.SeedMenuAsync(cafeId, name: "Menu 2", ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus", ct);
        var body = await response.Content.ReadFromJsonAsync<ListMenusResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.True(body.Menus.Count >= 2);
        Assert.Contains(body.Menus, m => m.MenuId == menu1.Id);
        Assert.Contains(body.Menus, m => m.MenuId == menu2.Id);
    }

    [Fact]
    public async Task ListMenus_ShouldReturn404_WhenCafeDoesNotExist()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListMenus_ShouldReturn404_WhenCafeIsDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Delete cafe
        await Factory.DeleteCafeAsync(cafeId, ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
