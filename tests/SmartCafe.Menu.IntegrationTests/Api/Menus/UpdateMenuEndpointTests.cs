using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.API.Models.Requests;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class UpdateMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task UpdateMenu_ShouldReturn204_WhenRequestValid()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(cafeId, ct: ct);

        var updateRequest = CreateValidUpdateMenuRequest(menu, name: "Updated Menu");

        // Act
        var response = await Client.PutAsJsonAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", updateRequest, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getAfter = await Client.GetFromJsonAsync<MenuDto>($"/api/cafes/{cafeId}/menus/{menu.Id}", cancellationToken: ct);
        Assert.NotNull(getAfter);
        Assert.Equal("Updated Menu", getAfter.Name);
    }

    [Fact]
    public async Task UpdateMenu_ShouldReturn204_WhenMenuPublished()
    {
        // Arrange
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        var menu = await Factory.SeedMenuAsync(
            cafeId,
            state: MenuState.Published,
            ct: ct);

        var updateRequest = CreateValidUpdateMenuRequest(menu, name: "Updated Menu");

        // Act
        var response = await Client.PutAsJsonAsync($"/api/cafes/{cafeId}/menus/{menu.Id}", updateRequest, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getAfter = await Client.GetFromJsonAsync<MenuDto>($"/api/cafes/{cafeId}/menus/{menu.Id}", cancellationToken: ct);
        Assert.NotNull(getAfter);
        Assert.Equal("Updated Menu", getAfter.Name);
    }

    private static UpdateMenuRequest CreateValidUpdateMenuRequest(MenuEntity menu, string? name = null)
    {
        var section = menu.Sections.First();
        var item = section.Items.First();

        return new UpdateMenuRequest(
            Name: name ?? "Updated Menu",
            Sections:
            [
                new SectionDto(
                    Id: section.Id,
                    Name: "Section",
                    AvailableFrom: null,
                    AvailableTo: null,
                    Items:
                    [
                        new MenuItemDto(
                            Id: item.Id,
                            Name: "Item",
                            Description: "Description",
                            Price: new PriceDto(1, PriceUnit.PerItem, 0),
                            Image: null,
                            Ingredients: [])
                    ])
            ]);
    }
}
