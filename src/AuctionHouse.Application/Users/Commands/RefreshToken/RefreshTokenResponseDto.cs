namespace AuctionHouse.Application.Users.Commands.RefreshToken;

using System.Text.Json.Serialization;

// Note: refresh token is returned in http only cookie.
public record RefreshTokenResponseDto(string Token, [property: JsonIgnore] string RefreshToken);