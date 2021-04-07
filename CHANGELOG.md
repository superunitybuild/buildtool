# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.1] - 2021-04-07
### Added
- Added UPM support. [PR #65](https://github.com/Chaser324/unity-build/pull/65/commits/6b90791566a771bf189ed6272d3005b4d1933ca1)
- Added ability to select which BuildSettings to use. [PR #60](https://github.com/Chaser324/unity-build/pull/60)
- Added option to sync version with PlayerSettings. [PR #53](https://github.com/Chaser324/unity-build/pull/53)
- Added support for Android ARM64 architecture. [PR #56](https://github.com/Chaser324/unity-build/pull/56)
- Added support for APK splitting in Android. [PR #58](https://github.com/Chaser324/unity-build/pull/58/commits/30c8959cd670bb68c3cd70a6728644df25055fc5)

### Changed
- Updated to iOS build platform work by [@chloethompson](https://github.com/chloethompson) to support changes. [PR #52](https://github.com/Chaser324/unity-build/pull/52)
- Improve handling of build options [PR #65](https://github.com/Chaser324/unity-build/pull/65/commits/e7cfee053255e5248784a6da96a36e89506ccf9f).

### Removed
- Removed obsolete code in `BuildProject.cs` and `BuildReleaseTypeDrawer.cs`.
- Removed obsolete Linux build architectures on Unity 2019.2+. [PR #50](https://github.com/Chaser324/unity-build/pull/50)

### Fixed
- Fixed typos [PR #59](https://github.com/Chaser324/unity-build/pull/59)
- Fixed Android device type setting being ignored. [PR #58](https://github.com/Chaser324/unity-build/pull/58/commits/15b96e9e9777ef500b6bfa6d9db800a17dab9273)
- Updated BuildActions wiki link. [PR #51](https://github.com/Chaser324/unity-build/pull/51)
- Restored pre-build player settings after build has finished. [PR #49](https://github.com/Chaser324/unity-build/pull/49)
- Fixed UI spacing for Unity 2019.3x [Issue #43](https://github.com/Chaser324/unity-build/issues/43)
- Fixed issue in Linux build name moving `binaryName` to `BuildArchitecture.cs`. [PR #41](https://github.com/Chaser324/unity-build/pull/41)
- Fixed an issue where custom defines were overwrite when build was finished. [Issue #36](https://github.com/Chaser324/unity-build/issues/36)

[Unreleased]: https://github.com/Chaser324/unity-build/compare/v1.0.0-pre.1...HEAD
[1.0.0-pre.1]: https://github.com/Chaser324/unity-build/compare/v0.9.8...v1.0.0-pre.1)
