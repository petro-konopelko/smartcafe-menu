using Bogus;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Tests.Shared.Mocks;

namespace SmartCafe.Menu.Tests.Shared.DataGenerators;

public static class CafeDataGenerator
{
    private static readonly Faker _faker = new();

    public static string GenerateCafeName()
        => _faker.Company.CompanyName();

    public static Cafe GenerateValidCafe(Guid? cafeId = null)
    {
        var id = cafeId ?? Guid.NewGuid();
        var cafeName = GenerateCafeName();
        var dateTimeProvider = new FakeDateTimeProvider();
        dateTimeProvider.SetUtcNow(DateGenerator.GenerateRecentUtcDateTime());

        var cafeResult = Cafe.Create(id, cafeName, dateTimeProvider);
        return cafeResult.EnsureValue();
    }
}
