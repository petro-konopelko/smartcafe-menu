using System.Net;
using System.Net.Http.Json;
using SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;
using SmartCafe.Menu.IntegrationTests.Extensions;
using SmartCafe.Menu.IntegrationTests.Fixtures;
using SmartCafe.Menu.Tests.Shared.DataGenerators;

namespace SmartCafe.Menu.IntegrationTests.Api.Cafes;

public class ListCafesEndpointTests(DatabaseFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task ListCafes_ShouldReturn200WithCafes_WhenCafesExist()
    {
        // Arrange
        var ct = Ct;
        var cafe1Name = CafeDataGenerator.GenerateCafeName();
        var cafe2Name = CafeDataGenerator.GenerateCafeName();
        var cafe3Name = CafeDataGenerator.GenerateCafeName();

        var cafe1 = await Factory.SeedCafeAsync(cafe1Name, "contact1@example.com", ct);
        var cafe2 = await Factory.SeedCafeAsync(cafe2Name, "contact2@example.com", ct);
        var cafe3 = await Factory.SeedCafeAsync(cafe3Name, null, ct);

        // Act
        var response = await Client.GetAsync("/api/cafes", ct);
        var body = await response.Content.ReadFromJsonAsync<ListCafesResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Cafes);

        // Response is ordered by CreatedAt descending, so sort expected cafes accordingly
        var expectedCafes = new[] { cafe1, cafe2, cafe3 }
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => (c.Id, c.Name, c.ContactInfo))
            .ToArray();

        Assert.All(expectedCafes, expected =>
        {
            Assert.Contains(body.Cafes, cafe =>
                cafe.Id == expected.Id &&
                cafe.Name == expected.Name &&
                cafe.ContactInfo == expected.ContactInfo &&
                cafe.CreatedAt != default &&
                cafe.UpdatedAt == null);
        });
    }

    [Fact]
    public async Task ListCafes_ShouldNotReturnDeletedCafes()
    {
        // Arrange
        var ct = Ct;
        var activeCafeName = CafeDataGenerator.GenerateCafeName();
        var deletedCafeName = CafeDataGenerator.GenerateCafeName();

        var activeCafe = await Factory.SeedCafeAsync(activeCafeName, "active@example.com", ct);
        var deletedCafe = await Factory.SeedCafeAsync(deletedCafeName, "deleted@example.com", ct);

        // Delete one cafe
        await Factory.DeleteCafeAsync(deletedCafe.Id, ct);

        // Act
        var response = await Client.GetAsync("/api/cafes", ct);
        var body = await response.Content.ReadFromJsonAsync<ListCafesResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);

        // Should contain active cafe but not deleted cafe
        Assert.Contains(body.Cafes, c => c.Id == activeCafe.Id);
        Assert.DoesNotContain(body.Cafes, c => c.Id == deletedCafe.Id);
    }

    [Fact]
    public async Task ListCafes_ShouldReturnCafesOrderedByCreatedAtDescending()
    {
        // Arrange
        var ct = Ct;

        // Create cafes in sequence to test CreatedAt ordering
        var cafe1 = await Factory.SeedCafeAsync("First Cafe", null, ct);
        var cafe2 = await Factory.SeedCafeAsync("Second Cafe", null, ct);
        var cafe3 = await Factory.SeedCafeAsync("Third Cafe", null, ct);

        // Act
        var response = await Client.GetAsync("/api/cafes", ct);
        var body = await response.Content.ReadFromJsonAsync<ListCafesResponse>(cancellationToken: ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);

        // Find our test cafes in the response
        var testCafes = body.Cafes
            .Where(c => c.Id == cafe1.Id || c.Id == cafe2.Id || c.Id == cafe3.Id)
            .ToList();

        Assert.Equal(3, testCafes.Count);

        // Verify they are ordered by CreatedAt descending (newest first)
        Assert.Multiple(() =>
        {
            for (var i = 0; i < body.Cafes.Count - 1; i++)
            {
                Assert.True(body.Cafes[i].CreatedAt >= body.Cafes[i + 1].CreatedAt);
            }
        });
    }
}
