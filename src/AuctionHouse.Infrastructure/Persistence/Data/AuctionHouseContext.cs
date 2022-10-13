using Microsoft.EntityFrameworkCore;

namespace AuctionHouse.Infrastructure.Persistence.Data
{
    public class AuctionHouseContext : DbContext
    {
        public AuctionHouseContext(DbContextOptions<AuctionHouseContext> options)
            : base(options)
        {
        }
    }
}
