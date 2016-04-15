using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityBuild
{

public sealed class BuildAssetBundles : PostBuildAction
{
    [MenuItem("Build/AssetBundles/Build AssetBundles", false, 50)]
    private static void BuildAll()
    {
        Build();
    }

    [MenuItem("Build/AssetBundles/Clear Cache", false, 51)]
    private static void ClearCache()
    {
        Caching.CleanCache();
    }

    [MenuItem("Build/AssetBundles/Delete Bundles", false, 52)]
    private static void DeleteBundles()
    {
        string path = BuildProject.settings.binPath + Path.DirectorySeparatorChar + "Bundles";
        if (Directory.Exists(path))
            FileUtil.DeleteFileOrDirectory(path);
    }

    public override void Execute()
    {
        Build();
    }

    private static void Build()
    {
        string path = BuildProject.settings.binPath + Path.DirectorySeparatorChar + "Bundles";

        for (int i = 0; i < BuildProject.platforms.Count; i++)
        {
            BuildPlatform platform = BuildProject.platforms[i];

            if (!platform.buildEnabled)
                continue;

            string platformBundlePath = path + Path.DirectorySeparatorChar + platform.name;

            if (!Directory.Exists(platformBundlePath))
                Directory.CreateDirectory(platformBundlePath);

            // Build AssetBundles.
            BuildPipeline.BuildAssetBundles(platformBundlePath, BuildAssetBundleOptions.None, platform.target);

            // Copy AssetBundles to data directory.
            if (Directory.Exists(platformBundlePath))
            {
                Directory.CreateDirectory(platform.dataDirectory + "Bundles/");
                FileUtil.CopyFileOrDirectory(platformBundlePath + "/", platform.dataDirectory + "Bundles/" + platform.name);
            }
        }
    }
}

}