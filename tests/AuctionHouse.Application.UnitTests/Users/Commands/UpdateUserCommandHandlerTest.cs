using AuctionHouse.Application.Common.Exceptions;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Users.Commands.UpdateUser;
using AuctionHouse.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AuctionHouse.Application.UnitTests.Users.Commands
{
    public class UpdateUserCommandHandlerTest
    {
        [Fact]
        public async Task UpdateUserCommandHandler_ShouldThrowUserUpdateException_WhenTheUpdateContainsAnExistingEmail()
        {
            var user = new User() { Email = "someone@gmail.com" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            var action = async () => await handler.Handle(new UpdateUserCommand { Email = "test@gmail.com" }, CancellationToken.None);

            await action.Should().ThrowAsync<UserUpdateException>().WithMessage("Email test@gmail.com is already in use");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersEmail_WhenTheEmailDoesNotExist()
        {
            var user = new User() { Email = "someone@gmail.com" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            await handler.Handle(new UpdateUserCommand { Email = "test@gmail.com" }, CancellationToken.None);

            user.Email.Should().Be("test@gmail.com");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldThrowUserUpdateException_WhenTheUpdateContainsAnExistingUsername()
        {
            var user = new User() { UserName = "User"};

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            var action = async () => await handler.Handle(new UpdateUserCommand { Username = "testUser" }, CancellationToken.None);

            await action.Should().ThrowAsync<UserUpdateException>().WithMessage("Username testUser is already in use");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersName_WhenTheUsernameDoesNotExist()
        {
            var user = new User() { UserName = "User" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            await handler.Handle(new UpdateUserCommand { Username = "testUser" }, CancellationToken.None);

            user.UserName.Should().Be("testUser");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersPassword_WhenTheUpdateContainsANewPassword()
        {
            var user = new User() { PasswordHash = "123456789" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.RemovePasswordAsync(It.IsAny<User>())).Callback<User>(x => user.PasswordHash = null);
            mockUserManager.Setup(x => x.AddPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).Callback<User, string>((x, y) => user.PasswordHash = $"hashed{y}");
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            await handler.Handle(new UpdateUserCommand { Password = "newPassword!123456" }, CancellationToken.None);

            user.PasswordHash.Should().Be("hashednewPassword!123456");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldUpdateTheUsersProfileImageUrl_WhenTheUpdateContainsANewProfileImageUrl()
        {
            var user = new User() { ProfileImageUrl = "http://test.com/image.jpg" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            await handler.Handle(new UpdateUserCommand { ProfileImageUrl = "http://test.com/image2.png" }, CancellationToken.None);

            user.ProfileImageUrl.Should().Be("http://test.com/image2.png");
        }

        [Fact]
        public async Task UpdateUserCommandHandler_ShouldInvalidateTheRefreshToken_WhenAnUpdateToTheUserIsMade()
        {
            var user = new User() { UserName = "test" };

            var mockDateTime = new Mock<IDateTime>();
            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mockUserManager = new Mock<UserManager<User?>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success).Callback<User>(x => user = x);
            mockDateTime.Setup(x => x.Now).Returns(new DateTime(2022, 10, 17));

            var handler = new UpdateUserCommandHandler(mockCurrentUserContext.Object, mockUserManager.Object, mockDateTime.Object);
            await handler.Handle(new UpdateUserCommand { Username = "test2" }, CancellationToken.None);

            user.RefreshTokenExpiry.Should().Be(new DateTime(2022, 10, 17));
        }
    }
}