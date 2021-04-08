# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.1] - 2021-04-08
### Added
- Added UPM support. [PR #65](https://github.com/superunitybuild/buildtool/pull/65/commits/6b90791566a771bf189ed6272d3005b4d1933ca1)
- Added support for setting iOS device type and SDK version. [Issue #61](https://github.com/superunitybuild/buildtool/issues/61)
- Added support for setting Android min SDK version.
- Added ability to select which BuildSettings to use. [PR #60](https://github.com/superunitybuild/buildtool/pull/60)
- Added support for Android ARM64 architecture. [PR #56](https://github.com/superunitybuild/buildtool/pull/56)
- Added support for APK splitting in Android. [PR #58](https://github.com/superunitybuild/buildtool/pull/58/commits/30c8959cd670bb68c3cd70a6728644df25055fc5)
- Added option to sync version with PlayerSettings. [PR #53](https://github.com/superunitybuild/buildtool/pull/53)

### Changed
- Minor UI tweaks to improve layout and better match Editor style.
- Improved default assets management.
- **Breaking change:** Updated namespace.
- Prefill company name and bundle identifier when creating new Release Types.
- Improve handling of build options. [PR #65](https://github.com/superunitybuild/buildtool/pull/65/commits/e7cfee053255e5248784a6da96a36e89506ccf9f).
- Updated to iOS build platform work by [@chloethompson](https://github.com/chloethompson) to support changes. [PR #52](https://github.com/superunitybuild/buildtool/pull/52)

### Removed
- Removed obsolete code in `BuildProject` and `BuildReleaseTypeDrawer`.
- Removed obsolete Linux build architectures on Unity 2019.2+. [PR #50](https://github.com/superunitybuild/buildtool/pull/50)

### Fixed
- `BuildPlatformListDrawer` performance improvement. [Issue #66](https://github.com/superunitybuild/buildtool/issues/66)
- **Breaking change:** Fixed platform instances not calling overridden ApplyVariant methods on build. [Issue #62](https://github.com/superunitybuild/buildtool/issues/62)
- Fixed iOS build variant options not being applied. [Issue #61](https://github.com/superunitybuild/buildtool/issues/61)
- Fixed typos [PR #59](https://github.com/superunitybuild/buildtool/pull/59)
- Fixed Android device type setting being ignored. [PR #58](https://github.com/superunitybuild/buildtool/pull/58/commits/15b96e9e9777ef500b6bfa6d9db800a17dab9273)
- Updated BuildActions wiki link. [PR #51](https://github.com/superunitybuild/buildtool/pull/51)
- Restored pre-build player settings after build has finished. [PR #49](https://github.com/superunitybuild/buildtool/pull/49)
- Fixed UI spacing for Unity 2019.3x [Issue #43](https://github.com/superunitybuild/buildtool/issues/43)
- Fixed issue in Linux build name moving `binaryName` to `BuildArchitecture`. [PR #41](https://github.com/superunitybuild/buildtool/pull/41)
- Fixed an issue where custom defines were overwrite when build was finished. [Issue #36](https://github.com/superunitybuild/buildtool/issues/36)

[Unreleased]: https://github.com/superunitybuild/buildtool/compare/v1.0.0-pre.1...HEAD
[1.0.0-pre.1]: https://github.com/superunitybuild/buildtool/compare/v0.9.8...v1.0.0-pre.1)
