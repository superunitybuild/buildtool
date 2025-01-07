using System;
using UnityEditor;
using UnityEditor.Callbacks;
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
                {
                    _instance = CreateAsset<BuildSettings>(Constants.DefaultSettingsFileName);
                }

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
        private BasicSettings _basicSettings = new();
        [SerializeField]
        private ProductParameters _productParameters = new();
        [SerializeField]
        private BuildReleaseTypeList _releaseTypeList = new();
        [SerializeField]
        private BuildPlatformList _platformList = new();
        [SerializeField]
        private ProjectConfigurations _projectConfigurations = new();
        [SerializeField]
        private BuildActionList _preBuildActions = new();
        [SerializeField]
        private BuildActionList _postBuildActions = new();

        #endregion

        #region Public Methods

        public static void Init()
        {
            _instance._preBuildActions ??= new BuildActionList();

            _instance._postBuildActions ??= new BuildActionList();
        }

        #endregion

        #region Properties

        public static BasicSettings basicSettings => instance._basicSettings;
        public static ProductParameters productParameters => instance._productParameters;
        public static BuildReleaseTypeList releaseTypeList => instance._releaseTypeList;
        public static BuildPlatformList platformList => instance._platformList;
        public static ProjectConfigurations projectConfigurations => instance._projectConfigurations;
        public static BuildActionList preBuildActions => instance._preBuildActions;
        public static BuildActionList postBuildActions => instance._postBuildActions;

        #endregion

        #region Events

        [OnOpenAssetAttribute(1)]
        public static bool HandleOpenAsset(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            if (assetPath == null)
            {
                //Asset did not exist
                return false;
            }
            BuildSettings asset = AssetDatabase.LoadAssetAtPath<BuildSettings>(assetPath);
            if (asset == null)
            {
                //Not the right type
                return false;
            }

            asset.OpenInUnityBuildWindow();

            return true;
        }

        public void OpenInUnityBuildWindow()
        {
            //Show the window using the same process as pressing the menu button
            UnityBuildWindow.ShowWindow();
            UnityBuildWindow thisWindow = EditorWindow.GetWindow<UnityBuildWindow>();
            if (thisWindow != null)
            {
                //If the window exists

                //Set this as the current BuildSettings to be used
                BuildSettings.instance = this;

                //Tell the window to use the new settings 
                thisWindow.UpdateCurrentBuildSettings();
            }

        }

        #endregion
    }
}
