using System.Text.Json.Serialization;

namespace AuctionHouse.Application.Users.Commands.RefreshToken
{
    public class RefreshTokenResponseDto
    {
        public string Token { get; set; }

        [JsonIgnore] // Refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
    }
}
