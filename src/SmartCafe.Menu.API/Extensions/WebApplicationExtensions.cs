using SmartCafe.Menu.API.Endpoints.Cafes;
using SmartCafe.Menu.API.Endpoints.Images;
using SmartCafe.Menu.API.Endpoints.Menus;

namespace SmartCafe.Menu.API.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication MapRoutes()
        {
            // Map API endpoints
            var api = app.MapGroup("/api");

            // Cafe endpoints
            app.MapCafeRoutes(api);

            // Menu endpoints
            app.MapMenuRoutes(api);

            // Image upload endpoints
            app.MapImageEndpoints(api);

            return app;
        }

        private void MapCafeRoutes(RouteGroupBuilder routeGroupBuilder)
        {
            // Cafe endpoints
            var cafes = routeGroupBuilder.MapGroup("/cafes");

            // CRUD operations
            cafes.MapListCafes();          // GET /api/cafes
            cafes.MapCreateCafe();         // POST /api/cafes
            cafes.MapGetCafe();            // GET /api/cafes/{cafeId}
            cafes.MapDeleteCafe();         // DELETE /api/cafes/{cafeId}
        }

        private void MapMenuRoutes(RouteGroupBuilder routeGroupBuilder)
        {
            // Menu endpoints
            var cafes = routeGroupBuilder.MapGroup("/cafes/{cafeId:guid}");
            var menus = cafes.MapGroup("/menus");

            // CRUD operations
            menus.MapCreateMenu();          // POST /api/cafes/{cafeId}/menus
            menus.MapUpdateMenu();          // PUT /api/cafes/{cafeId}/menus/{menuId}
            menus.MapGetMenu();             // GET /api/cafes/{cafeId}/menus/{menuId}
            menus.MapListMenus();           // GET /api/cafes/{cafeId}/menus
            menus.MapDeleteMenu();          // DELETE /api/cafes/{cafeId}/menus/{menuId}

            // Menu operations
            menus.MapCloneMenu();           // POST /api/cafes/{cafeId}/menus/{menuId}/clone
            menus.MapPublishMenu();         // POST /api/cafes/{cafeId}/menus/{menuId}/publish
            menus.MapActivateMenu();        // POST /api/cafes/{cafeId}/menus/{menuId}/activate
            menus.MapGetActiveMenu();       // GET /api/cafes/{cafeId}/menus/active
        }

        private void MapImageEndpoints(RouteGroupBuilder routeGroupBuilder)
        {
            // Image upload endpoints
            var images = routeGroupBuilder.MapGroup("/images");
            images.MapUploadImage();        // POST /api/images/upload
        }
    }
}
