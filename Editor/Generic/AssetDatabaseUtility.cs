using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    public static class AssetDatabaseUtility
    {
        public static void ImportAsset(string path, ImportAssetOptions options = ImportAssetOptions.Default)
        {
#if UNITY_2021_2_OR_NEWER
            // Unity 2021.2+ fixes a bug in older Unity versions that required calling AssetDatabase.SaveAssets() before ImportAsset()
            // See <https://issuetracker.unity3d.com/issues/duplicating-asset-replaces-it-with-one-of-its-sub-assets-if-the-asset-is-created-in-a-version-before-fix>
            // TODO: Update this when fix is backported
#else
            AssetDatabase.SaveAssets();
#endif

            AssetDatabase.ImportAsset(path, options);
        }

        public static void EnsureDirectoriesExist()
        {
            string assetsRoot = Path.Combine(Constants.AssetsDirectoryName, Constants.RootDirectoryName);
            string buildActionsRoot = Path.Combine(assetsRoot, Constants.BuildActionsDirectoryName);
            string settingsRoot = Path.Combine(assetsRoot, Constants.SettingsDirectoryName);

            CreateFolder(assetsRoot, Constants.AssetsDirectoryName, Constants.RootDirectoryName);
            CreateFolder(buildActionsRoot, assetsRoot, Constants.BuildActionsDirectoryName);
            CreateFolder(settingsRoot, assetsRoot, Constants.SettingsDirectoryName);
        }

        private static void CreateFolder(string path, string parentFolderName, string folderName)
        {
            if (!Directory.Exists(path))
                AssetDatabase.CreateFolder(parentFolderName, folderName);
        }
    }
}
