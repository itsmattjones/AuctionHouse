namespace AuctionHouse.Application.UnitTests.Users.Commands;

using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.UpdateUser;
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

public class UpdateUserCommandHandlerTest
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IDateTime> _dateTimeProvider;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly UpdateUserCommandHandler _sut;

    public UpdateUserCommandHandlerTest()
    {
        _dateTimeProvider = new Mock<IDateTime>();
        _currentUserContext = new Mock<ICurrentUserContext>();

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

        _sut = new UpdateUserCommandHandler(_currentUserContext.Object, _userManager.Object, _dateTimeProvider.Object);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersEmail_WhenTheEmailDoesNotExist()
    {
        var user = new User() { Email = "someone@gmail.com" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        await _sut.Handle(new UpdateUserCommand { Email = "test@gmail.com" }, CancellationToken.None);

        user.Email.Should().Be("test@gmail.com");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldThrowUserUpdateException_WhenTheUpdateContainsAnExistingEmail()
    {
        var user = new User() { Email = "someone@gmail.com" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { Email = "test@gmail.com" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Email test@gmail.com is already in use");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersName_WhenTheUsernameDoesNotExist()
    {
        var user = new User() { UserName = "User" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { Username = "testUser" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.UserName.Should().Be("testUser");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldThrowUserUpdateException_WhenTheUpdateContainsAnExistingUsername()
    {
        var user = new User() { UserName = "User"};

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { Username = "testUser" }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.Invalid);
        result.ErrorMessages.Should().ContainSingle("Username testUser is already in use");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersProfileImageUrl_WhenTheUpdateContainsANewProfileImageUrl()
    {
        var user = new User() { ProfileImageUrl = "http://test.com/image.jpg" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { ProfileImageUrl = "http://test.com/image2.png" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.ProfileImageUrl.Should().Be("http://test.com/image2.png");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersPassword_WhenTheUpdateContainsANewPassword()
    {
        var user = new User() { PasswordHash = "123456789" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.RemovePasswordAsync(It.IsAny<User>())).Callback<User>(x => user.PasswordHash = null);
        _userManager.Setup(x => x.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Callback<User, string>((x, y) => user.PasswordHash = $"hashed{y}");
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { Password = "newPassword!123456" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be("hashednewPassword!123456");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldInvalidateTheRefreshToken_WhenAnUpdateToTheUserIsMade()
    {
        var user = new User() { UserName = "test" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

        var result = await _sut.Handle(new UpdateUserCommand { Username = "test2" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 17));
    }
}