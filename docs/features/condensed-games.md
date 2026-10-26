# Condensed games only

Quality profiles include a **Condensed games only** checkbox. It defaults off. When enabled, only releases explicitly marked Condensed, Game in 40, or CG are eligible. Full broadcasts cannot satisfy the profile or replace a condensed file. Other non-event content, including press conferences, highlights, recaps and All-22, remains subject to identity rejection.

The identity matcher defers the cut preference to the quality evaluator at acquisition entry points. The evaluator applies the profile gate and uses a 45-minute runtime for quality-definition size checks. Existing event/date, quality, custom-format, seeder, and pack ownership rules remain in force.

A condensed release may replace an existing full broadcast even at a lower resolution. After a condensed version is held, normal quality and score cutoffs apply among condensed releases. Acquisition respects disabled upgrades. Manual preference overrides remain explicit.

The profile flag is stored in QualityProfiles. SQLite and PostgreSQL migrations add it with a false default; the existing SQLite legacy-schema safety net also ensures the column. Native profile edits persist it; compatibility profile edits preserve it.

Prowlarr may exclude categoryless indexers such as NZBIndex. Use its Add and Remove Only mode for Sportarr before adding a manual unrestricted Newznab entry through Prowlarr. Full Sync would remove that entry again.

Build and test with the instructions in development/building.md. This patch is based on v4.1.11.1121, commit 0690071049d92b5411364caa060c5b2ff74dfbc2.

Automatic Newznab searches omit category filters only when the indexer has no configured categories and its discovered capabilities explicitly list no categories. RSS uses the same known capability result. Explicit categories and unknown-capability defaults remain intact. This fixes the difference between manual and automatic searches for NZBIndex.

The renamed-league-folder health check resolves the first directory beneath the configured library root. It supports event folders and custom season layouts, still detects a genuine old league directory, and ignores paths outside known roots.

Native System Status displays the numeric upstream version followed by (CG-only.1; Folder-health.1). Numeric compatibility and update versions remain upstream. The deployment pins the exact local image digest and disables WUD watching; both are temporary deviations until a tested upstream build replaces the patches.

## Folder settings for single-file games

Use league and season folders, with event folders and event-type grouping off. Files can sit directly under `NFL/Season 2026/`. Preserve both season/episode notation and the Sportarr event id in filenames. Event thumbnails and metadata use each video filename as their base, so event folders are optional. Use Reorganize Folders on Rename and the rename preview to move existing files through Sportarr.

## Testing

The upstream v4.1.11.1121 integration run passed 6,288 tests with the six existing VolumeComparisonTests excluded; two of that class fail in the original upstream build in the same bind-mounted SDK container. The final display-marker check passed 43 affected tests. PostgreSQL migration metadata is included, but a PostgreSQL deployment has not been tested.
