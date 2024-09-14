namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System;
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
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class CreateUserCommandHandlerTest
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTest()
    {
        var mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();

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

        _sut = new CreateUserCommandHandler(_userManager.Object, mockLogger.Object);
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldCreateANewUser_WhenValidUsernameAndEmailAndPasswordAreProvided()
    {
        var users = new List<User>();

        _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<User, string>((x, y) => users.Add(x));

        var result = await _sut.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        users.Should().HaveCount(1);
        users.First().UserName.Should().Be("test");
        users.First().Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationExpection_WhenAUserAlreadyExistsWithTheUsername()
    {
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var result = await _sut.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Conflict);
        result.ErrorMessages.Should().ContainSingle("Username test is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserAlreadyExistsWithTheEmail()
    {
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var result = await _sut.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Conflict);
        result.ErrorMessages.Should().ContainSingle("Email test@gmail.com is already in use");
    }

    [Fact]
    public async Task CreateUserCommandHandler_ShouldThrowUserRegistrationException_WhenAUserCouldNotBeCreatedByTheUserManager()
    {
        _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "99", Description = "some error" }));

        var result = await _sut.Handle(new CreateUserCommand { Username = "test", Email = "test@gmail.com", Password = "password12345" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Critial);
        result.ErrorMessages.Should().ContainSingle("Could not create user [test]");
    }
}