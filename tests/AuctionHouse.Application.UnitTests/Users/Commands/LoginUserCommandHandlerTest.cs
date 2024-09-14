namespace AuctionHouse.Application.UnitTests.Users.Commands;

using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.LoginUser;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class LoginUserCommandHandlerTest
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IDateTime> _dateTimeProvider;
    private readonly LoginUserCommandHandler _sut;

    public LoginUserCommandHandlerTest()
    {
        var mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();

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

        _sut = new LoginUserCommandHandler(_userManager.Object, _tokenService.Object, 
            _dateTimeProvider.Object, mockLogger.Object);
    }

    [Fact]
    public async Task LoginUserCommandHandler_ShouldReturnTokens_WhenUserExistsAndCredentialsAreCorrect()
    {
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        _tokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        _tokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var result = await _sut.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("TestToken1");
        result.Value.RefreshToken.Should().Be("TestToken2");
    }

    [Fact]
    public async Task LoginUserCommandHandler_ShouldThrowUserLoginException_WhenUserDoesNotExist()
    {
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        _tokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        _tokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        _userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var result = await _sut.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
    }

    [Fact]
    public async Task LoginUserCommandHandler_ShouldThrowUserLoginException_WhenUserExistsButThePasswordIsIncorrect()
    {
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        _tokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        _tokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
    }
}