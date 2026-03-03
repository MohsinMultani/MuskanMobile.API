using Microsoft.Extensions.DependencyInjection;
using MuskanMobile.Application.Interfaces;
using MuskanMobile.Application.Products;
using MuskanMobile.Application.Services;

namespace MuskanMobile.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}

