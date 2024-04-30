namespace AuctionHouse.Infrastructure.Persistence.Data;

using Microsoft.EntityFrameworkCore;

public class AuctionHouseContext : DbContext
{
    public AuctionHouseContext(DbContextOptions<AuctionHouseContext> options)
        : base(options)
    {
    }
}
