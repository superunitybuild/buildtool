using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public static class BuildProject
{
    #region Constants & Enums

    private const string BUILD_TYPE = "BUILD_TYPE_";
    private const string BUILD_PLATFORM = "BUILD_PLATFORM_";
    private const string BUILD_ARCH = "BUILD_ARCH_";
    private const string BUILD_DIST = "BUILD_DIST_";

    #endregion

    #region Public Methods

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

    public static string GenerateDefaultDefines(BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist)
    {
        List<string> defines = new List<string>();

        if (releaseType != null)
            defines.Add(BUILD_TYPE + SanitizeCodeString(releaseType.typeName.ToUpper().Replace(" ", "")));

        if (buildPlatform != null)
            defines.Add(BUILD_PLATFORM + SanitizeCodeString(buildPlatform.platformName.ToUpper().Replace(" ", "")));

        if (arch != null)
            defines.Add(BUILD_ARCH + SanitizeCodeString(arch.name.ToUpper().Replace(" ", "")));

        if (dist != null)
            defines.Add(BUILD_DIST + SanitizeCodeString(dist.distributionName.ToUpper().Replace(" ", "")));

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

    public static string MergeDefines(string currentDefines, string unityBuildDefines)
    {
        string stripped = StripBuildDefines(currentDefines);
        string retVal = unityBuildDefines;
        if (stripped.Length > 0)
        {
            retVal = unityBuildDefines + ";" + stripped;
        }

        return retVal;
    }

    public static string StripBuildDefines(string defineString)
    {
        List<string> defines = new List<string>(defineString.Split(';'));
        List<BuildReleaseType> releaseTypes = new List<BuildReleaseType>(BuildSettings.releaseTypeList.releaseTypes);

        for (int i = 0; i < defines.Count; i++)
        {
            string testString = defines[i];

            // Check for default define strings.
            if (testString.StartsWith(BUILD_TYPE) ||
                testString.StartsWith(BUILD_PLATFORM) ||
                testString.StartsWith(BUILD_ARCH) ||
                testString.StartsWith(BUILD_DIST))
                {
                    defines.RemoveAt(i);
                    --i;
                    continue;
                }

            // Check for custom define strings.
            foreach (var rt in releaseTypes)
            {
                if (rt.customDefines.Contains(testString))
                {
                    defines.RemoveAt(i);
                    --i;
                    break;
                }
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
        PlayerSettings.bundleVersion = retVal;

        return retVal;
    }

    public static string GenerateBuildPath(string prototype, BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist, DateTime buildTime)
    {
        string resolvedProto = ResolvePath(prototype, releaseType, buildPlatform, arch, dist, buildTime);
        string buildPath = Path.Combine(BuildSettings.basicSettings.baseBuildFolder, resolvedProto);
        buildPath = Path.GetFullPath(buildPath).TrimEnd('\\').TrimEnd('/');


        return buildPath;
    }

    public static string ResolvePath(string prototype, BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist, DateTime buildTime)
    {
        StringBuilder sb = new StringBuilder(prototype);

        sb.Replace("$YEAR", buildTime.ToString("yyyy"));
        sb.Replace("$MONTH", buildTime.ToString("MM"));
        sb.Replace("$DAY", buildTime.ToString("dd"));
        sb.Replace("$TIME", buildTime.ToString("hhmmss"));

        string archName = arch.name;
        if (buildPlatform.variants != null && buildPlatform.variants.Length > 0)
            archName += "(" + buildPlatform.variantKey + ")";

        sb.Replace("$RELEASE_TYPE", SanitizeFolderName(releaseType.typeName));
        sb.Replace("$PLATFORM", SanitizeFolderName(buildPlatform.platformName));
        sb.Replace("$ARCHITECTURE", SanitizeFolderName(archName));
        sb.Replace("$DISTRIBUTION", SanitizeFolderName(dist != null ? dist.distributionName : BuildConstantsGenerator.NONE));
        sb.Replace("$VERSION", SanitizeFolderName(BuildSettings.productParameters.lastGeneratedVersion));
        sb.Replace("$BUILD", BuildSettings.productParameters.buildCounter.ToString());
        sb.Replace("$PRODUCT_NAME", SanitizeFolderName(releaseType.productName));

        return sb.ToString();
    }

    // TODO: Convert sanitize string methods into extensions.
    public static string SanitizeCodeString(string str)
    {
        str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }
        return str;
    }

    public static string SanitizeFolderName(string folderName)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
        string invalidRegStr = string.Format(@"[{0}]", invalidChars);

        return Regex.Replace(folderName, invalidRegStr, "");
    }

    public static string SanitizeFileName(string fileName)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return Regex.Replace(fileName, invalidRegStr, "_");
    }

    #endregion

    #region Private Methods

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

    private static void PerformBuild(string[] buildConfigs, BuildOptions options = BuildOptions.None)
    {
        int successCount = 0;
        int failCount = 0;

        // Save current script defines, build constants, etc. so we can restore them after build.
        string buildConstantsPath = BuildConstantsGenerator.FindFile();
        string currentBuildConstantsFile = null;
        if (!string.IsNullOrEmpty(buildConstantsPath))
        {
            currentBuildConstantsFile = FileUtil.GetUniqueTempPathInProject();
            File.Copy(buildConstantsPath, currentBuildConstantsFile);
        }

        DateTime buildTime;
        PerformPreBuild(out buildTime);

        for (int i = 0; i < buildConfigs.Length; i++)
        {
            BuildReleaseType releaseType;
            BuildPlatform platform;
            BuildArchitecture arch;
            BuildDistribution dist;
            string configKey = buildConfigs[i];

            // Parse build config and perform build.
            string notification = string.Format("Building ({0}/{1}): ", i + 1, buildConfigs.Length);
            BuildSettings.projectConfigurations.ParseKeychain(configKey, out releaseType, out platform, out arch, out dist);
            bool success = BuildPlayer(notification, releaseType, platform, arch, dist, buildTime, options, configKey);

            if (success)
                ++successCount;
            else
                ++failCount;
        }

        PerformPostBuild();

        // Restore editor status.
        if (!string.IsNullOrEmpty(buildConstantsPath))
        {
            File.Copy(currentBuildConstantsFile, buildConstantsPath, true);
            File.Delete(currentBuildConstantsFile);
        }

        // Report success/failure.
        StringBuilder sb = new StringBuilder();
        if (failCount == 0)
        {
            sb.AppendFormat("{0} successful build{1}. No failures. ✔️",
                successCount, successCount > 1 ? "s" : "");
        }
        else if (successCount == 0)
        {
            sb.AppendFormat("No successful builds. {0} failure{1}. ✖️",
                failCount, failCount > 1 ? "s" : "");
        }
        else
        {
            sb.AppendFormat("{0} successful build{1}. {2} failure{3}.",
                successCount, successCount > 1 ? "s" : "",
                failCount, failCount > 1 ? "s" : "");
        }
        BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                "Build Complete.", sb.ToString(),
                true, null));

        // Open output folder if option is enabled.
        if (BuildSettings.basicSettings.openFolderPostBuild)
        {
            System.Diagnostics.Process.Start(BuildSettings.basicSettings.baseBuildFolder);
        }
    }

    private static bool BuildPlayer(string notification, BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, DateTime buildTime, BuildOptions options, string configKey)
    {
        bool success = true;

        // Get build options.
        if (releaseType.developmentBuild)
            options |= BuildOptions.Development;
        if (releaseType.allowDebugging)
            options |= BuildOptions.AllowDebugging;
        if (releaseType.enableHeadlessMode)
            options |= BuildOptions.EnableHeadlessMode;

        // Generate build path.
        string buildPath = GenerateBuildPath(BuildSettings.basicSettings.buildPath, releaseType, platform, architecture, distribution, buildTime);
        string binName = string.Format(architecture.binaryNameFormat, SanitizeFileName(releaseType.productName));

        // Save current user defines, and then set target defines.
        string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
        string buildDefines = GenerateDefaultDefines(releaseType, platform, architecture, distribution);
        buildDefines = MergeDefines(preBuildDefines, buildDefines);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, buildDefines);
        //Debug.Log("Build Defines: " + buildDefines);

        // Save current player settings, and then set target settings.
        string preBuildCompanyName = PlayerSettings.companyName;
        string preBuildProductName = PlayerSettings.productName;

        PlayerSettings.companyName = releaseType.companyName;
        PlayerSettings.productName = releaseType.productName;

        // Set bundle info.
        // Unfortunately, there's not a good way to do this pre-5.6 that doesn't break building w/ batch mode.
