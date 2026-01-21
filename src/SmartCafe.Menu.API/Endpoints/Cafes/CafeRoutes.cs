namespace SmartCafe.Menu.API.Endpoints.Cafes;

public static class CafeRoutes
{
    public static string GetCafeLocation(Guid cafeId) =>
        $"/api/cafes/{cafeId}";
}
