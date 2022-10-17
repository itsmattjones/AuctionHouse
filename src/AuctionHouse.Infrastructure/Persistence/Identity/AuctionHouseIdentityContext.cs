using AuctionHouse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Persistence.Identity
{
    public class AuctionHouseIdentityContext : IdentityDbContext<User>
    {
        public AuctionHouseIdentityContext(DbContextOptions<AuctionHouseIdentityContext> options)
            : base(options)
        {
        }
    }
}
