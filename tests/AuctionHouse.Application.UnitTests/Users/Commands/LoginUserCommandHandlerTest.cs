namespace AuctionHouse.Application.UnitTests.Users.Commands;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.LoginUser;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class LoginUserCommandHandlerTest
{
    [Fact]
    public async Task LoginUserCommandHandler_ShouldReturnTokens_WhenUserExistsAndCredentialsAreCorrect()
    {
        var mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTimeService = new Mock<IDateTime>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockDateTimeService.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var handler = new LoginUserCommandHandler(mockUserManager.Object, mockTokenService.Object, mockDateTimeService.Object, mockLogger.Object);
        var response = await handler.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        response.Token.Should().Be("TestToken1");
        response.RefreshToken.Should().Be("TestToken2");
    }

    [Fact]
    public async Task LoginUserCommandHandler_ShouldThrowUserLoginException_WhenUserDoesNotExist()
    {
        var mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTimeService = new Mock<IDateTime>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockDateTimeService.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var handler = new LoginUserCommandHandler(mockUserManager.Object, mockTokenService.Object, mockDateTimeService.Object, mockLogger.Object);
        var action = async () => await handler.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        await action.Should().ThrowAsync<UserLoginException>().WithMessage("The provided credentials are incorrect!");
    }

    [Fact]
    public async Task LoginUserCommandHandler_ShouldThrowUserLoginException_WhenUserExistsButThePasswordIsIncorrect()
    {
        var mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();
        var mockTokenService = new Mock<ITokenService>();
        var mockDateTimeService = new Mock<IDateTime>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockDateTimeService.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("TestToken1");
        mockTokenService.Setup(x => x.CreateRefreshToken()).Returns("TestToken2");
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        var handler = new LoginUserCommandHandler(mockUserManager.Object, mockTokenService.Object, mockDateTimeService.Object, mockLogger.Object);
        var action = async () => await handler.Handle(new LoginUserCommand { Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        await action.Should().ThrowAsync<UserLoginException>().WithMessage("The provided credentials are incorrect!");
    }
}