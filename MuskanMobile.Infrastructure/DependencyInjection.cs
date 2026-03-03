//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MuskanMobile.Infrastructure
//{
//    internal class DependencyInjection
//    {
//    }
//}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuskanMobile.Infrastructure.Data;
using MuskanMobile.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MuskanMobile.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MuskanMobileDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}

