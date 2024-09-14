namespace AuctionHouse.Infrastructure.Services;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenService(TokenSettings settings, IDateTime dateTime) : ITokenService
{
    private readonly TokenSettings _settings = settings;
    private readonly IDateTime _dateTime = dateTime;

    /// <inheritdoc/>
    public string CreateToken(User user)
    {
        var tokenKey = Encoding.ASCII.GetBytes(_settings.JwtSecret);

        var securityToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: GenerateDefaultClaims(user),
            notBefore: _dateTime.Now,
            expires: _dateTime.Now.AddMinutes(5),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    /// <inheritdoc/>
    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    /// <inheritdoc/>
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey =  new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.JwtSecret)),
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false,
            RequireExpirationTime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Contains("hmac-sha256", StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    private IEnumerable<Claim> GenerateDefaultClaims(User user) =>
    [
        new Claim(ClaimTypes.Name, user.Id),
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Iss, _settings.Issuer),
        new Claim(JwtRegisteredClaimNames.Aud, _settings.Audience),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("username", user.UserName!)
    ];
}
