using System.Threading;
using System.Threading.Tasks;
using AuctionHouse.Application.Common.Interfaces;
using AuctionHouse.Application.Common.Mappers;
using AuctionHouse.Application.Users.Queries.GetCurrentUser;
using AuctionHouse.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace AuctionHouse.Application.UnitTests.Users.Queries
{
    public class GetCurrentUserQueryTest
    {
        [Fact]
        public async Task GetCurrentUserQueryHandler_ShouldReturnCurrentUser_WhenCalledForCurrentUSer()
        {
            var user = new User { Email = "test@gmail.com", UserName = "test", ProfileImageUrl = "test.com/image.png" };

            var mockCurrentUserContext = new Mock<ICurrentUserContext>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

            mockCurrentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);

            var handler = new GetCurrentUserQueryHandler(mockCurrentUserContext.Object, mapper);
            var response = await handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

            response.Email.Should().Be("test@gmail.com");
            response.Username.Should().Be("test");
            response.ProfileImageUrl.Should().Be("test.com/image.png");
        }
    }
}