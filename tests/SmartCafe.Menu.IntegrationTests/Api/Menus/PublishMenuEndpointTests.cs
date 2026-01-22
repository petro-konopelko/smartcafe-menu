using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class PublishMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task PublishMenu_ShouldReturn200_WhenMenuNew()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        // Act
        var response = await Client.PostAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/publish", content: null, ct);
        var body = await response.Content.ReadFromJsonAsync<PublishMenuResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(menu.Id, body.MenuId);
        Assert.False(string.IsNullOrWhiteSpace(body.MenuName));
        Assert.True(body.PublishedAt.Kind == DateTimeKind.Utc || body.PublishedAt.Kind == DateTimeKind.Unspecified);
    }

    [Fact]
    public async Task PublishMenu_ShouldReturn409_WhenAlreadyPublished()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(
            cafeId,
            state: MenuState.Published,
            ct: ct);

        // Act
        var second = await Client.PostAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/publish", content: null, ct);
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task PublishMenu_ShouldReturn404_WhenCafeIsDeleted()
    {
        // Arrange
        var ct = Ct;
        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        // Delete cafe
        await Factory.DeleteCafeAsync(cafeId, ct);

        // Act
        var response = await Client.PostAsync($"/api/cafes/{cafeId}/menus/{menu.Id}/publish", null, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
