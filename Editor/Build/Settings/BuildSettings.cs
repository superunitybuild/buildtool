using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityBuild
{

[InitializeOnLoad]
public class BuildSettings : BaseSettings
{
    #region Singleton

    private static BuildSettings instance = null;

    public static BuildSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateAsset<BuildSettings>("BuildSettings");
            }

            return instance;
        }
    }

    #endregion

    #region MenuItems

    [MenuItem("Build/Edit Settings", priority = 0)]
    public static void EditSettings()
    {
        Selection.activeObject = Instance;
        EditorApplication.ExecuteMenuItem("Window/Inspector");
    }

    #endregion

    #region Variables

    [Header("Build Settings (Field Info in Tooltips)")]

    // The name of executable file (e.g. mygame.exe, mygame.app)
    [SerializeField]
    [Tooltip("The name of executable file (e.g. mygame.exe, mygame.app)")]
    private string _binName = Application.productName;

    // The base path where builds are output.
    // Path is relative to the Unity project's base folder unless an absolute path is given.
    [SerializeField]
    [Tooltip("The base path where builds are output.")]
    private string _binPath = "bin";

    // A list of scenes (filepaths) to include in the build. The first listed scene will be loaded first.
    [SerializeField]
    [Tooltip("A list of scenes to include in the build. First listed scene will be loaded first. ")]
    private string[] _scenesInBuild = new string[] {
        // @"Assets/Scenes/scene1.unity",
        // @"Assets/Scenes/scene2.unity",
        // ...
    };

    // A list of files/directories to include with the build. 
    // Paths are relative to Unity project's base folder unless an absolute path is given.
    [SerializeField]
    [Tooltip("A list of files/directories to include with the build.")]
    private string[] _copyToBuild = new string[] {
        // @"DirectoryToInclude/",
        // @"FileToInclude.txt",
        // ...
    };

    #endregion

    #region Methods & Properties

    public static string binName
    {
        get { return Instance._binName; }
    }

    public static string binPath
    {
        get { return Instance._binPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
    }

    public static string[] scenesInBuild
    {
        get { return Instance._scenesInBuild; }
    }

    public static string[] copyToBuild
    {
        get { return Instance._copyToBuild; }
    }

    #endregion
}

}