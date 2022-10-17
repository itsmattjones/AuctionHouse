using AuctionHouse.Infrastructure.Services;
using FluentAssertions;
using System;
using Xunit;

namespace AuctionHouse.Infrastructure.UnitTests.Services
{
    public class MachineDateTimeTests
    {
        [Fact]
        public void Now_ShouldReturnCurrentDateTime_WhenRequested()
        {
            var machineDateTime = new MachineDateTime();

            machineDateTime.Now.Ticks.Should().BeLessThan(DateTime.UtcNow.AddSeconds(1).Ticks)
                .And.BeGreaterThan(DateTime.UtcNow.AddSeconds(-1).Ticks);

            machineDateTime.CurrentYear.Should().Be(DateTime.UtcNow.Year);
            machineDateTime.CurrentMonth.Should().Be(DateTime.UtcNow.Month);
            machineDateTime.CurrentDay.Should().Be(DateTime.UtcNow.Day);
        }
    }
}
