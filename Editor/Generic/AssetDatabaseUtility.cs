using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    public static class AssetDatabaseUtility
    {
        public static void ImportAsset(string path, ImportAssetOptions options = ImportAssetOptions.Default)
        {
            AssetDatabase.ImportAsset(path, options);
        }

        public static void EnsureDefaultDirectoriesExist()
        {
            string assetsRoot = Path.Combine(Constants.AssetsDirectoryName, Constants.RootDirectoryName);
            string editorRoot = Path.Combine(assetsRoot, Constants.EditorDirectoryName);
            string buildActionsRoot = Path.Combine(editorRoot, Constants.BuildActionsDirectoryName);
            string settingsRoot = Path.Combine(assetsRoot, Constants.SettingsDirectoryName);

            CreateFolder(assetsRoot, Constants.AssetsDirectoryName, Constants.RootDirectoryName);
            CreateFolder(editorRoot, assetsRoot, Constants.EditorDirectoryName);
            CreateFolder(buildActionsRoot, editorRoot, Constants.BuildActionsDirectoryName);
            CreateFolder(settingsRoot, assetsRoot, Constants.SettingsDirectoryName);
        }

        private static void CreateFolder(string path, string parentFolderName, string folderName)
        {
            if (!Directory.Exists(path))
                AssetDatabase.CreateFolder(parentFolderName, folderName);
        }
    }
}
