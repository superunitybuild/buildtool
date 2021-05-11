# SuperUnityBuild

[![openupm](https://img.shields.io/npm/v/com.github.superunitybuild.buildtool?label=openupm&registry_uri=https://package.openupm.com)][openupm-package]

> A powerful automation tool for quickly and easily generating builds with Unity.

![Logo](https://raw.githubusercontent.com/superunitybuild/buildtool/gh-pages/Cover.png)
![Screenshot](https://raw.githubusercontent.com/superunitybuild/buildtool/gh-pages/Screenshot_v1.0.0.png)

[Unity Forums Thread][unity-forums-thread] | [Documentation Wiki][wiki] | [OpenUPM package][openupm-package]

SuperUnityBuild is a Unity utility that automates the process of generating builds. It's easy and quick enough to use on small apps, but it's also powerful and extensible enough to be extremely useful on larger projects. The key to this flexibility lies in SuperUnityBuild's configurable degrees of granularity and its [BuildActions][buildactions] framework which allows additional operations to be added into the build process.

Features:

-   **Manage and Build Multiple Versions** - If you're targetting more than one platform or distribution storefront, the build process can quickly become very cumbersome with Unity's built in tools. SuperUnityBuild makes it easy to manage a wide array of build configurations targetting any number of versions, platforms, architectures, or distribution methods.
-   **One-Click Batch Builds** - Easily kick-off batch builds for all or a specific subset of your build configurations.
-   **Expanded Build Capability** - SuperUnityBuild offers many features not available in Unity's built in build workflow such as version number generation, and the BuildAction framework provides options for even more expanded build capabilities like automated file copying/moving, creating zip files, and building AssetBundles.
-   **Quick Initial Setup** - If all you need is the bare essentials, you can be up and running in minutes. SuperUnityBuild scales to fit your project's specific needs whether it be large or small.
-   **Highly Extensible and Customizable** - SuperUnityBuild's BuildAction framework provides simple hooks for adding in your own additional functionality.
-   **Free and Open-Source** - Some similar tools available only on the AssetStore are as much as $50+.

## Basic Usage

Requires Unity 2019.1 or higher. Supports building for Windows, macOS, Linux, iOS, Android and WebGL.

### Installation

Official releases of SuperUnityBuild can be installed via [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) from the [OpenUPM](https://openupm.com) package registry. See [https://openupm.com/packages/com.github.superunitybuild.buildtool/][openupm-package] for installation options.

You can also [download the source zip][download] of this repository and extract its contents into your Unity project's `Packages` directory to install SuperUnityBuild as an embedded package.

You may also want to [install](https://github.com/superunitybuild/buildtool#installation) the optional [BuildActions][buildactions] package to expand SuperUnityBuild's capabilities.

### Setup

See [Standard Usage](https://github.com/superunitybuild/buildtool/wiki/Standard-Usage) in the wiki.

## Customizing and Expanding

### Creating BuildActions

See [Build Actions](https://github.com/superunitybuild/buildtool/wiki/Build-Actions) in the wiki for details.

## Command Line Interface

See [Command Line Interface](https://github.com/superunitybuild/buildtool/wiki/Command-Line-Interface) in the wiki.

## Contributing

Bug reports, feature requests, and pull requests are welcome and appreciated.

## Credits

### Creator

-   **Chase Pettit** - [GitHub](https://github.com/Chaser324), [Twitter](http://twitter.com/chasepettit)

### Maintainer

-   **Robin North** - [GitHub](https://github.com/robinnorth)

### Contributors

You can see a complete list of contributors at [https://github.com/superunitybuild/buildtool/graphs/contributors][contributors]

## License

All code in this repository ([buildtool][buildtool]) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.

[download]: https://github.com/superunitybuild/buildtool/archive/master.zip
[contributors]: https://github.com/superunitybuild/buildtool/graphs/contributors
[release]: https://github.com/superunitybuild/buildtool/releases
[buildtool]: https://github.com/superunitybuild/buildtool
[buildactions]: https://github.com/superunitybuild/buildactions
[wiki]: https://github.com/superunitybuild/buildtool/wiki/
[openupm-package]: https://openupm.com/packages/com.github.superunitybuild.buildtool/
[unity-forums-thread]: https://forum.unity3d.com/threads/super-unity-build-automated-build-tool-and-framework.471114/
