namespace AuctionHouse.Application.UnitTests.Users.Queries;

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

public class GetCurrentUserQueryTest
{
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly GetCurrentUserQueryHandler _sut;

    public GetCurrentUserQueryTest()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

        _currentUserContext = new Mock<ICurrentUserContext>();

        _sut = new GetCurrentUserQueryHandler(_currentUserContext.Object, mapper);
    }

    [Fact]
    public async Task GetCurrentUserQueryHandler_ShouldReturnCurrentUser_WhenCalledForCurrentUSer()
    {
        var user = new User { Email = "test@gmail.com", UserName = "test", ProfileImageUrl = "test.com/image.png" };

        _currentUserContext.Setup(x => x.GetCurrentUserContext()).ReturnsAsync(user);

        var result = await _sut.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("test@gmail.com");
        result.Value.Username.Should().Be("test");
        result.Value.ProfileImageUrl.Should().Be("test.com/image.png");
    }
}