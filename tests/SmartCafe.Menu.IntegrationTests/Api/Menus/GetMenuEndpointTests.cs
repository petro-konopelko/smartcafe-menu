using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class GetMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task GetMenu_ShouldReturn200_WhenMenuExists()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();

        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", ct);
        var menuDto = await response.Content.ReadFromJsonAsync<MenuDto>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(menuDto);
        Assert.Equal(menu.Id, menuDto.Id);
        Assert.NotEmpty(menuDto.Sections);
    }

    [Fact]
    public async Task GetMenu_ShouldReturn404_WhenMenuDoesNotExist()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.GetAsync($"/api/cafes/{cafeId}/menus/{Guid.NewGuid()}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
