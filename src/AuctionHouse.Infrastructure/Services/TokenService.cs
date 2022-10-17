using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuctionHouse.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IDateTime _dateTime;

        /// <summary>
        /// Creates a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        public TokenService(IConfiguration configuration, IDateTime dateTime)
        {
            _configuration = configuration;
            _dateTime = dateTime;
        }

        /// <inheritdoc/>
        public string CreateToken(User user)
        {
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["JWT_SECRET"]);
            var issuer = _configuration["ISSUER"];
            var audience = _configuration["AUDIENCE"];

            var securityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: GetDefaultClaims(user, issuer, audience),
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
                ValidIssuer = _configuration["ISSUER"],
                ValidAudience = _configuration["AUDIENCE"],
                IssuerSigningKey =  new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT_SECRET"])),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false,
                RequireExpirationTime = false
            };
    
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Contains("hmac-sha256", StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private static IEnumerable<Claim> GetDefaultClaims(User user, string issuer, string audience)
        {
            return new[]
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("username", user.UserName)
            };
        }
    }
}
