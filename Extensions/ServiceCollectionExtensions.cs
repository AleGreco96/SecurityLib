using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SecurityLib.Models;
using SecurityLib.Services;

namespace SecurityLib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityLib(this IServiceCollection services, IConfiguration config)
        {
            services.ConfigureJwt(config);
            services.ConfigurePolicies();

            DatabaseService databaseService = new(config.GetConnectionString("SecurityDB") ?? throw new InvalidOperationException("Connection string not found."));

            services.AddSingleton(databaseService);
            services.AddSingleton<TokenService>();
            services.AddSingleton<CryptographyService>();


            return services;
        }

        private static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration config)
        {
            JwtSettings jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(key)
                        };
                    }
                    );

            return services;
        }

        private static IServiceCollection ConfigurePolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireRole("User", "Admin");
                });
            });

            return services;
        }
    }
}
