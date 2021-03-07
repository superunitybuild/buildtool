# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.9] - 2020-03-07
### Added
- Added support for Android ARM64 architecture.
- Added support for APK splitting in Android.

### Changed
- Updated to iOS build platform work by @chloethompson to support changes.
- Improve handling of build options [PR #42](https://github.com/Chaser324/unity-build/pull/42/commits/e7cfee053255e5248784a6da96a36e89506ccf9f).

### Removed
- Removed obsolete code in BuildProject.cs and BuildReleaseTypeDrawer.cs.
- Removed obsolete Linux build architectures on 2019.2+.

### Fixed
- Fixed an issue where custom defines were overwrite when build was finished.
- Fixed issue in Linux build name moving binaryName to BuildArchitecture.
- Fixed UI spacing for Unity 2019.3x [Issue #43](https://github.com/Chaser324/unity-build/issues/43)
- Restored pre-build player settings after build has finished.
- Fixed Android device type setting being ignored.

[Unreleased]: https://github.com/Chaser324/unity-build/compare/v0.9.9...HEAD
[0.9.9]: https://github.com/Chaser324/unity-build/compare/v0.9.8...v0.9.9)
