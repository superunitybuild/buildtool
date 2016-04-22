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
    #region Public Variables & Auto-Properties

    public static BuildSettings settings { get; private set; }
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
        if (settings == null)
        {
            Debug.LogError("No BuildSettings found. Please run Edit/Generate BuildSettings.");
            return;
        }

        if (preBuildActions != null)
            preBuildActions.Sort();

        if (postBuildActions != null)
            postBuildActions.Sort();

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
    /// Register the project's BuildSettings.
    /// </summary>
    /// <param name="settings"></param>
    public static void RegisterSettings(BuildSettings settings)
    {
        if (BuildProject.settings != null && BuildProject.settings.GetType() != settings.GetType())
            Debug.LogError("Multiple BuildSettings classes. There can be only one!");

        BuildProject.settings = settings;
    }

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
        // Build player.
        Debug.Log("Building " + platform.name);
        FileUtil.DeleteFileOrDirectory(platform.buildPath);
        BuildPipeline.BuildPlayer(settings.scenesInBuild, platform.buildPath + platform.exeName, platform.target, BuildOptions.None);

        // Copy any other data.
        for (int i = 0; i < settings.copyToBuild.Length; i++)
        {
            string item = settings.copyToBuild[i];

            // Log an error if file/directory does not exist.
            if (!File.Exists(item) && !Directory.Exists(item))
            {
                Debug.LogError("Item to copy does not exist: " + item);
                continue;
            }

            // Copy the file/directory.
            if (Path.HasExtension(item))
            {
                string filename = Path.GetFileName(item);
                FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + filename);
            }
            else
            {
                string dirname = Path.GetFileName(item.TrimEnd('/').TrimEnd('\\'));
                FileUtil.CopyFileOrDirectory(item, platform.dataDirectory + dirname);
            }
        }
    }

    #endregion

    #region Private Methods

    private static void SetAllBuildPlatforms(bool enabled)
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            EditorPrefs.SetBool("buildGame" + platforms[i].name, enabled);
        }
    }

    private static void PerformPreBuild()
    {
        settings.PreBuild();

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
        settings.PostBuild();

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
        settings.PreBuild(platform);

        if (preBuildActions != null)
        {
            for (int i = 0; i < preBuildActions.Count; i++)
            {
                Debug.Log("Executing PreBuild (" + platform.name + "): " + preBuildActions[i].GetType().Name + " (" + preBuildActions[i].priority + ")");
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
                Debug.Log("Executing PostBuild (" + platform.name + "): " + postBuildActions[i].GetType().Name + " (" + postBuildActions[i].priority + ")");
                postBuildActions[i].Execute(platform);
            }
        }
    }

    #endregion
}

}
