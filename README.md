# Sportarr/CG

An unofficial Sportarr fork focused on condensed football games. It is based on Sportarr **v4.1.11.1121**. The patched default branch is [`cg`](https://github.com/rtm135/Sportarr-CG/tree/cg).

## What changes

- Adds a **Condensed games only** quality-profile option. When enabled, only explicitly marked `Condensed`, `Game in 40`, or `CG` releases qualify; full-game fallback is rejected.
- Allows a condensed release to replace a full broadcast, including when its resolution is lower. Normal quality rules still apply between condensed releases.
- Fixes automatic Newznab searches for sources that explicitly advertise no categories.
- Fixes false league-folder health alerts for nested event layouts.
- Shows local patch identifiers in System Status.

The quality-profile option defaults off. Existing event identity, date, quality, and non-event-content checks remain in force.

## Build and test

This repository provides source only. It does not publish a fork-specific installer or Docker image. To build the `cg` branch, follow the [Sportarr build-from-source guide](https://wiki.sportarr.net/development/building/) and see [the patch notes and limitations](docs/features/condensed-games.md). Back up your configuration and database before testing; prefer a separate test instance.

Verification used .NET 8, Linux ARM64, and SQLite. The upstream v4.1.11.1121 integration run passed 6,288 tests; six existing `VolumeComparisonTests` were excluded because two fail on the unmodified upstream build in the same SDK container. The final patch-version check passed 43 affected tests. PostgreSQL migration metadata is included, but PostgreSQL deployment has not been verified.

## Help test

Open an [issue](https://github.com/rtm135/Sportarr-CG/issues) with reproduction steps, Sportarr version, operating system and architecture, database type, and sanitized logs. Note whether a release was found by automatic or manual search. Do not include API keys, passwords, private tracker details, or copyrighted media.

CG availability depends on your own indexers and subscriptions. This fork does not provide game files or source access.

## Upstream project

Sportarr/CG is an independent community fork of [Sportarr](https://github.com/Sportarr/Sportarr), not an official release. Upstream license, documentation, and project credits remain applicable; see [LICENSE.md](LICENSE.md) and the [Sportarr Wiki](https://wiki.sportarr.net/).
