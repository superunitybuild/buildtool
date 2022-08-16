using System;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CreateAssetMenu(fileName = Constants.DefaultSettingsFileName, menuName = "SuperUnityBuild Settings", order = 1)]
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
                    _instance = CreateAsset<BuildSettings>(Constants.DefaultSettingsFileName);

                return _instance;
            }
            internal set
            {
                _instance = value;
                EditorPrefs.SetString(SettingsPrefsKey, AssetDatabase.GetAssetPath(_instance));
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

        public static BasicSettings basicSettings { get => instance._basicSettings; }
        public static ProductParameters productParameters { get => instance._productParameters; }
        public static BuildReleaseTypeList releaseTypeList { get => instance._releaseTypeList; }
        public static BuildPlatformList platformList { get => instance._platformList; }
        public static ProjectConfigurations projectConfigurations { get => instance._projectConfigurations; }
        public static BuildActionList preBuildActions { get => instance._preBuildActions; }
        public static BuildActionList postBuildActions { get => instance._postBuildActions; }

        #endregion
    }
}
