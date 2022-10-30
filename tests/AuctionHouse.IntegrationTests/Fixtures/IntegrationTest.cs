using System.Net.Http;
using Xunit;

namespace AuctionHouse.IntegrationTests.Fixtures
{
    public abstract class IntegrationTest : IClassFixture<AuctionHouseWebApplicationFactory>
    {
        protected readonly AuctionHouseWebApplicationFactory _factory;
        protected readonly HttpClient _client;

        protected IntegrationTest(AuctionHouseWebApplicationFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
        }
    }
}
