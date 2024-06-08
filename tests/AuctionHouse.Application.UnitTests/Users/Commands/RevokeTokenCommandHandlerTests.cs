namespace AuctionHouse.Application.UnitTests.Users.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.RevokeToken;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

public class RevokeTokenCommandHandlerTest
{
    [Fact]
    public async Task RevokeTokenCommandHandler_ShouldSetRefreshTokenToExpired_WhenCalledForTheCurrentUser()
    {
        var user = new User { RefreshToken = "test", RefreshTokenExpiry = new DateTime(2022, 10, 10) };

        var mockCurrentUserContext = new Mock<ICurrentUserContext>();
        var mockDateTime = new Mock<IDateTime>();
        var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));
        mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
        mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);

        var handler = new RevokeTokenCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
        var result = await handler.Handle(new RevokeTokenCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 17));
    }
}