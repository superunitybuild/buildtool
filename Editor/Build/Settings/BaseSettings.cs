using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public class BaseSettings : ScriptableObject
    {
        protected const string SettingsPrefsKey = "SuperUnityBuild.BuildTool.BuildSettings";

        protected static T CreateAsset<T>(string assetName) where T : BaseSettings
        {
            // Try to load an existing settings asset at the path specified in EditorPrefs, or fallback to a default path
            string assetsRoot = Path.Combine(Constants.AssetsDirectoryName, Constants.RootDirectoryName);
            string settingsRoot = Path.Combine(assetsRoot, Constants.SettingsDirectoryName);
            string defaultAssetPath = Path.Combine(settingsRoot, string.Format("{0}.asset", assetName));
            string prefsAssetPath = EditorPrefs.HasKey(SettingsPrefsKey) ?
                EditorPrefs.GetString(SettingsPrefsKey, defaultAssetPath) :
                defaultAssetPath;
            string assetPath = File.Exists(prefsAssetPath) ? prefsAssetPath : defaultAssetPath;

            T instance = AssetDatabase.LoadAssetAtPath<T>(assetPath) as T;

            if (instance == null)
            {
                Debug.Log($"SuperUnityBuild: Creating settings file: {defaultAssetPath}");
                instance = CreateInstance<T>();
                instance.name = assetName;

                AssetDatabaseUtility.EnsureDirectoriesExist();

                AssetDatabase.CreateAsset(instance, defaultAssetPath);
            }

            return instance;
        }
    }
}
