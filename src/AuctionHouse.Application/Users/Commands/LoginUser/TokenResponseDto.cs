namespace AuctionHouse.Application.Users.Commands.LoginUser;

using System.Text.Json.Serialization;

// Note: refresh token is returned in http only cookie.
public record TokenResponseDto(string Token, [property: JsonIgnore] string RefreshToken);