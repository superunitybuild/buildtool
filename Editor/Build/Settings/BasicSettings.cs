using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BasicSettings
{
    // The name of executable file (e.g. mygame.exe, mygame.app)
    [SerializeField]
    [Tooltip("The name of executable file (e.g. mygame.exe, mygame.app)")]
    public string executableName = Application.productName;

    // The base path where builds are output.
    // Path is relative to the Unity project's base folder unless an absolute path is given.
    [SerializeField]
    [Tooltip("The base path where builds are output.")]
    public string baseBuildFolder = "bin";

    [SerializeField]
    public string buildPath = "$YEAR-$MONTH-$DAY-$TIME/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/";
    
    // A list of scenes (filepaths) to include in the build. The first listed scene will be loaded first.
    [SerializeField]
    [Tooltip("A list of scenes to include in the build. First listed scene will be loaded first. ")]
    public string[] scenesInBuild = new string[] {
        // @"Assets/Scenes/scene1.unity",
        // @"Assets/Scenes/scene2.unity",
        // ...
    };

    // A list of files/directories to include with the build. 
    // Paths are relative to Unity project's base folder unless an absolute path is given.
    [SerializeField]
    [Tooltip("A list of files/directories to include with the build.")]
    public string[] copyToBuild = new string[] {
        // @"DirectoryToInclude/",
        // @"FileToInclude.txt",
        // ...
    };
}

}