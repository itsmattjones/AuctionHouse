namespace AuctionHouse.Application.Common.Interfaces;

using AuctionHouse.Domain.Entities;
using System.Security.Claims;

public interface ITokenService
{
    /// <summary>
    /// Create a new JWT token for the user.
    /// </summary>
    /// <param name="user">A user.</param>
    /// <returns>A JWT token.</returns>
    string CreateToken(User user);

    /// <summary>
    /// Creates a new randomly generated refresh token. 
    /// </summary>
    /// <returns>A refresh token.</returns>
    string CreateRefreshToken();

    /// <summary>
    /// Retreives the claims principal from
    /// an expired JWT token.
    /// </summary>
    /// <param name="token">A JWT token.</param>
    /// <returns>A claims principal.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}