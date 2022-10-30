using AuctionHouse.Application.Users.Commands.CreateUser;
using AuctionHouse.Application.Users.Commands.LoginUser;
using AuctionHouse.IntegrationTests.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AuctionHouse.IntegrationTests.Controllers
{
    public class UserControllerTests : IntegrationTest
    {
        public UserControllerTests(AuctionHouseWebApplicationFactory fixture)
            :base(fixture) { }

        [Fact]
        public async Task Register_ShouldRegisterANewUser_WhenUserDoesntAlreadyExist()
        {
            var createUserCommand = new CreateUserCommand
            {
                Email = "geoff.bloggs@gmail.com",
                Username = "geoffbloggs",
                Password = "password12345"
            };

            var request = new StringContent(JsonSerializer.Serialize(createUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/user", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("joebloggs", "joe.bloggs1@gmail.com")]
        [InlineData("joebloggs1", "joe.bloggs@gmail.com")]
        public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExist(string username, string email)
        {
            var createUserCommand = new CreateUserCommand
            {
                Email = email,
                Username = username,
                Password = "password12345"
            };

            var request = new StringContent(JsonSerializer.Serialize(createUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/user", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenUserCredentialsAreCorrect()
        {
            var loginUserCommand = new LoginUserCommand
            {
                Email = "joe.bloggs@gmail.com",
                Password = "password12345"
            };

            var request = new StringContent(JsonSerializer.Serialize(loginUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/user/login", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("joe.bloggs@gmail.com", "wrongpassword")]
        [InlineData("wrong.email@gmail.com", "password12345")]
        public async Task Login_ShouldReturnBadRequest_WhenUserCredentialsIncorrect(string email, string password)
        {
            var loginUserCommand = new LoginUserCommand
            {
                Email = email,
                Password = password
            };

            var request = new StringContent(JsonSerializer.Serialize(loginUserCommand), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/user/login", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
