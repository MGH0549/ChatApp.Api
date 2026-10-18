using System.Security.Claims;
using System.Text;
using ChatApp.Data;
using ChatApp.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Extensions
{
    public static class IdentityServicesExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<AppUser, AppRoles>(op =>
         {
             op.Password.RequireNonAlphanumeric = false;
         })
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

            var tokenKey = config["TokenKey"]
                ?? throw new InvalidOperationException("TokenKey must be configured.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(tokenKey)
        ),

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,

        NameClaimType = "unique_name",
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }


            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(context.Exception.ToString());

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {

            foreach (var claim in context.Principal!.Claims)
            {
                Console.WriteLine(
                    $"{claim.Type} = {claim.Value}"
                );
            }


            return Task.CompletedTask;
        }
    };

});
            services.AddAuthorization(opt =>
                       {
                           opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                           opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                       });
            return services;
        }
    }
}