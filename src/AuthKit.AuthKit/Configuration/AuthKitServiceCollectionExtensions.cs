using AuthKit.AuthKit.Configuration;
using AuthKit.AuthKit.Services;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;
using AuthKit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthKit.AuthKit.Configuration;

public static class AuthKitServiceCollectionExtensions
{
    public static IServiceCollection AddAuthKit(this IServiceCollection services, Action<AuthKitBuilder>? configure = null)
    {
        var options = new AuthKitOptions();
        var builder = new AuthKitBuilder(options);
        configure?.Invoke(builder);

        services.AddSingleton(options);
        services.AddSingleton(builder.Options.Database);
        services.AddSingleton(builder.Options.Jwt);
        services.AddSingleton(builder.Options.RefreshTokens);
        services.AddSingleton(builder.Options.PasswordHashing);
        services.AddSingleton(builder.Options.RateLimiting);
        services.AddSingleton(builder.Options.MultiTenancy);
        services.AddSingleton(builder.Options.TwoFactor);
        services.AddSingleton(builder.Options.OAuth);
        services.AddSingleton(builder.Options.Scalar);
        services.AddSingleton(builder.Options.Migration);
        services.AddSingleton(builder.Options.Seed);

        ConfigureJwtAuthentication(services, options.Jwt);
        ConfigureServices(services, options);
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddAuthKit(this IServiceCollection services, string connectionString, Action<AuthKitBuilder>? configure = null)
    {
        return services.AddAuthKit(b =>
        {
            b.SetConnectionString(connectionString);
            configure?.Invoke(b);
        });
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, JwtOptions jwt)
    {
        var key = Encoding.UTF8.GetBytes(jwt.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(jwt.Audience),
                ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(jwt.ClockSkewMinutes),
                RequireExpirationTime = jwt.RequireExpirationTime,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context => Task.CompletedTask,
                OnChallenge = context => Task.CompletedTask
            };
        });

        services.AddAuthorization();
    }

    private static void ConfigureServices(IServiceCollection services, AuthKitOptions options)
    {
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHashService>();

        if (!string.IsNullOrEmpty(options.Database.ConnectionString))
        {
            services.AddDbContext<AuthKitDbContext>(builder =>
            {
                builder.UseSqlServer(options.Database.ConnectionString, sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: options.Migration.RetryCount,
                        maxRetryDelay: TimeSpan.FromMilliseconds(options.Migration.RetryDelayMs),
                        errorNumbersToAdd: null);
                });

                if (options.Database.EnableSensitiveDataLogging)
                {
                    builder.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IAuthRepository, EfCoreAuthRepository>();
        }
    }
}

