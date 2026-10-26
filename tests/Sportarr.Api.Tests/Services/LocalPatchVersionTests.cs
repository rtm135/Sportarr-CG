using FluentAssertions;

namespace Sportarr.Api.Tests.Services;

public class LocalPatchVersionTests
{
    [Fact]
    public void SystemStatusVersionIdentifiesBothLocalPatches() =>
        Sportarr.Api.Version.GetDisplayVersion().Should().Be(
            Sportarr.Api.Version.GetFullVersion() + " (CG-only.1; Folder-health.1)");

    [Fact]
    public void CompatibilityVersionRetainsNumericUpstreamIdentity() =>
        Sportarr.Api.Version.GetFullVersion().Should().NotContain("CG-only");
}
