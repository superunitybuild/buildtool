# SuperUnityBuild
> A powerful automation tool for quickly and easily generating builds of a game with Unity.

SuperUnityBuild is a Unity utility that automates the process of generating builds of your game. It's easy and quick enough to use on small games, but it's also powerful and extensible enough to be extremely useful on larger projects. The key to this flexibility lies in SuperUnityBuilds configurable degrees of granularity and its "BuildActions" framework which allows additional operations to be added into the build process.

Features:
* **Free and Open-Source** - Some similar tools on the AssetStore are as much as $50+.

## Basic Usage

*TODO*

## Project Contents

*TODO*

## Customizing and Expanding

#### Creating a new BuildPlatform.

#### Creating BuildActions.
Create an editor class that inherits from `PreBuildAction` or `PostBuildAction` and overrides the `Execute` method.

## Command Line Interface

The command line interface is still in development. Feel free to let me know if there's specific functionality you want from this feature.

## Contributing
Bug reports, feature requests, and pull requests are welcome and appreciated.

## Credits
* **Chase Pettit** - [github](https://github.com/Chaser324), [twitter](http://twitter.com/chasepettit)

## License
All code in this repository ([unity-build](https://github.com/Chaser324/unity-build)) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.
