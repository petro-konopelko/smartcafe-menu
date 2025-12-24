using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class ActivateMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task ActivateMenu_ShouldReturn200_WhenMenuPublished()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();

        var menu = await Factory.SeedMenuAsync(
            cafeId,
            state: MenuState.Published,
            ct: ct);

        // Act
        var response = await Client.PostAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/activate", content: null, ct);
        var body = await response.Content.ReadFromJsonAsync<ActivateMenuResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(menu.Id, body.MenuId);
        Assert.False(string.IsNullOrWhiteSpace(body.MenuName));

        var active = await Client.GetAsync($"/api/cafes/{cafeId}/menus/active", ct);
        Assert.Equal(HttpStatusCode.OK, active.StatusCode);
        var activeMenu = await active.Content.ReadFromJsonAsync<MenuDto>(cancellationToken: ct);
        Assert.NotNull(activeMenu);
        Assert.Equal(menu.Id, activeMenu.Id);
    }

    [Fact]
    public async Task ActivateMenu_ShouldReturn409_WhenMenuNotPublished()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        // Act
        var response = await Client.PostAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/activate", content: null, ct);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
