namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.RevokeToken;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class RevokeTokenCommandHandlerTest
{
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IDateTime> _dateTimeProvider;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly RevokeTokenCommandHandler _sut;

    public RevokeTokenCommandHandlerTest()
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

        _sut = new RevokeTokenCommandHandler(_currentUserContext.Object, _userManager.Object, _dateTimeProvider.Object);
    }

    [Fact]
    public async Task RevokeTokenCommandHandler_ShouldSetRefreshTokenToExpired_WhenCalledForTheCurrentUser()
    {
        var user = new User { RefreshToken = "test", RefreshTokenExpiry = new DateTime(2022, 10, 10) };

        _dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        _userManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);

        var result = await _sut.Handle(new RevokeTokenCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 17));
    }
}