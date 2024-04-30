namespace AuctionHouse.Infrastructure.Persistence.Identity;

using AuctionHouse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AuctionHouseIdentityContext : IdentityDbContext<User>
{
    public AuctionHouseIdentityContext(DbContextOptions<AuctionHouseIdentityContext> options)
        : base(options)
    {
    }
}
