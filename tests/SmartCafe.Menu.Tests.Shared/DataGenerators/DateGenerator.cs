using Bogus;

namespace SmartCafe.Menu.Tests.Shared.DataGenerators;

public static class DateGenerator
{
    private static readonly Faker _faker = new();

    public static DateTime GenerateRecentUtcDateTime()
        => _faker.Date.Recent().ToUniversalTime();

    public static DateTime GeneratePastUtcDateTime()
        => _faker.Date.Past().ToUniversalTime();

    public static DateTime GenerateFutureUtcDateTime()
        => _faker.Date.Future().ToUniversalTime();

}
