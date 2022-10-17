using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure.Persistence.Data;
using AuctionHouse.Infrastructure.Persistence.Identity;
using AuctionHouse.WebAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace AuctionHouse.IntegrationTests.Fixtures
{
    public class AuctionHouseWebApplicationFactory : WebApplicationFactory<Program> 
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<AuctionHouseContext>)));
                services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<AuctionHouseIdentityContext>)));

                services.AddDbContext<AuctionHouseContext>(options => { options.UseInMemoryDatabase("AuctionHouseData"); });
                services.AddDbContext<AuctionHouseIdentityContext>(options => { options.UseInMemoryDatabase("AuctionHouseIdentity"); });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;
                var auctionHouseDataDb = scopedServices.GetRequiredService<AuctionHouseContext>();
                var auctionHouseIdentityDb = scopedServices.GetRequiredService<AuctionHouseIdentityContext>();

                auctionHouseDataDb.Database.EnsureCreated();
                auctionHouseIdentityDb.Database.EnsureCreated();

                var testUser1 = new User
                {
                    Email = "joe.bloggs@gmail.com",
                    NormalizedEmail = "joe.bloggs@gmail.com".ToUpperInvariant(),
                    UserName = "joebloggs",
                    NormalizedUserName = "joebloggs".ToUpperInvariant(),
                    RefreshToken = "abcdefg123456789",
                    RefreshTokenExpiry = DateTime.UtcNow.AddHours(1),
                    SecurityStamp = "randomStamp",
                };

                testUser1.PasswordHash = new PasswordHasher<User>().HashPassword(testUser1, "password12345");
                auctionHouseIdentityDb.Add(testUser1);

                var testUser2 = new User
                {
                    Email = "dave.bloggs@gmail.com",
                    NormalizedEmail = "dave.bloggs@gmail.com".ToUpperInvariant(),
                    UserName = "davebloggs",
                    NormalizedUserName = "davebloggs".ToUpperInvariant(),
                    RefreshToken = "abcdefghijk123456789",
                    RefreshTokenExpiry = DateTime.UtcNow.AddHours(1),
                    SecurityStamp = "randomStamp",
                };

                testUser2.PasswordHash = new PasswordHasher<User>().HashPassword(testUser2, "password12345");
                auctionHouseIdentityDb.Add(testUser2);

                auctionHouseIdentityDb.SaveChanges();
            });

            return base.CreateHost(builder);
        }
    }
}
