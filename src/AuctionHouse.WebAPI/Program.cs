namespace AuctionHouse.WebAPI;

using AuctionHouse.Application;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure;
using AuctionHouse.Infrastructure.Persistence.Identity;
using AuctionHouse.WebAPI.Endpoints;
using AuctionHouse.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddTransient<ICurrentUserContext, CurrentUserContext>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add Identity
        builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AuctionHouseIdentityContext>()
            .AddDefaultTokenProviders();

        // Add authentication and the token service
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwtBearerOptions =>
        {
            jwtBearerOptions.SaveToken = true;
            jwtBearerOptions.RequireHttpsMetadata = false;
            jwtBearerOptions.ClaimsIssuer = builder.Configuration["ISSUER"];
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["ISSUER"],
                ValidAudience = builder.Configuration["AUDIENCE"],
                IssuerSigningKey =  new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT_SECRET"])),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                RequireExpirationTime = false
            };
        });

        // Add authorization
        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(options =>
        {
            options.WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete, HttpMethods.Options);
            options.AllowAnyHeader();
            options.AllowAnyOrigin();
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapUserEndpoints();
        app.Run();
    }
}