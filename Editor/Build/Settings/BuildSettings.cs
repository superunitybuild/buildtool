using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

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

    #region Variables

    public BasicSettings _basicSettings = new BasicSettings();
    public ProductParameters _productParameters = new ProductParameters();
    public BuildReleaseTypeList _releaseTypeList = new BuildReleaseTypeList();
    public BuildPlatformList _platformList = new BuildPlatformList();

    #endregion

    #region Properties

    public static BasicSettings basicSettings
    {
        get
        {
            return Instance._basicSettings;
        }
    }

    public static ProductParameters productParameters
    {
        get
        {
            return Instance._productParameters;
        }
    }

    public static BuildReleaseTypeList releaseTypeList
    {
        get
        {
            return Instance._releaseTypeList;
        }
    }

    public static BuildPlatformList platformList
    {
        get
        {
            return Instance._platformList;
        }
    }

    #endregion
}

}