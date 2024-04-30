namespace AuctionHouse.Infrastructure.UnitTests.Services;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

public class TokenServiceTests
{
    [Fact]
    public void CreateToken_ShouldCreateJwtToken_WhenRequestedForAUser()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var mockDateTime = new Mock<IDateTime>();

        mockConfiguration.Setup(x => x["JWT_SECRET"]).Returns("1234567890Secret123456789Key12345678");
        mockConfiguration.Setup(x => x["ISSUER"]).Returns("Test1");
        mockConfiguration.Setup(x => x["AUDIENCE"]).Returns("Test2");
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17, 0, 0, 0, DateTimeKind.Utc));

        var tokenService = new TokenService(mockConfiguration.Object, mockDateTime.Object);

        var result = tokenService.CreateToken(new User { Id = "1", Email = "test@test.com", UserName = "test" });
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result);

        token.Issuer.Should().Be("Test1");
        token.Audiences.Should().Contain("Test2");
        token.Claims.Should().Contain(x => x.Type == ClaimTypes.Name && x.Value == "1");
        token.Claims.Should().Contain(x => x.Type == JwtRegisteredClaimNames.Sub && x.Value == "1");
        token.Claims.Should().Contain(x => x.Type == JwtRegisteredClaimNames.Email && x.Value == "test@test.com");
        token.Claims.Should().Contain(x => x.Type == JwtRegisteredClaimNames.Iss && x.Value == "Test1");
        token.Claims.Should().Contain(x => x.Type == JwtRegisteredClaimNames.Aud && x.Value == "Test2");
        token.Claims.Should().Contain(x => x.Type == JwtRegisteredClaimNames.Jti);
        token.Claims.Should().Contain(x => x.Type == "username", "test");
        token.ValidFrom.Should().Be(new DateTime(2022, 10, 17, 0, 0, 0, DateTimeKind.Utc));
        token.ValidTo.Should().Be(new DateTime(2022, 10, 17, 0, 5, 0, DateTimeKind.Utc));
        token.SignatureAlgorithm.Should().Be(SecurityAlgorithms.HmacSha256Signature);
    }

    [Fact]
    public void CreateRefreshToken_ShouldCreateRandomRefreshToken_WhenRequested()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var mockDateTime = new Mock<IDateTime>();

        var tokenService = new TokenService(mockConfiguration.Object, mockDateTime.Object);

        var result = tokenService.CreateRefreshToken();
        var token = Convert.FromBase64String(result);

        token.Should().NotBeNull();
        token.Count().Should().Be(32);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnClaimsPrincipal_WhenProvidedAnExpiredToken()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var mockDateTime = new Mock<IDateTime>();

        mockConfiguration.Setup(x => x["JWT_SECRET"]).Returns("1234567890Secret123456789Key12345678");
        mockConfiguration.Setup(x => x["ISSUER"]).Returns("Test1");
        mockConfiguration.Setup(x => x["AUDIENCE"]).Returns("Test2");
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17, 0, 0, 0, DateTimeKind.Utc));

        var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: "Test1",
            audience: "Test2",
            claims: new[] { new Claim("test", "claim") },
            notBefore: new DateTime(2022, 10, 16, 0, 0, 0, DateTimeKind.Utc),
            expires: new DateTime(2022, 10, 16, 1, 0, 0, DateTimeKind.Utc),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890Secret123456789Key12345678")), SecurityAlgorithms.HmacSha256Signature)));

        var tokenService = new TokenService(mockConfiguration.Object, mockDateTime.Object);

        var result = tokenService.GetPrincipalFromExpiredToken(token);

        result.Claims.Should().Contain(x => x.Type == "test" && x.Value == "claim");
    }
}