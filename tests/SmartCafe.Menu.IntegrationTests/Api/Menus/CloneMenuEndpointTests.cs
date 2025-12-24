using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.API.Models.Requests;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class CloneMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task CloneMenu_ShouldReturn201_WhenRequestValid()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, name: "Source", ct: ct);

        var request = new CloneMenuRequest(NewName: "Cloned");

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/clone", request, ct);
        var clone = await response.Content.ReadFromJsonAsync<CreateMenuResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(clone);
        Assert.Equal(cafeId, clone.CafeId);
        Assert.NotEqual(menu.Id, clone.MenuId);

        var get = await Client.GetAsync($"/api/cafes/{cafeId}/menus/{clone.MenuId}", ct);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task CloneMenu_ShouldReturn400_WhenNameMissing()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        var request = new CloneMenuRequest(NewName: "");

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/clone", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CloneMenu_ShouldReturn404_WhenSourceMenuDoesNotExist()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        var request = new CloneMenuRequest(NewName: "Cloned");

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus/{Guid.NewGuid()}/clone", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
