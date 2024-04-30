namespace AuctionHouse.Infrastructure;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Infrastructure.Persistence;
using AuctionHouse.Infrastructure.Persistence.Data;
using AuctionHouse.Infrastructure.Persistence.Identity;
using AuctionHouse.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        if (configuration["UseInMemoryDatabase"] is not null)
        {
            serviceCollection.AddDbContext<AuctionHouseContext>(o =>
                o.UseInMemoryDatabase("AuctionHouse"));

            serviceCollection.AddDbContext<AuctionHouseIdentityContext>(o => 
                o.UseInMemoryDatabase("AuctionHouseIdentity"));
        }
        else
        {
            serviceCollection.AddDbContext<AuctionHouseContext>(o => 
                o.UseSqlServer(configuration["AuctionHouseConnectionString"]));

            serviceCollection.AddDbContext<AuctionHouseIdentityContext>(o => 
                o.UseSqlServer(configuration["AuctionHouseIdentityConnectionString"]));
        }

        serviceCollection.AddScoped<ITokenService>(_ => new TokenService(configuration, new MachineDateTime()));
        serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        serviceCollection.AddScoped<IDateTime, MachineDateTime>();
        serviceCollection.AddLogging();

        return serviceCollection;
    }
}