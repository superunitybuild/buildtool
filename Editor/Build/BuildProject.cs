using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace UnityBuild
{

public static class BuildProject
{
    public static BuildSettings settings { get; private set; }
    public static List<BuildPlatform> platforms { get; private set; }
    public static List<BuildAction> preBuildActions { get; private set; }
    public static List<BuildAction> postBuildActions { get; private set; }

    [MenuItem("Build/Run Build", false, 1)]
    public static void BuildAll()
    {
        if (settings == null)
        {
            Debug.LogError("No BuildSettings found. Please run Edit/Generate BuildSettings.");
            return;
        }

        PerformPreBuild();

        for (int i = 0; i < platforms.Count; i++)
        {
            BuildPlatform platform = platforms[i];
            if (platform.buildEnabled)
            {
                PerformPreBuild(platform);
                platform.Build();
                PerformPostBuild(platform);
            }
        }

        PerformPostBuild();
    }

    [MenuItem("Build/Platforms/Enable All", false, 50)]
    private static void EnableAllPlatforms()
    {
        SetAllBuildPlatforms(true);
    }

    [MenuItem("Build/Platforms/Disable All", false, 50)]
    private static void DisableAllPlatforms()
    {
        SetAllBuildPlatforms(false);
    }

    #region Register Methods

    public static void RegisterSettings(BuildSettings settings)
    {
        if (BuildProject.settings != null)
            Debug.LogError("Multiple BuildSettings classes. There can be only one!");

        BuildProject.settings = settings;
    }

    public static void RegisterPlatform(BuildPlatform platform)
    {
        if (platforms == null)
            platforms = new List<BuildPlatform>();

        platforms.Add(platform);
    }

    public static void RegisterPreBuildAction(BuildAction action)
    {
        if (preBuildActions == null)
            preBuildActions = new List<BuildAction>();

        preBuildActions.Add(action);
    }

    public static void RegisterPostBuildAction(BuildAction action)
    {
        if (postBuildActions == null)
            postBuildActions = new List<BuildAction>();

        postBuildActions.Add(action);
    }

    #endregion

    public static void PerformBuild(BuildPlatform platform)
    {
        // Build player
        FileUtil.DeleteFileOrDirectory(platform.buildPath);
        BuildPipeline.BuildPlayer(settings.scenesInBuild, platform.buildPath + platform.exeName, platform.target, BuildOptions.None);

        // Copy any other data
        for (int i = 0; i < settings.copyToBuild.Length; i++)
        {
            string item = settings.copyToBuild[i];

            if (Path.HasExtension(item))
            {
                string filename = Path.GetFileName(item);
                FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + filename);
            }
            else
            {
                string dirname = Path.GetFileName(item.TrimEnd('/'));
                FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + dirname);
            }
        }
    }

    private static void SetAllBuildPlatforms(bool enabled)
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            EditorPrefs.SetBool("buildGame" + platforms[i].name, enabled);
        }
    }

    private static void BuildAssetBundle(string path, BuildTarget target)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, target);
    }

    private static void PerformPreBuild()
    {
        settings.PreBuild();

        if (preBuildActions != null)
        {
            for (int i = 0; i < preBuildActions.Count; i++)
            {
                preBuildActions[i].Execute();
            }
        }
    }

    private static void PerformPostBuild()
    {
        settings.PostBuild();

        if (postBuildActions != null)
        {
            for (int i = 0; i < postBuildActions.Count; i++)
            {
                postBuildActions[i].Execute();
            }
        }
    }

    private static void PerformPreBuild(BuildPlatform platform)
    {
        settings.PreBuild(platform);

        if (preBuildActions != null)
        {
            for (int i = 0; i < preBuildActions.Count; i++)
            {
                preBuildActions[i].Execute(platform);
            }
        }
    }

    private static void PerformPostBuild(BuildPlatform platform)
    {
        settings.PostBuild(platform);

        if (postBuildActions != null)
        {
            for (int i = 0; i < postBuildActions.Count; i++)
            {
                postBuildActions[i].Execute(platform);
            }
        }
    }
}

}
