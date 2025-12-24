using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.API.Models.Requests;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;

namespace SmartCafe.Menu.IntegrationTests.Api.Menus;

public class CreateMenuEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task CreateMenu_ShouldReturn201_WhenRequestValid()
    {
        // Arrange
        var request = CreateValidCreateMenuRequest(name: "Menu");
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus", request, ct);
        var body = await response.Content.ReadFromJsonAsync<CreateMenuResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(body);
        Assert.Equal(cafeId, body.CafeId);
        Assert.NotEqual(Guid.Empty, body.MenuId);
    }

    [Fact]
    public async Task CreateMenu_ShouldReturn400_WhenNameMissing()
    {
        // Arrange
        var request = CreateValidCreateMenuRequest(name: "");
        var ct = Ct;

        var cafeId = Guid.NewGuid();
        await Factory.SeedCafeAsync(cafeId, ct: ct);

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMenu_ShouldReturn404_WhenCafeDoesNotExist()
    {
        // Arrange
        var cafeId = Guid.NewGuid();
        var request = CreateValidCreateMenuRequest();
        var ct = Ct;

        // Act
        var response = await Client.PostAsJsonAsync($"/api/cafes/{cafeId}/menus", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public static CreateMenuRequest CreateValidCreateMenuRequest(string? name = null)
    {
        return new CreateMenuRequest(
            Name: name ?? "Menu",
            Sections:
            [
                new SectionDto(
                    Id: null,
                    Name: "Section",
                    AvailableFrom: null,
                    AvailableTo: null,
                    Items:
                    [
                        new MenuItemDto(
                            Id: null,
                            Name: "Item",
                            Description: "Description",
                            Price: new PriceDto(1, PriceUnit.PerItem, 0),
                            Image: null,
                            Ingredients: [])
                    ])
            ]);
    }
}
