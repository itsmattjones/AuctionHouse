namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Users.Commands.CreateUser;
using AuctionHouse.Domain.Common.Result;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class CreateUserCommandHandlerTest
{
    [Fact]
    public async Task CreateUserCommandHandler_ShouldCreateANewUser_WhenValidUsernameAndEmailAndPasswordAreProvided()
    {
        var users = new List<User>();

        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<User, string>((x, y) => users.Add(x));

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var result = await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        users.Should().HaveCount(1);
        users.First().UserName.Should().Be("test");
        users.First().Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationExpection_WhenAUserAlreadyExistsWithTheUsername()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var result = await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Conflict);
        result.ErrorMessages.Should().ContainSingle("Username test is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserAlreadyExistsWithTheEmail()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var result = await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Conflict);
        result.ErrorMessages.Should().ContainSingle("Email test@gmail.com is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserCouldNotBeCreatedByTheUserManager()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "99", Description = "some error" }));

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var result = await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Critial);
        result.ErrorMessages.Should().ContainSingle("Could not create user [test]");
    }
}