namespace AuctionHouse.Infrastructure.Configuration;

public class TokenSettings
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string JwtSecret { get; set; }
}