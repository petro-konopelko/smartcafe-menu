using Bogus;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Tests.Shared.DataGenerators;

public static class CafeDataGenerator
{
    private static readonly Faker _faker = new();

    public static string GenerateCafeName()
        => _faker.Company.CompanyName();

    public static Cafe GenerateValidCafe(IDateTimeProvider dateTimeProvider, Guid? cafeId = null)
    {
        ArgumentNullException.ThrowIfNull(dateTimeProvider);
        var id = cafeId ?? Guid.NewGuid();
        var cafeName = GenerateCafeName();

        var cafeResult = Cafe.Create(id, cafeName, dateTimeProvider);
        return cafeResult.EnsureValue();
    }
}
