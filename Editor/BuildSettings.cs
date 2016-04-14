using UnityEngine;
using UnityEditor;

namespace UnityBuild
{

public abstract class BuildSettings
{
    public abstract string binName { get; }
    public abstract string binPath { get; }
    public abstract string[] scenesInBuild { get; }
    public abstract string[] copyToBuild { get; }

    //// The name of executable file (e.g. mygame.exe, mygame.app)
    //public const string BIN_NAME = "mygame";

    //// The base path where builds are output.
    //// Path is relative to the Unity project's base folder unless an absolute path is given.
    //public const string BIN_PATH = "bin";

    //// A list of scenes to include in the build. The first listed scene will be loaded first.
    //public static string[] scenesInBuild = new string[] {
    //    // "Assets/Scenes/scene1.unity",
    //    // "Assets/Scenes/scene2.unity",
    //    // ...
    //};

    //// A list of files/directories to include with the build. 
    //// Paths are relative to Unity project's base folder unless an absolute path is given.
    //public static string[] copyToBuild = new string[] {
    //    // "DirectoryToInclude/",
    //    // "FileToInclude.txt",
    //    // ...
    //};


}
}