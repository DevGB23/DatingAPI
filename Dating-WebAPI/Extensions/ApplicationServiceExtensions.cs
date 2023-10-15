using Dating_WebAPI.Data;
using Dating_WebAPI.Interfaces;
using Dating_WebAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services
    , IConfiguration config)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(config["ConnectionString:Dating"]));

        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();    

        return services;
    }
}
