# SuperUnityBuild

[![openupm](https://img.shields.io/npm/v/com.github.superunitybuild.buildtool?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.github.superunitybuild.buildtool/)

> A powerful automation tool for quickly and easily generating builds of a game with Unity.

![Screenshot](https://raw.githubusercontent.com/superunitybuild/buildtool/gh-pages/Screenshot_v1.0.0.png)

[Unity Forums Thread][unity-thread] | [Documentation Wiki][unitybuild-wiki]

SuperUnityBuild is a Unity utility that automates the process of generating builds of your game. It's easy and quick enough to use on small games, but it's also powerful and extensible enough to be extremely useful on larger projects. The key to this flexibility lies in SuperUnityBuild's configurable degrees of granularity and its "BuildActions" framework which allows additional operations to be added into the build process.

Features:

-   **Manage and Build Multiple Versions** - If you're targetting more than one platform or distribution storefront, the build process can quickly become very cumbersome with Unity's built in tools. SuperUnityBuild makes it easy to manage a wide array of build configurations targetting any number of versions, platforms, architectures, or distribution methods.
-   **One-Click Batch Builds** - Easily kick-off batch builds for all or a specific subset of your build configurations.
-   **Expanded Build Capability** - SuperUnityBuild offers many features not available in Unity's built in build workflow such as version number generation, and the BuildAction framework provides options for even more expanded build capabilities like automated file copying/moving, creating zip files, and building AssetBundles.
-   **Quick Initial Setup** - If all you need is the bare essentials, you can be up and running in minutes. SuperUnityBuild scales to fit your project's specific needs whether it be large or small.
-   **Highly Extensible and Customizable** - SuperUnityBuild's BuildAction framework provides simple hooks for adding in your own additional functionality.
-   **Free and Open-Source** - Some similar tools available only on the AssetStore are as much as $50+.

## Basic Usage

Requires Unity 2018.1 or higher. Supports building for Windows, OSX, Linux, iOS, Android and WebGL.

### Installation

Official releases of SuperUnityBuild can be installed via [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) from the [OpenUPM](https://openupm.com) package registry. See [https://openupm.com/packages/com.github.superunitybuild.buildtool/](https://openupm.com/packages/com.github.superunitybuild.buildtool/) for installation options.

You can also [download the source zip](https://github.com/superunitybuild/buildactions) of this repository and extract its contents into your Unity project's `Packages` directory to install SuperUnityBuild as an embedded package.

You may also want to download some of the available BuildAction modules in the [BuildActions](https://github.com/superunitybuild/buildactions) repository to expand SuperUnityBuild's capabilities.

### Basic Setup

See [Standard Usage](https://github.com/superunitybuild/buildtool/wiki/Standard-Usage) in the wiki.

## Customizing and Expanding

### Creating BuildPlatforms

_TODO_

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

You can see a complete list of contributors at [https://github.com/superunitybuild/buildtool/graphs/contributors](https://github.com/superunitybuild/buildtool/graphs/contributors)

## License

All code in this repository ([buildtool](https://github.com/superunitybuild/buildtool)) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.

[download]: https://github.com/superunitybuild/buildtool/archive/master.zip
[release]: https://github.com/superunitybuild/buildtool/releases
[unitybuild]: https://github.com/superunitybuild/buildtool
[unitybuild-actions]: https://github.com/superunitybuild/buildactions
[unitybuild-wiki]: https://github.com/superunitybuild/buildtool/wiki/
[unity-thread]: https://forum.unity3d.com/threads/super-unity-build-automated-build-tool-and-framework.471114/