#if UNITY_5_6_OR_NEWER
        string preBuildBundleIdentifier = PlayerSettings.GetApplicationIdentifier(platform.targetGroup);
        PlayerSettings.SetApplicationIdentifier(platform.targetGroup, releaseType.bundleIdentifier);
#endif

        // Apply build variant.
        platform.ApplyVariant();

        // Pre-build actions.
        PerformPreBuild(releaseType, platform, architecture, distribution, buildTime, ref options, configKey, buildPath);

        // Generate BuildConstants.
        BuildConstantsGenerator.Generate(buildTime, BuildSettings.productParameters.lastGeneratedVersion, releaseType, platform, architecture, distribution);

        // Refresh scene list to make sure nothing has been deleted or moved.
        releaseType.sceneList.Refresh();

        // Report build starting.
        BuildNotificationList.instance.AddNotification(new BuildNotification(
            BuildNotification.Category.Notification,
            notification, configKey,
            true, null));

        // Build player.
        FileUtil.DeleteFileOrDirectory(buildPath);

        string error = "";
#if UNITY_2018_1_OR_NEWER
        UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(releaseType.sceneList.GetSceneFileList(), Path.Combine(buildPath, binName), architecture.target, options);
        if (buildReport.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            error = buildReport.summary.totalErrors + " occurred.";
#else
        error = BuildPipeline.BuildPlayer(releaseType.sceneList.GetSceneFileList(), Path.Combine(buildPath, binName), architecture.target, options);
#endif
        if (!string.IsNullOrEmpty(error))
        {
            success = false;

            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "Build Failed:", configKey.ToString() + "\n" + error,
                true, null));
        }

        // Post-build actions.
        PerformPostBuild(releaseType, platform, architecture, distribution, buildTime, ref options, configKey, buildPath);

        // Restore pre-build settings.
        PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, preBuildDefines);
        PlayerSettings.companyName = preBuildCompanyName;
        PlayerSettings.productName = preBuildProductName;

