using System;
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

    [SerializeField]
    private BasicSettings _basicSettings = new BasicSettings();
    [SerializeField]
    private ProductParameters _productParameters = new ProductParameters();
    [SerializeField]
    private BuildReleaseTypeList _releaseTypeList = new BuildReleaseTypeList();
    [SerializeField]
    private BuildPlatformList _platformList = new BuildPlatformList();
    [SerializeField]
    private ProjectConfigurations _projectConfigurations = new ProjectConfigurations();
    [SerializeField]
    private BuildActionList _preBuildActions = new BuildActionList();
    [SerializeField]
    private BuildActionList _postBuildActions = new BuildActionList();

    #endregion

    #region Public Methods

    public static void Init()
    {
        if (_instance._preBuildActions == null)
            _instance._preBuildActions = new BuildActionList();

        if (_instance._postBuildActions == null)
            _instance._postBuildActions = new BuildActionList();
    }

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