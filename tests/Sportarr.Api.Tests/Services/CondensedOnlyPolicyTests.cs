using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Sportarr.Api.Helpers;
using Sportarr.Api.Models;
using Sportarr.Api.Services;

namespace Sportarr.Api.Tests.Services;

public class CondensedOnlyPolicyTests
{
    private static ReleaseEvaluator Evaluator() => new(NullLogger<ReleaseEvaluator>.Instance,
        new EventPartDetector(NullLogger<EventPartDetector>.Instance), new CustomFormatMatchCache(NullLogger<CustomFormatMatchCache>.Instance));
    private static ReleaseSearchResult Release(string title) => new()
    { Title = title, Guid = title, DownloadUrl = "http://test/", Indexer = "Test", Seeders = 5, Size = 700L * 1024 * 1024 };
    private static QualityProfile Profile(bool condensed = true) => new() { Name = "Test", CondensedOnly = condensed };

    [Theory]
    [InlineData("NFL Panthers vs Eagles Condensed 1080p", true)]
    [InlineData("NFL_Panthers_vs_Eagles_Condensed_1080p.7z", true)]
    [InlineData("NFL Panthers vs Eagles Game in 40 1080p", true)]
    [InlineData("NFL.Panthers.Eagles.1080p.MKV-CG", true)]
    [InlineData("NFL.Panthers.Eagles.CG.7z", true)]
    [InlineData("NFL.Panthers.Eagles.1080p.ACC.2CH", false)]
    [InlineData("NFL.Panthers.Eagles.1080p-CGI", false)]
    [InlineData("NFL Panthers vs Eagles Full Game 1080p", false)]
    [InlineData("NFL Panthers vs Eagles Highlights 1080p", false)]
    public void OnlyExplicitCutLabelsCount(string title, bool expected) => CondensedGamePolicy.IsCondensed(title).Should().Be(expected);

    [Fact]
    public void ProfileRejectsFullBroadcastEvenAtHigherQuality() =>
        Evaluator().EvaluateRelease(Release("NFL Panthers vs Eagles 2160p WEB"), Profile()).Rejections
            .Should().Contain(CondensedGamePolicy.FullGameRejection);

    [Fact]
    public void DefaultProfileStillRejectsCondensedCut() =>
        Evaluator().EvaluateRelease(Release("NFL Panthers vs Eagles Condensed 1080p WEB"), Profile(false)).Approved.Should().BeFalse();

    [Fact]
    public void CondensedSizeUsesFortyFiveMinutesRatherThanFullGame()
    {
        var result = Evaluator().EvaluateRelease(Release("NFL Panthers vs Eagles Condensed 1080p WEB-DL"), Profile(),
            qualityDefinitions: new() { new() { Title = "WEB-1080p", Quality = 15, MinSize = 15, MaxSize = 100 } });
        result.Rejections.Should().NotContain(r => r.Contains("below minimum"));
    }

    [Fact]
    public void CondensedMayReplaceHigherResolutionFullGame()
    {
        var decision = ImportUpgradeRule.Evaluate("1080p HDTV", 0, "NFL Panthers vs Eagles 1080p HDTV",
            "720p HDTV", 100, "NFL Panthers vs Eagles Condensed 720p HDTV", "preferAndUpgrade", Profile());
        decision.IsUpgrade.Should().BeTrue();
        ExistingFileUpgradeGate.RefusalReason(new EventFile { FilePath = "/test.mkv", OriginalTitle = "NFL Panthers vs Eagles 1080p HDTV", Quality = "1080p HDTV" },
            "NFL Panthers vs Eagles Condensed 720p HDTV", "720p HDTV", 100, Profile(), new Config()).Should().BeNull();
    }

    [Fact]
    public void FullGameCannotReplaceCondensedEvenAtHigherResolution()
    {
        ImportUpgradeRule.Evaluate("720p HDTV", 100, "NFL Panthers vs Eagles Condensed 720p HDTV",
            "2160p WEB-DL", 500, "NFL Panthers vs Eagles 2160p WEB-DL", "preferAndUpgrade", Profile()).IsUpgrade.Should().BeFalse();
    }

