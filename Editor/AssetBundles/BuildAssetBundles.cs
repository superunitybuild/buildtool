using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public sealed class BuildAssetBundles : PostBuildAction
{
    #region MenuItems

    [MenuItem("Build/AssetBundles/Build AssetBundles", false, 50)]
    private static void BuildAllAssetBundles()
    {
        BuildAll();
    }

    [MenuItem("Build/AssetBundles/Clear Cache", false, 51)]
    private static void ClearCache()
    {
        Caching.CleanCache();
    }

    [MenuItem("Build/AssetBundles/Delete Bundles", false, 52)]
    private static void DeleteBundles()
    {
        if (Directory.Exists(BuildAssetBundlesSettings.buildPath))
            FileUtil.DeleteFileOrDirectory(BuildAssetBundlesSettings.buildPath);
    }

    #endregion

    #region Public Methods

    public override void Execute(BuildPlatform platform)
    {
        Build(platform);
    }

    #endregion

    #region Private Methods

    private static void BuildAll()
    {
        for (int i = 0; i < BuildProject.platforms.Count; i++)
        {
            BuildPlatform platform = BuildProject.platforms[i];
            Build(platform);
        }
    }

    private static void Build(BuildPlatform platform)
    {
        //if (!platform.buildEnabled)
        //    return;

        //// Path where this platform's AssetBundles will be built.
        //string platformBundlePath = BuildAssetBundlesSettings.buildPath + Path.DirectorySeparatorChar + platform.name;

        //// Create build destination directory if it does not exist.
        //if (!Directory.Exists(platformBundlePath))
        //    Directory.CreateDirectory(platformBundlePath);

        //// Build AssetBundles.
        //BuildPipeline.BuildAssetBundles(platformBundlePath, BuildAssetBundleOptions.None, platform.target);

        //// Copy AssetBundles to platform's data directory.
        //if (BuildAssetBundlesSettings.copyToBuild && Directory.Exists(platformBundlePath))
        //{
        //    string bundleDirectory = platform.dataDirectory + "Bundles" + Path.DirectorySeparatorChar;
        //    string targetPath = bundleDirectory + platform.name;

        //    if (Directory.Exists(bundleDirectory))
        //        FileUtil.DeleteFileOrDirectory(bundleDirectory);

        //    Directory.CreateDirectory(bundleDirectory);
        //    FileUtil.CopyFileOrDirectory(platformBundlePath + Path.DirectorySeparatorChar, targetPath);
        //}
    }

    #endregion
}

}