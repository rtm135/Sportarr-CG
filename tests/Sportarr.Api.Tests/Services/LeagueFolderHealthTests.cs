using FluentAssertions;
using Sportarr.Api.Services;

namespace Sportarr.Api.Tests.Services;

public class LeagueFolderHealthTests
{
    [Theory]
    [InlineData("/media/Sports/NFL/Season 2026/Eagles vs Commanders/game.mkv", "NFL")]
    [InlineData("/media/Sports/NFL/2026/Eagles vs Commanders/game.mkv", "NFL")]
    [InlineData("/media/Sports/NFL/Regular Season/Season 2026/Event/game.mkv", "NFL")]
    [InlineData("/media/Sports/NFL/game.mkv", "NFL")]
    [InlineData("/media/Sports/Old NFL/Season 2026/Event/game.mkv", "Old NFL")]
    [InlineData("/media/Sports/game.mkv", null)]
    [InlineData("/media/SportsOther/NFL/game.mkv", null)]
    [InlineData("/elsewhere/NFL/game.mkv", null)]
    public void LeagueFolderIsResolvedRelativeToLibraryRoot(string file, string? expected) =>
        HealthCheckService.LeagueFolderOf(file, new[] { "/media/Sports" }).Should().Be(expected);

    [Fact]
    public void MostSpecificConfiguredRootWins() =>
        HealthCheckService.LeagueFolderOf("/media/Sports/NFL/Event/game.mkv", new[] { "/media", "/media/Sports" }).Should().Be("NFL");

    [Fact]
    public void RootMatchingAcceptsTrailingSeparators() =>
        HealthCheckService.LeagueFolderOf("/media/Sports/NFL/Event/game.mkv", new[] { "/media/Sports/" }).Should().Be("NFL");
}
