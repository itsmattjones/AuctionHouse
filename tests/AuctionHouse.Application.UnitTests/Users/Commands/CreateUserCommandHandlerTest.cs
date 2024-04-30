namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Users.Commands.CreateUser;
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
        await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

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
        var action = async () => await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        await action.Should().ThrowAsync<UserRegistrationException>().WithMessage("Username test is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserAlreadyExistsWithTheEmail()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var action = async () => await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        await action.Should().ThrowAsync<UserRegistrationException>().WithMessage("Email test@gmail.com is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserCouldNotBeCreatedByTheUserManager()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "99", Description = "error" }));

        var handler = new CreateUserCommandHandler(mockUserManager.Object, mockLogger.Object);
        var action = async () => await handler.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<UserRegistrationException>();
        exception.And.Message.Should().Be("Could not create user [test]");
        exception.And.Data["errors"].As<string[]>().Should().Contain("99: error");
    }
}