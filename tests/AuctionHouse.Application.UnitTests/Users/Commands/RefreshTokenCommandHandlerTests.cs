namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.RefreshToken;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class RefreshTokenCommandHandlerTest
{
    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldProvideNewTokens_WhenTheRefreshTokenIsValid()
    {
        var user = new User { RefreshToken = "TestToken2", RefreshTokenExpiry = new DateTime(2022, 10, 18) };

        var mockTokenService = new Mock<ITokenService>();
        var mockDateTime = new Mock<IDateTime>();
        var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken3");
        mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken4");

        var handler = new RefreshTokenCommandHandler(mockTokenService.Object, mockUserManager.Object, mockDateTime.Object, mockLogger.Object);
        var result = await handler.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("TestToken3");
        result.Value.RefreshToken.Should().Be("TestToken4");
        user.RefreshToken.Should().Be("TestToken4");
        user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 19));
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheTokenUserCouldNotBeFound()
    {
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTime = new Mock<IDateTime>();
        var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());

        var handler = new RefreshTokenCommandHandler(mockTokenService.Object, mockUserManager.Object, mockDateTime.Object, mockLogger.Object);
        var result = await handler.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Could not find user associated with the provided tokens.");
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheRefreshTokenUserCouldNotBeFound()
    {
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTime = new Mock<IDateTime>();
        var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User { RefreshToken = "BadToken" });

        var handler = new RefreshTokenCommandHandler(mockTokenService.Object, mockUserManager.Object, mockDateTime.Object, mockLogger.Object);
        var result = await handler.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Could not find user associated with the provided tokens.");
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheRefreshTokenHasExpired()
    {
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTime = new Mock<IDateTime>();
        var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User { RefreshToken = "TestToken2", RefreshTokenExpiry = new DateTime(2022, 10, 16) });
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var handler = new RefreshTokenCommandHandler(mockTokenService.Object, mockUserManager.Object, mockDateTime.Object, mockLogger.Object);
        var result = await handler.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("The refresh token has expired.");
    }
}