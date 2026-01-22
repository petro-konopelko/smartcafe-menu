using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class GetActiveMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task GetActiveMenu_ShouldReturn200_WhenActiveMenuExists()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(
            cafeId,
            state: MenuState.Active,
            ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus/active", ct);
        var body = await response.Content.ReadFromJsonAsync<MenuDto>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(menu.Id, body.Id);
    }

    [Fact]
    public async Task GetActiveMenu_ShouldReturn404_WhenNoActiveMenu()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus/active", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveMenu_ShouldReturn404_WhenCafeIsDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Delete cafe
        await Factory.DeleteCafeAsync(cafeId, ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus/active", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