    [Fact]
    public void DisabledUpgradesRemainDisabled() =>
        ExistingFileUpgradeGate.RefusalReason(new EventFile { FilePath = "/test.mkv", OriginalTitle = "NFL 1080p HDTV", Quality = "1080p HDTV" },
            "NFL Condensed 720p HDTV", "720p HDTV", 100, new QualityProfile { Name = "Test", CondensedOnly = true, UpgradesAllowed = false },
            new Config()).Should().NotBeNull();

    [Theory]
    [InlineData("NFL.2026.09.10.Chiefs.vs.Ravens.Condensed.1080p.WEB.h264-GRP", false)]
    [InlineData("NFL.2026.09.10.Chiefs.vs.Ravens.Condensed.Press.Conference.1080p", true)]
    [InlineData("NFL.2026.09.10.Chiefs.vs.Ravens.Condensed.All-22.1080p", true)]
    [InlineData("NFL.2026.09.10.Chiefs.vs.Ravens.Condensed.Recap.1080p", true)]
    public void OptInDoesNotPermitOtherNonEventContent(string title, bool rejected)
    {
        var svc = new ReleaseMatchingService(NullLogger<ReleaseMatchingService>.Instance,
            new SportsFileNameParser(NullLogger<SportsFileNameParser>.Instance), new EventPartDetector(NullLogger<EventPartDetector>.Instance));
        var evt = new Event { Id = 1, Title = "Kansas City Chiefs vs Baltimore Ravens", Sport = "American Football",
            EventDate = new DateTime(2026,9,10,0,20,0,DateTimeKind.Utc), League = new League { Name = "NFL", Sport = "American Football" } };
        var result = svc.ValidateRelease(Release(title), evt, allowCondensed: true);
        result.Rejections.Any(r => r.Contains("Non-event content")).Should().Be(rejected);
    }

    [Fact]
    public void ActualNzbIndexPanthersReleaseMatchesTheRequestedGame()
    {
        var svc = new ReleaseMatchingService(NullLogger<ReleaseMatchingService>.Instance,
            new SportsFileNameParser(NullLogger<SportsFileNameParser>.Instance), new EventPartDetector(NullLogger<EventPartDetector>.Instance));
        var title = "NFL__NZBSPLIT__ee0fa661f6542ac5b7856401ac79f2cd__NZBSPLIT__NFL_2026_W6_Panthers_vs_Eagles_Condensed_18_10_1080pEN30fps_CBS.7z";
        var evt = new Event { Id = 2841, Title = "Philadelphia Eagles vs Carolina Panthers", Sport = "American Football", Season = "2026", Round = "6",
            EventDate = new DateTime(2026,10,18,17,0,0,DateTimeKind.Utc), HomeTeamName = "Philadelphia Eagles", AwayTeamName = "Carolina Panthers",
            League = new League { Name = "NFL", Sport = "American Football" } };
        var result = svc.ValidateRelease(Release(title), evt, allowCondensed: true);
        result.IsHardRejection.Should().BeFalse(string.Join("; ", result.Rejections));
        result.IsMatch.Should().BeTrue(string.Join("; ", result.Rejections));
    }

    [Fact]
    public void CategorylessSourceDoesNotReceiveInventedTvCategories() =>
        NewznabClient.GetEffectiveCategories(new Indexer { Name = "NZBIndex", Url = "http://test/" }, new TorznabCapabilities())
            .Should().BeEmpty();

    [Fact]
    public void ExplicitCategoriesStillWinOverCapabilityDiscovery() =>
        NewznabClient.GetEffectiveCategories(new Indexer { Name = "Test", Url = "http://test/", Categories = new() { "5060" } }, new TorznabCapabilities())
            .Should().Equal("5060");

    [Fact]
    public void UnknownCapabilitiesRetainConservativeCategoryDefaults() =>
        NewznabClient.GetEffectiveCategories(new Indexer { Name = "Test", Url = "http://test/" })
            .Should().NotBeEmpty();
}
