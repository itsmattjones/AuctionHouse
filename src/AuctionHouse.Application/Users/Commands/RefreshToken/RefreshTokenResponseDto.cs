namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using System.Text.Json.Serialization;

public class RefreshTokenResponseDto
{
    public string Token { get; set; }

    [JsonIgnore] // Refresh token is returned in http only cookie
    public string RefreshToken { get; set; }
}