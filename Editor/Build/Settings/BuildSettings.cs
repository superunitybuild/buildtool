using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[InitializeOnLoad]
[Serializable]
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
                instance = CreateAsset<BuildSettings>("UnityBuildSettings");
            }

            return instance;
        }
    }

    #endregion

    [SerializeField]
    public BasicSettings _basicSettings = new BasicSettings();

    [SerializeField]
    public BuildPlatformList _platformList = new BuildPlatformList();

    #region MenuItems

    //[MenuItem("Build/Edit Settings", priority = 0)]
    //public static void EditSettings()
    //{
    //    Selection.activeObject = Instance;
    //    EditorApplication.ExecuteMenuItem("Window/Inspector");
    //}

    #endregion

    public static BasicSettings basicSettings
    {
        get
        {
            return Instance._basicSettings;
        }
    }
}

}