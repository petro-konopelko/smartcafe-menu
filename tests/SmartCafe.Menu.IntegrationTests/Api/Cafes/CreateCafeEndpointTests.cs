using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.API.Models.Requests.Cafes;
using SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;
using SmartCafe.Menu.IntegrationTests.Fixtures;
using SmartCafe.Menu.Tests.Shared.DataGenerators;

namespace SmartCafe.Menu.IntegrationTests.Api.Cafes;

public class CreateCafeEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task CreateCafe_ShouldReturn201_WhenRequestValid()
    {
        // Arrange
        var cafeName = CafeDataGenerator.GenerateCafeName();
        var request = new CreateCafeRequest(cafeName, "contact@example.com");
        var ct = Ct;

        // Act
        var response = await Client.PostAsJsonAsync("/api/cafes", request, ct);
        var body = await response.Content.ReadFromJsonAsync<CreateCafeResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.CafeId);
    }

    [Fact]
    public async Task CreateCafe_ShouldReturn400_WhenNameMissing()
    {
        // Arrange
        var request = new CreateCafeRequest("", null);
        var ct = Ct;

        // Act
        var response = await Client.PostAsJsonAsync("/api/cafes", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCafe_ShouldReturn400_WhenContactInfoTooLong()
    {
        // Arrange
        var cafeName = CafeDataGenerator.GenerateCafeName();
        var contactInfo = new string('a', 501);
        var request = new CreateCafeRequest(cafeName, contactInfo);
        var ct = Ct;

        // Act
        var response = await Client.PostAsJsonAsync("/api/cafes", request, ct);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
