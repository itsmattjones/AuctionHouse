namespace AuctionHouse.Infrastructure;

using System;
using System.Text;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure.Configuration;
using AuctionHouse.Infrastructure.Persistence;
using AuctionHouse.Infrastructure.Persistence.Data;
using AuctionHouse.Infrastructure.Persistence.Identity;
using AuctionHouse.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabases(configuration);
        services.AddAuth(configuration);
        services.AddLogging();

        return services;
    }

    private static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(nameof(DatabaseSettings)).Get<DatabaseSettings>()!;

        if (settings.UseInMemoryDatabase)
        {
            services.AddDbContext<AuctionHouseContext>(o => o.UseInMemoryDatabase("AuctionHouse"));
            services.AddDbContext<AuctionHouseIdentityContext>(o => o.UseInMemoryDatabase("AuctionHouseIdentity"));
        }
        else
        {
            services.AddDbContext<AuctionHouseContext>(o => o.UseSqlServer(settings.AuctionHouseConnectionString));
            services.AddDbContext<AuctionHouseIdentityContext>(o => o.UseSqlServer(settings.AuctionHouseIdentityConnectionString));
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(nameof(TokenSettings)).Get<TokenSettings>()!;

        // Add Identity
        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AuctionHouseIdentityContext>()
            .AddDefaultTokenProviders();

        // Add authentication and the token service
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwtBearerOptions =>
        {
            jwtBearerOptions.SaveToken = true;
            jwtBearerOptions.RequireHttpsMetadata = false;
            jwtBearerOptions.ClaimsIssuer = settings.Issuer;
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,
                IssuerSigningKey =  new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.JwtSecret)),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                RequireExpirationTime = false
            };
        });

        // Add authorization
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        services.AddSingleton(settings);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IDateTime, MachineDateTime>();

        return services;
    }
}