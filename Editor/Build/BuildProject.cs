using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public static class BuildProject
{
    #region Public Methods

    private static void PerformPreBuild(out DateTime buildTime)
    {
        buildTime = DateTime.Now;

        BuildNotificationList.instance.RefreshAll();

        if (BuildSettings.productParameters.autoGenerate)
        {
            GenerateVersionString(BuildSettings.productParameters, buildTime);
        }

        // TODO: Run pre-build actions.
    }

    private static void PerformPostBuild()
    {
        // TODO: Run post-build actions.
    }

    public static void BuildAll()
    {
        string[] buildConfigs = BuildSettings.projectConfigurations.BuildAllKeychains();
        PerformBuild(buildConfigs);
    }

    public static void BuildSingle(string keyChain, BuildOptions options = BuildOptions.None)
    {
        string[] buildConfigs = new string[] { keyChain };
        PerformBuild(buildConfigs, options);
    }

    private static void PerformBuild(string[] buildConfigs, BuildOptions options = BuildOptions.None)
    {
        int successCount = 0;
        int failCount = 0;

        DateTime buildTime;
        BuildProject.PerformPreBuild(out buildTime);

        for (int i = 0; i < buildConfigs.Length; i++)
        {
            BuildReleaseType releaseType;
            BuildPlatform platform;
            BuildArchitecture arch;
            BuildDistribution dist;

            BuildSettings.projectConfigurations.ParseKeychain(buildConfigs[i], out releaseType, out platform, out arch, out dist);
            bool success = BuildProject.BuildPlayer(releaseType, platform, arch, dist, buildTime, options);

            if (success)
                ++successCount;
            else
                ++failCount;
        }

        BuildProject.PerformPostBuild();

        StringBuilder sb = new StringBuilder();
        if (failCount == 0)
        {
            sb.AppendFormat("{0} successful build{1}. No failures. ✔️", successCount, successCount > 1 ? "s" : "");
        }
        else if (successCount == 0)
        {
            sb.AppendFormat("No successful builds. {0} failure{1}. ✖️", failCount, failCount > 1 ? "s" : "");
        }
        else
        {
        }
        BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                "Build Complete.", sb.ToString(),
                true, null));

        if (BuildSettings.basicSettings.openFolderPostBuild)
        {
            System.Diagnostics.Process.Start(BuildSettings.basicSettings.baseBuildFolder);
        }
    }

    private static bool BuildPlayer(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, DateTime buildTime, BuildOptions options)
    {
        bool success = true;

        // Get build options.
        if (releaseType.developmentBuild)
            options |= BuildOptions.Development;
        if (releaseType.allowDebugging)
            options |= BuildOptions.AllowDebugging;

        // Generate configuration keychain string.
        StringBuilder configKey = new StringBuilder(string.Format("{0}/{1}/{2}", releaseType.typeName, platform.platformName, architecture.name));
        if (distribution != null)
            configKey.AppendFormat("/{0}", distribution.distributionName);

        // Generate build path.
        string buildPath = GenerateBuildPath(releaseType, platform, architecture, distribution, buildTime);
        string exeName = SanitizeFileName(string.Format(platform.binaryNameFormat, releaseType.productName));

        // TODO: Pre-build actions.

        // Generate BuildConstants.
        BuildConstantsGenerator.Generate(buildTime, BuildSettings.productParameters.lastGeneratedVersion, releaseType, platform, architecture, distribution);

        // Refresh scene list to make sure nothing has been deleted or moved.
        releaseType.sceneList.Refresh();

        // Build player.
        BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                "Building:", configKey.ToString(),
                true, null));

        FileUtil.DeleteFileOrDirectory(buildPath);
        string error = BuildPipeline.BuildPlayer(releaseType.sceneList.GetSceneList(), Path.Combine(buildPath, exeName), architecture.target, options);

        if (!string.IsNullOrEmpty(error))
        {
            success = false;

            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "Build Failed:", configKey.ToString() + "\n" + error,
                true, null));
        }

        // TODO: Post-build actions.

        return success;
    }

    public static string GenerateDefaultDefines(BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist)
    {
        List<string> defines = new List<string>();

        if (releaseType != null)
            defines.Add("BUILD_TYPE_" + SanitizeCodeString(releaseType.typeName.ToUpper().Replace(" ", "")));

        if (buildPlatform != null)
            defines.Add("BUILD_PLATFORM_" + SanitizeCodeString(buildPlatform.platformName.ToUpper().Replace(" ", "")));

        if (arch != null)
            defines.Add("BUILD_ARCH_" + SanitizeCodeString(arch.name.ToUpper().Replace(" ", "")));

        if (dist != null)
            defines.Add("BUILD_DIST_" + SanitizeCodeString(dist.distributionName.ToUpper().Replace(" ", "")));

        if (releaseType != null && !string.IsNullOrEmpty(releaseType.customDefines))
        {
            string[] customDefines = releaseType.customDefines.Split(';', ',');
            for (int i = 0; i < customDefines.Length; i++)
            {
                defines.Add(SanitizeCodeString(customDefines[i]).ToUpper());
            }
        }

        return string.Join(";", defines.ToArray());
    }

    public static string GenerateVersionString(ProductParameters productParameters, DateTime buildTime)
    {
        string prototypeString = productParameters.version;
        StringBuilder sb = new StringBuilder(prototypeString);

        // Regex = (?:\$DAYSSINCE\(")([^"]*)(?:"\))
        Match match = Regex.Match(prototypeString, "(?:\\$DAYSSINCE\\(\")([^\"]*)(?:\"\\))");
        while (match.Success)
        {
            DateTime parsedTime;
            int daysSince = 0;
            if (DateTime.TryParse(match.Groups[1].Value, out parsedTime))
            {
                daysSince = buildTime.Subtract(parsedTime).Days;
            }
            
            sb.Replace(match.Captures[0].Value, daysSince.ToString());
            match = match.NextMatch();
        }

        ReplaceFromFile(sb, "$NOUN", "nouns.txt");
        ReplaceFromFile(sb, "$ADJECTIVE", "adjectives.txt");

        sb.Replace("$SECONDS", (buildTime.TimeOfDay.TotalSeconds / 15f).ToString("F0"));

        sb.Replace("$YEAR", buildTime.ToString("yyyy"));
        sb.Replace("$MONTH", buildTime.ToString("MM"));
        sb.Replace("$DAY", buildTime.ToString("dd"));
        sb.Replace("$TIME", buildTime.ToString("hhmmss"));

        sb.Replace("$BUILD", (++productParameters.buildCounter).ToString());

        string retVal = sb.ToString();
        productParameters.lastGeneratedVersion = retVal;

        return retVal;
    }

    public static string GenerateBuildPath(BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist, DateTime buildTime)
    {
        StringBuilder sb = new StringBuilder(BuildSettings.basicSettings.buildPath);

        sb.Replace("$YEAR", buildTime.ToString("yyyy"));
        sb.Replace("$MONTH", buildTime.ToString("MM"));
        sb.Replace("$DAY", buildTime.ToString("dd"));
        sb.Replace("$TIME", buildTime.ToString("hhmmss"));

        sb.Replace("$RELEASE_TYPE", SanitizeFolderName(releaseType.typeName));
        sb.Replace("$PLATFORM", SanitizeFolderName(buildPlatform.platformName));
        sb.Replace("$ARCHITECTURE", SanitizeFolderName(arch.name));
        sb.Replace("$DISTRIBUTION", SanitizeFolderName(dist != null ? dist.distributionName : BuildConstantsGenerator.NONE));
        sb.Replace("$VERSION", SanitizeFolderName(BuildSettings.productParameters.lastGeneratedVersion));
        sb.Replace("$BUILD", BuildSettings.productParameters.buildCounter.ToString());

        return Path.Combine(BuildSettings.basicSettings.baseBuildFolder, sb.ToString());
    }

    #endregion

    #region Private Methods

    private static string SanitizeCodeString(string str)
    {
        str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }
        return str;
    }

    private static string SanitizeFolderName(string folderName)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
        string invalidRegStr = string.Format(@"[{0}]", invalidChars);

        return Regex.Replace(folderName, invalidRegStr, "");
    }

    private static string SanitizeFileName(string fileName)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return Regex.Replace(fileName, invalidRegStr, "_");
    }

    private static void ReplaceFromFile(StringBuilder sb, string keyString, string filename)
    {
        if (sb.ToString().IndexOf(keyString) > -1)
        {
            string[] fileSearchResults = Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories);
            string filePath = null;
            string desiredFilePath = string.Format("UnityBuild{0}Editor{0}{1}", Path.DirectorySeparatorChar, filename);
            for (int i = 0; i < fileSearchResults.Length; i++)
            {
                if (fileSearchResults[i].EndsWith(desiredFilePath))
                {
                    filePath = fileSearchResults[i];
                    break;
                }
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                int index = sb.ToString().IndexOf(keyString, 0);
                while (index > -1)
                {
                    string noun = lines[UnityEngine.Random.Range(0, lines.Length - 1)].ToUpper();
                    sb.Replace(keyString, noun, index, keyString.Length);
                    index = sb.ToString().IndexOf(keyString, index + 1);
                }
            }
        }
    }
    
    //private static void PerformPreBuild()
    //{
        //if (preBuildActions != null)
        //{
        //    for (int i = 0; i < preBuildActions.Count; i++)
        //    {
        //        Debug.Log("Executing PreBuild: " + preBuildActions[i].GetType().Name + " (" + preBuildActions[i].priority + ")");
        //        preBuildActions[i].Execute();
        //    }
        //}
    //}

    //private static void PerformPostBuild()
    //{
        //if (postBuildActions != null)
        //{
        //    for (int i = 0; i < postBuildActions.Count; i++)
        //    {
        //        Debug.Log("Executing PostBuild: " + postBuildActions[i].GetType().Name + " (" + postBuildActions[i].priority + ")");
        //        postBuildActions[i].Execute();
        //    }
        //}
    //}

    //private static void PerformPreBuild(BuildPlatform platform)
    //{
        //if (preBuildActions != null)
        //{
        //    for (int i = 0; i < preBuildActions.Count; i++)
        //    {
        //        Debug.Log("Executing PreBuild (" + platform.platformName + "): " + preBuildActions[i].GetType().Name + " (" + preBuildActions[i].priority + ")");
        //        preBuildActions[i].Execute(platform);
        //    }
        //}
    //}

    //private static void PerformPostBuild(BuildPlatform platform)
    //{
        //if (postBuildActions != null)
        //{
        //    for (int i = 0; i < postBuildActions.Count; i++)
        //    {
        //        Debug.Log("Executing PostBuild (" + platform.platformName + "): " + postBuildActions[i].GetType().Name + " (" + postBuildActions[i].priority + ")");
        //        postBuildActions[i].Execute(platform);
        //    }
        //}
    //}

    #endregion
}

}
