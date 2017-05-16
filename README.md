# SuperUnityBuild
> A powerful automation tool for quickly and easily generating builds of a game with Unity.

![](https://github.com/Chaser324/unity-build/blob/gh-pages/Unity_2017-01-11_00-38-20.png)

[Unity Forums Thread][unity-thread] | [Documentation Wiki][unitybuild-wiki]

SuperUnityBuild is a Unity utility that automates the process of generating builds of your game. It's easy and quick enough to use on small games, but it's also powerful and extensible enough to be extremely useful on larger projects. The key to this flexibility lies in SuperUnityBuilds configurable degrees of granularity and its "BuildActions" framework which allows additional operations to be added into the build process.

Features:
* **Manage and Build Multiple Versions** - If you're targetting more than one platform or distribution storefront, the build process can quickly become very cumbersome with Unity's built in tools. SuperUnityBuild makes it easy to manage a wide array of build configurations targetting any number of versions, platforms, architectures, or distribution methods.
* **One-Click Batch Builds** - Easily kick-off batch builds for all or a specific subset of your build configurations.
* **Expanded Build Capability** - SuperUnityBuild overs many features not available in Unity's built in build workflow such as version number generation, and the BuildAction framework provides options for even more expanded build capabilities like automated file copying/moving, creating zip files, and building AssetBundles.
* **Quick Initial Setup** - If all you need is the bare essentials, you can be up and running in minutes. SuperUnityBuild scales to fit your project's specific needs whether it be large or small.
* **Highly Extensible and Customizable** - SuperUnityBuild's BuildAction framework provides simple hooks for adding in your own additional functionality.
* **Free and Open-Source** - Some similar tools available only on the AssetStore are as much as $50+.




## Basic Usage

Requires Unity 5.0 or higher. Currently supports Windows, OSX, and Linux builds - more platforms soon once they've been tested more thoroughly.

### Install

Either [download the latest released Unity Package][release] and import it into your project OR [download the zip][download] of this repository and extract its contents into your Unity project's Assets directory.

You may also want to download some of the available BuildAction modules in the [Unity-Build-Actions][unitybuild-actions] repository to expand SuperUnityBuild's capabilities.

### Basic Setup

See [Standard Usage](https://github.com/Chaser324/unity-build/wiki/Standard-Usage) in the wiki.




## Customizing and Expanding

### Creating BuildPlatforms

*TODO*

### Creating BuildActions

See [Build Actions](https://github.com/Chaser324/unity-build/wiki/Build-Actions) in the wiki for details.




## Command Line Interface

See [Command Line Interface](https://github.com/Chaser324/unity-build/wiki/Command-Line-Interface) in the wiki.



## Contributing

Bug reports, feature requests, and pull requests are welcome and appreciated.




## Credits

* **Chase Pettit** - [github](https://github.com/Chaser324), [twitter](http://twitter.com/chasepettit)




## License
All code in this repository ([unity-build](https://github.com/Chaser324/unity-build)) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.




[download]: https://github.com/Chaser324/unity-build/archive/master.zip
[release]: https://github.com/Chaser324/unity-build/releases
[unitybuild]: https://github.com/Chaser324/unity-build
[unitybuild-actions]: https://github.com/Chaser324/unity-build-actions
[unitybuild-wiki]: https://github.com/Chaser324/unity-build/wiki/
[unity-thread]: https://forum.unity3d.com/threads/super-unity-build-automated-build-tool-and-framework.471114/
