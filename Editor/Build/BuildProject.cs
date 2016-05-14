using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SuperSystems.UnityBuild
{

public static class BuildProject
{
    #region Public Variables & Auto-Properties

    public static List<BuildPlatform> platforms { get; private set; }
    public static List<BuildAction> preBuildActions { get; private set; }
    public static List<BuildAction> postBuildActions { get; private set; }

    #endregion

    #region MenuItems

    /// <summary>
    /// Build all enabled platforms.
    /// </summary>
    [MenuItem("Build/Run Build", false, 1)]
    public static void BuildAll()
    {
        //if (preBuildActions != null)
        //    preBuildActions.Sort();

        //if (postBuildActions != null)
        //    postBuildActions.Sort();

        //PerformPreBuild();

        //for (int i = 0; i < platforms.Count; i++)
        //{
        //    IBuildPlatform platform = platforms[i];
        //    if (platform.buildEnabled)
        //    {
        //        PerformPreBuild(platform);
        //        platform.Build();
        //        PerformPostBuild(platform);
        //    }
        //}

        //PerformPostBuild();
    }

    /// <summary>
    /// Enable building of all platforms.
    /// </summary>
    [MenuItem("Build/Platforms/Enable All", false, 50)]
    private static void EnableAllPlatforms()
    {
        SetAllBuildPlatforms(true);
    }

    /// <summary>
    /// Disable building of all platforms.
    /// </summary>
    [MenuItem("Build/Platforms/Disable All", false, 50)]
    private static void DisableAllPlatforms()
    {
        SetAllBuildPlatforms(false);
    }

    #endregion

    #region Register Methods

    /// <summary>
    /// Register a platform that can be built.
    /// </summary>
    /// <param name="platform"></param>
    public static void RegisterPlatform(BuildPlatform platform)
    {
        if (platforms == null)
            platforms = new List<BuildPlatform>();

        platforms.Add(platform);
    }

    /// <summary>
    /// Register a pre-build action.
    /// </summary>
    /// <param name="action"></param>
    public static void RegisterPreBuildAction(BuildAction action)
    {
        if (preBuildActions == null)
            preBuildActions = new List<BuildAction>();

        preBuildActions.Add(action);
    }

    /// <summary>
    /// Register a post-build action.
    /// </summary>
    /// <param name="action"></param>
    public static void RegisterPostBuildAction(BuildAction action)
    {
        if (postBuildActions == null)
            postBuildActions = new List<BuildAction>();

        postBuildActions.Add(action);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Perform build of a platform.
    /// </summary>
    /// <param name="platform"></param>
    public static void PerformBuild(BuildPlatform platform)
    {
        //// Build player.
        //Debug.Log("Building " + platform.name);
        //FileUtil.DeleteFileOrDirectory(platform.buildPath);
        //BuildPipeline.BuildPlayer(BuildSettings.basicSettings.scenesInBuild, platform.buildPath + platform.exeName, platform.target, BuildOptions.None);

        //// Copy any other data.
        //for (int i = 0; i < BuildSettings.basicSettings.copyToBuild.Length; i++)
        //{
        //    string item = BuildSettings.basicSettings.copyToBuild[i];

        //    // Log an error if file/directory does not exist.
        //    if (!File.Exists(item) && !Directory.Exists(item))
        //    {
        //        Debug.LogError("Item to copy does not exist: " + item);
        //        continue;
        //    }

        //    // Copy the file/directory.
        //    if (Path.HasExtension(item))
        //    {
        //        string filename = Path.GetFileName(item);
        //        FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + filename);
        //    }
        //    else
        //    {
        //        string dirname = Path.GetFileName(item.TrimEnd('/').TrimEnd('\\'));
        //        FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + dirname);
        //    }
        //}
    }

    public static string GenerateDefaultDefines(BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist)
    {
        List<string> defines = new List<string>();

        if (releaseType != null)
            defines.Add("BUILD_TYPE_" + releaseType.typeName.ToUpper().Replace(" ", ""));

        if (buildPlatform != null)
            defines.Add("BUILD_PLATFORM_" + buildPlatform.platformName.ToUpper().Replace(" ", ""));

        if (arch != null)
            defines.Add("BUILD_ARCH_" + arch.name.ToUpper().Replace(" ", ""));

        if (dist != null)
            defines.Add("BUILD_DIST_" + dist.distributionName.ToUpper().Replace(" ", ""));

        return string.Join(";", defines.ToArray());
    }

    #endregion

    #region Private Methods

    private static void SetAllBuildPlatforms(bool enabled)
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            EditorPrefs.SetBool("buildGame" + platforms[i].platformName, enabled);
        }
    }

    private static void PerformPreBuild()
    {
        if (preBuildActions != null)
        {
            for (int i = 0; i < preBuildActions.Count; i++)
            {
                Debug.Log("Executing PreBuild: " + preBuildActions[i].GetType().Name + " (" + preBuildActions[i].priority + ")");
                preBuildActions[i].Execute();
            }
        }
    }

    private static void PerformPostBuild()
    {
        if (postBuildActions != null)
        {
            for (int i = 0; i < postBuildActions.Count; i++)
            {
                Debug.Log("Executing PostBuild: " + postBuildActions[i].GetType().Name + " (" + postBuildActions[i].priority + ")");
                postBuildActions[i].Execute();
            }
        }
    }

    private static void PerformPreBuild(BuildPlatform platform)
    {
        if (preBuildActions != null)
        {
            for (int i = 0; i < preBuildActions.Count; i++)
            {
                Debug.Log("Executing PreBuild (" + platform.platformName + "): " + preBuildActions[i].GetType().Name + " (" + preBuildActions[i].priority + ")");
                preBuildActions[i].Execute(platform);
            }
        }
    }

    private static void PerformPostBuild(BuildPlatform platform)
    {
        if (postBuildActions != null)
        {
            for (int i = 0; i < postBuildActions.Count; i++)
            {
                Debug.Log("Executing PostBuild (" + platform.platformName + "): " + postBuildActions[i].GetType().Name + " (" + postBuildActions[i].priority + ")");
                postBuildActions[i].Execute(platform);
            }
        }
    }

    #endregion
}

}