#if UNITY_5_6_OR_NEWER
        PlayerSettings.SetApplicationIdentifier(platform.targetGroup, preBuildBundleIdentifier);
#endif

        return success;
    }

    private static void PerformPreBuild(out DateTime buildTime)
    {
        buildTime = DateTime.Now;

        // Clear any old notifications.
        BuildNotificationList.instance.RefreshAll();

        // Generate version string.
        if (BuildSettings.productParameters.autoGenerate)
        {
            GenerateVersionString(BuildSettings.productParameters, buildTime);
        }

        // Run pre-build actions.
        BuildAction[] buildActions = BuildSettings.preBuildActions.buildActions;
        if (buildActions != null)
        {
            for (int i = 0; i < buildActions.Length; i++)
            {
                BuildAction action = buildActions[i];

                // Check if execute method has been overriden.
                MethodInfo m = action.GetType().GetMethod("Execute");
                if (m.GetBaseDefinition().DeclaringType != m.DeclaringType && action.actionType == BuildAction.ActionType.SingleRun)
                {
                    if (action.actionEnabled)
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Performing Pre-Build Action ({0}/{1}).", i + 1, buildActions.Length), action.actionName,
                            true, null));

                        action.Execute();
                    }
                    else
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Skipping Pre-Build Action ({0}/{1}).", i + 1, buildActions.Length), action.actionName,
                            true, null));
                    }
                }
            }
        }
    }

    private static void PerformPreBuild(
        BuildReleaseType releaseType,
        BuildPlatform platform,
        BuildArchitecture architecture,
        BuildDistribution distribution,
        DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        BuildAction[] buildActions = BuildSettings.preBuildActions.buildActions;
        if (buildActions != null)
        {
            for (int i = 0; i < buildActions.Length; i++)
            {
                BuildAction action = buildActions[i];

                // Check if execute method has been overriden.
                MethodInfo m = action.GetType().GetMethod("PerBuildExecute");
                if (m.GetBaseDefinition().DeclaringType != m.DeclaringType && action.actionType == BuildAction.ActionType.PerPlatform)
                {
                    // Check build filter and execute if true.
                    if (action.filter == null || action.filter.Evaluate(releaseType, platform, architecture, distribution, configKey) && action.actionEnabled)
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Performing Pre-Build Action ({0}/{1}).", i + 1, buildActions.Length), string.Format("{0}: {1}", action.actionName, configKey),
                            true, null));

                        action.PerBuildExecute(releaseType, platform, architecture, distribution, buildTime, ref options, configKey, buildPath);
                    }
                    else
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Skipping Pre-Build Action ({0}/{1}).", i + 1, buildActions.Length), string.Format("{0}: {1}", action.actionName, configKey),
                            true, null));
                    }
                }
            }
        }
    }

    private static void PerformPostBuild()
    {
        // Run post-build actions.
        BuildAction[] buildActions = BuildSettings.postBuildActions.buildActions;
        if (buildActions != null)
        {
            for (int i = 0; i < buildActions.Length; i++)
            {
                BuildAction action = buildActions[i];

                // Check if execute method has been overriden.
                MethodInfo m = action.GetType().GetMethod("Execute");
                if (m.GetBaseDefinition().DeclaringType != m.DeclaringType && action.actionType == BuildAction.ActionType.SingleRun)
                {
                    if (action.actionEnabled)
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Performing Post-Build Action ({0}/{1}).", i + 1, buildActions.Length), action.actionName,
                            true, null));

                        action.Execute();
                    }
                    else
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Skipping Post-Build Action ({0}/{1}).", i + 1, buildActions.Length), action.actionName,
                            true, null));
                    }
                }
            }
        }
    }

    private static void PerformPostBuild(
        BuildReleaseType releaseType,
        BuildPlatform platform,
        BuildArchitecture architecture,
        BuildDistribution distribution,
        DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        BuildAction[] buildActions = BuildSettings.postBuildActions.buildActions;
        if (buildActions != null)
        {
            for (int i = 0; i < buildActions.Length; i++)
            {
                BuildAction action = buildActions[i];

                // Check if execute method has been overriden.
                MethodInfo m = action.GetType().GetMethod("PerBuildExecute");
                if (m.GetBaseDefinition().DeclaringType != m.DeclaringType && action.actionType == BuildAction.ActionType.PerPlatform)
                {
                    // Check build filter and execute if true.
                    if (action.filter == null || action.filter.Evaluate(releaseType, platform, architecture, distribution, configKey) && action.actionEnabled)
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Performing Post-Build Action ({0}/{1}).", i + 1, buildActions.Length), string.Format("{0}: {1}", action.actionName, configKey),
                            true, null));

                        action.PerBuildExecute(releaseType, platform, architecture, distribution, buildTime, ref options, configKey, buildPath);
                    }
                    else
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Skipping Post-Build Action ({0}/{1}).", i + 1, buildActions.Length), string.Format("{0}: {1}", action.actionName, configKey),
                            true, null));
                    }
                }
            }
        }
    }

    #endregion
}

}
