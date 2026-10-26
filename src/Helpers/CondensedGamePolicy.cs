using System.Text.RegularExpressions;
using Sportarr.Api.Models;

namespace Sportarr.Api.Helpers;

/// <summary>Explicit cut identity: audio codec/channels and short file sizes are not evidence of a condensed game.</summary>
public static class CondensedGamePolicy
{
    public const int RuntimeMinutes = 45;
    public const string FullGameRejection = "Condensed games only: release is not explicitly labeled as a condensed game";
    private static readonly Regex Marker = new(
        @"\bcondensed\b|\bgame[\s._-]*(?:in[\s._-]*)?40\b|(?:^|[\s._-])CG(?=$|[\s._-])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

    public static bool IsCondensed(string? title) =>
        !string.IsNullOrWhiteSpace(title) && Marker.IsMatch(title.Replace('_', ' '));

    public static bool ReplacesFullGame(QualityProfile? profile, string? existingTitle, string? newTitle) =>
        profile?.CondensedOnly == true && !string.IsNullOrWhiteSpace(existingTitle) &&
        !IsCondensed(existingTitle) && IsCondensed(newTitle);
}
