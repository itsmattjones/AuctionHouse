namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class RefreshTokenCommandHandlerTest
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IDateTime> _dateTimeProvider;
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTest()
    {
        var mockLogger = new Mock<ILogger<RefreshTokenCommandHandler>>();

        _tokenService = new Mock<ITokenService>();
        _dateTimeProvider = new Mock<IDateTime>();

        _userManager = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(),
            new List<IUserValidator<User>>(),
            new List<PasswordValidator<User>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), 
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object);

        _sut = new RefreshTokenCommandHandler(_tokenService.Object, _userManager.Object,
            _dateTimeProvider.Object, mockLogger.Object);
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldProvideNewTokens_WhenTheRefreshTokenIsValid()
    {
        var claimsPricipal = new Mock<ClaimsPrincipal>();
        claimsPricipal.SetupGet(x => x.Claims).Returns([new Claim(ClaimTypes.Email, "test@email.com")]);

        var user = new User { RefreshToken = "TestToken2", RefreshTokenExpiry = new DateTime(2022, 10, 18) };

        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPricipal.Object);
        _tokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken3");
        _tokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken4");

        var result = await _sut.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("TestToken3");
        result.Value.RefreshToken.Should().Be("TestToken4");
        user.RefreshToken.Should().Be("TestToken4");
        user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 19));
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheTokenUserCouldNotBeFound()
    {
        _tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());

        var result = await _sut.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Could not find user associated with the provided tokens.");
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheRefreshTokenUserCouldNotBeFound()
    {
        _tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User { RefreshToken = "BadToken" });

        var result = await _sut.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Could not find user associated with the provided tokens.");
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_ShouldThrowRefreshTokenException_WhenTheRefreshTokenHasExpired()
    {
        _tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(new ClaimsPrincipal());
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User { RefreshToken = "TestToken2", RefreshTokenExpiry = new DateTime(2022, 10, 16) });
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new RefreshTokenCommand { Token = "TestToken1", RefreshToken = "TestToken2" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("The refresh token has expired.");
    }
}