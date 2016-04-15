# UnityBuild
> An expandable and customizable framework for multiplatform builds with Unity.

The primary goal of this project is to take build scripts that I've already used on some projects and make them slightly more generic, expandable, and customizable. This includes supporting the generation of AssetBundles as well as preparing/uploading builds for services like itch.io and Steam.

## Basic Usage

#### Install
Do one of the following:
* [Download](https://github.com/Chaser324/unity-build/archive/master.zip) this project and copy the `Editor` dirctory into your project's assets directory.
* Make this repository a git submodule within your project's assets directory.

#### Generate BuildSettings
* Generate a BuildSettings file by clicking `Build > Generate > BuildSettings` and selecting an Editor directory within your project.
* Customize the new BuildSettings class to suit your project.

#### Build
* Click `Build > Run Build` to build the project. You can select which platforms to build in the `Build > Platforms` menu.

## Project Contents
This project is designed to be highly modular, so you can delete any parts you don't need for your project. I may break these out into separate GitHub repositories at some point but for now just delete what you don't need.
* `AssetBundles` - AssetBundle building. Delete this directory if you aren't using AssetBundles or have your own AssetBundle workflow.
* `Build` - The core build functionality. Don't delete this directory. If you don't need a particular platform, you can delete that individual class.
* `Upload` - *TODO*

## Customizing and Expanding

#### Creating a new BuildPlatform.
* Generate a BuildPlatform file by clicking `Build > Generate > BuildPlatform` and selecting an Editor directory within your project.
* Set the constant values.

#### Creating Pre/Post BuildActions.
There are two options here:
* If your pre/post build actions are very specific to your project, you may want to just override the `PreBuild` and `PostBuild` actions in your project's BuildSettings class.
* If you want the pre/post build actions to be easier to reuse on new projects, create an editor class that inherits from `PreBuildAction` or `PostBuildAction` and overrides the `Execute` method.

## Command Line Interface

*TODO*

## Contributing
Bug reports, feature requests, and pull requests are welcome and appreciated.

## Credits
* **Chase Pettit** - [github](https://github.com/Chaser324), [twitter](http://twitter.com/chasepettit)

## License
All code in this repository ([unity-build](https://github.com/Chaser324/unity-build)) is made freely available under the MIT license.
