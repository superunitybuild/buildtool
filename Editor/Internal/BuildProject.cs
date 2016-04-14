using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace UnityBuild
{

public class BuildProject
{
    private static BuildSettings settings;
    private static List<BuildPlatform> platforms = new List<BuildPlatform>();

    [MenuItem("Build/Run Build", false, 50)]
    public static void BuildAll()
    {
        if (settings == null)
        {
            Debug.LogError("No BuildSettings found. Please run Edit/Generate BuildSettings.");
            return;
        }

        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].Build();
        }
    }

    public static void RegisterSettings(BuildSettings settings)
    {
        if (BuildProject.settings != null)
            Debug.LogError("Multiple BuildSettings classes. There can be only one!");

        BuildProject.settings = settings;
    }

    public static void RegisterPlatform(BuildPlatform platform)
    {
        platforms.Add(platform);
    }

    public static void PerformBuild(BuildTarget target, string binaryNameFormat, string dataDirNameFormat, string platform)
    {
        StringBuilder binPath =
            new StringBuilder(settings.binPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        binPath.Append(Path.DirectorySeparatorChar + platform + Path.DirectorySeparatorChar);

        string exeName = string.Format(binaryNameFormat, settings.binName);
        string dataDestination = string.Format(dataDirNameFormat, settings.binName);

        // Build player
        FileUtil.DeleteFileOrDirectory(binPath.ToString());
        BuildPipeline.BuildPlayer(settings.scenesInBuild, binPath.ToString() + exeName, target, BuildOptions.None);

        // Copy any other data
        for (int i = 0; i < settings.copyToBuild.Length; i++)
        {
            string item = settings.copyToBuild[i];

            if (Path.HasExtension(item))
            {
                string filename = Path.GetFileName(item);
                FileUtil.CopyFileOrDirectory(item, dataDestination + filename);
            }
            else
            {
                string dirname = Path.GetFileName(item.TrimEnd('/'));
                FileUtil.CopyFileOrDirectory(item, dataDestination + dirname);
            }
        }
    }
}

}
