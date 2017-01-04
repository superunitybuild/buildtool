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

    private static BuildSettings _instance = null;

    public static BuildSettings instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateAsset<BuildSettings>("UnityBuildSettings");
            }

            return _instance;
        }
    }

    #endregion

    #region Variables

    public BasicSettings _basicSettings = new BasicSettings();
    public ProductParameters _productParameters = new ProductParameters();
    public BuildReleaseTypeList _releaseTypeList = new BuildReleaseTypeList();
    public BuildPlatformList _platformList = new BuildPlatformList();
    public ProjectConfigurations _projectConfigurations = new ProjectConfigurations();
    public BuildActionList _preBuildActions = new BuildActionList();
    public BuildActionList _postBuildActions = new BuildActionList();

    #endregion

    #region Properties

    public static BasicSettings basicSettings
    {
        get
        {
            return instance._basicSettings;
        }
    }

    public static ProductParameters productParameters
    {
        get
        {
            return instance._productParameters;
        }
    }

    public static BuildReleaseTypeList releaseTypeList
    {
        get
        {
            return instance._releaseTypeList;
        }
    }

    public static BuildPlatformList platformList
    {
        get
        {
            return instance._platformList;
        }
    }

    public static ProjectConfigurations projectConfigurations
    {
        get
        {
            return instance._projectConfigurations;
        }
    }

    public static BuildActionList preBuildActions
    {
        get
        {
            return instance._preBuildActions;
        }
    }

    public static BuildActionList postBuildActions
    {
        get
        {
            return instance._postBuildActions;
        }
    }

    #endregion
}

}