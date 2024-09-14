namespace AuctionHouse.Infrastructure.Configuration;

public class DatabaseSettings
{
    public required string AuctionHouseConnectionString { get; set; }
    public required string AuctionHouseIdentityConnectionString { get; set; }
    public bool UseInMemoryDatabase { get; set; }
}