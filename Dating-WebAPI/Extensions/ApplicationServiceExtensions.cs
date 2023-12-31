using Dating_WebAPI.Data;
using Dating_WebAPI.Helpers;
using Dating_WebAPI.Interfaces;
using Dating_WebAPI.Repository;
using Dating_WebAPI.Repository.Interfaces;
using Dating_WebAPI.Services;
using Dating_WebAPI.SignalR;
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<LogUserActivity>();
        services.AddSingleton<PresenceTracker>();

        services.AddSignalR();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());   

        return services;
    }
}
