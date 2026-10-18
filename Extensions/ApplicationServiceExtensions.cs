
using ChatApp.Data;
using ChatApp.Helpers;
using ChatApp.Interfaces;
using ChatApp.Services;
using ChatApp.SignalR;

namespace ChatApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddSignalR();
             services.AddSingleton<PresenceTracker>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(configuration => configuration.AddProfile<AutoMappingProfiles>());
            services.AddOpenApi();
            return services;
        }
    }
}