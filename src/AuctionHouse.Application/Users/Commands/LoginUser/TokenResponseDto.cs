using System.Text.Json.Serialization;

namespace AuctionHouse.Application.Users.Commands.LoginUser
{
    public class TokenResponseDto
    {
        public string Token { get; set; }

        [JsonIgnore] // Refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
    }
}
