namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class MenuRoutes
{
    public static string GetMenuLocation(Guid cafeId, Guid menuId) =>
        $"/api/cafes/{cafeId}/menus/{menuId}";
}
