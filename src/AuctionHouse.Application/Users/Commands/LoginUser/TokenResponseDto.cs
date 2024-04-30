namespace AuctionHouse.Application.Users.Commands.LoginUser;

using System.Text.Json.Serialization;

public class TokenResponseDto
{
    public string Token { get; set; }

    [JsonIgnore] // Refresh token is returned in http only cookie
    public string RefreshToken { get; set; }
}