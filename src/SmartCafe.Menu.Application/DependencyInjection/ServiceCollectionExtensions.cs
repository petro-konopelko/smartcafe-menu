using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Application.Features.Images.UploadImage;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu;
using SmartCafe.Menu.Application.Features.Menus.GetMenu;
using SmartCafe.Menu.Application.Features.Menus.ListMenus;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

namespace SmartCafe.Menu.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationLayer()
        {
            // Register handlers (Application layer)
            services.AddScoped<CreateMenuHandler>();
            services.AddScoped<UpdateMenuHandler>();
            services.AddScoped<CloneMenuHandler>();
            services.AddScoped<GetMenuHandler>();
            services.AddScoped<GetActiveMenuHandler>();
            services.AddScoped<ListMenusHandler>();
            services.AddScoped<PublishMenuHandler>();
            services.AddScoped<ActivateMenuHandler>();
            services.AddScoped<DeleteMenuHandler>();
            services.AddScoped<UploadImageHandler>();

            // Register validators
            return services;
        }
    }
}
