using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public static class BuildProject
    {
        #region Constants & Enums

        private const string BUILD_TYPE = "BUILD_TYPE_";
        private const string BUILD_PLATFORM = "BUILD_PLATFORM_";
        private const string BUILD_ARCH = "BUILD_ARCH_";
        private const string BUILD_BACKEND = "BUILD_BACKEND_";
        private const string BUILD_DIST = "BUILD_DIST_";

        #endregion

        #region Public Methods

        public static void BuildAll()
        {
            string[] buildConfigs = BuildSettings.projectConfigurations.BuildAllKeychains();
            PerformBuild(buildConfigs);
        }

        public static void BuildSingle(string keyChain, BuildOptions options)
        {
            string[] buildConfigs = new string[] { keyChain };
            PerformBuild(buildConfigs, options);
        }

        public static void ConfigureEditor(string configKey, BuildOptions options = BuildOptions.None)
        {
            DateTime configureTime = DateTime.Now;

            // Clear any old notifications
            BuildNotificationList.instance.RefreshAll();

            // Report Editor environment configuration
            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                "Configuring Editor environment for: ", configKey,
                true, null));

            // Parse build config
            BuildSettings.projectConfigurations.ParseKeychain(configKey, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildArchitecture architecture,
                out BuildScriptingBackend scriptingBackend, out BuildDistribution distribution);

            // Configure environment
            ConfigureEnvironment(releaseType, platform, architecture, scriptingBackend, distribution, configureTime);

            // Run pre-build actions that have opted in to configuring the Editor
            BuildAction[] buildActions = BuildSettings.preBuildActions.buildActions.Where(item => item.configureEditor).ToArray();

            PerformBuildActions(
                releaseType,
                platform,
                architecture,
                scriptingBackend,
                distribution,
                configureTime, ref options, configKey, null, buildActions, "'Configure Editor' Pre-"
            );
        }

        public static string GenerateDefaultDefines(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture arch,
            BuildScriptingBackend scriptingBackend, BuildDistribution dist)
        {
            List<string> defines = new List<string>();

            if (releaseType != null)
                defines.Add($"{BUILD_TYPE}{releaseType.typeName.SanitizeDefine()}");

            if (platform != null)
                defines.Add($"{BUILD_PLATFORM}{platform.platformName.SanitizeDefine()}");

            if (arch != null)
                defines.Add($"{BUILD_ARCH}{arch.name.SanitizeDefine()}");

            if (scriptingBackend != null)
                defines.Add($"{BUILD_BACKEND}{scriptingBackend.name.SanitizeDefine()}");

            if (dist != null)
                defines.Add($"{BUILD_DIST}{dist.distributionName.SanitizeDefine()}");

            if (releaseType != null && !string.IsNullOrEmpty(releaseType.customDefines))
            {
                string[] customDefines = releaseType.customDefines.Split(';', ',');
                for (int i = 0; i < customDefines.Length; i++)
                {
                    defines.Add(customDefines[i].SanitizeDefine());
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
                    testString.StartsWith(BUILD_BACKEND) ||
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
            // Increment build number
            ++productParameters.buildCounter;

            // Build version string
            string prototype = TokensUtility.ResolveBuildNumberToken(productParameters.versionTemplate);
            prototype = TokensUtility.ResolveBuildTimeTokens(prototype, buildTime);

            StringBuilder sb = new StringBuilder(prototype);

            // Regex = (?:\$DAYSSINCE\(")([^"]*)(?:"\))
            Match match = Regex.Match(prototype, "(?:\\$DAYSSINCE\\(\")([^\"]*)(?:\"\\))");
            while (match.Success)
            {
                int daysSince = (DateTime.TryParse(match.Groups[1].Value, out DateTime parsedTime)) ?
                    buildTime.Subtract(parsedTime).Days :
                    0;

                sb.Replace(match.Captures[0].Value, daysSince.ToString());
                match = match.NextMatch();
            }

            ReplaceFromFile(sb, "$NOUN", "nouns.txt");
            ReplaceFromFile(sb, "$ADJECTIVE", "adjectives.txt");

            sb.Replace("$SECONDS", (buildTime.TimeOfDay.TotalSeconds / 15f).ToString("F0"));

            string version = sb.ToString();

            productParameters.buildVersion = version;
            PlayerSettings.bundleVersion = version;

            // Increment build numbers for supported platforms
            PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
            PlayerSettings.iOS.buildNumber = $"{int.Parse(PlayerSettings.iOS.buildNumber) + 1}";
            PlayerSettings.macOS.buildNumber = $"{int.Parse(PlayerSettings.macOS.buildNumber) + 1}";

            return version;
        }

        public static string GenerateBuildPath(string prototype, BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture arch,
            BuildScriptingBackend scriptingBackend, BuildDistribution dist, DateTime buildTime)
        {
            string resolvedPath = TokensUtility.ResolveBuildConfigurationTokens(prototype, releaseType, platform, arch, scriptingBackend, dist, buildTime);
            string buildPath = Path.Combine(BuildSettings.basicSettings.baseBuildFolder, resolvedPath);
            buildPath = Path.GetFullPath(buildPath).TrimEnd('\\').TrimEnd('/');

            return buildPath;
        }

        public static void SetEditorBuildSettingsScenes(BuildReleaseType releaseType)
        {
            // Create EditorBuildSettingsScene instances from release type scene list
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

            var releaseScenes = releaseType.sceneList.releaseScenes;
            for (int i = 0; i < releaseScenes.Count; i++)
            {
                var thisScene = releaseScenes[i];
                editorBuildSettingsScenes.Add(
                    new EditorBuildSettingsScene(releaseType.sceneList.SceneGUIDToPath(thisScene.fileGUID),
                    thisScene.sceneActive));
            }

            // Set the Build Settings scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        #endregion

        #region Private Methods

        private static void ConfigureEnvironment(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime)
        {
            // Switch to target build platform
            EditorUserBuildSettings.SwitchActiveBuildTarget(platform.targetGroup, architecture.target);

            // Adjust scripting backend
            PlayerSettings.SetScriptingBackend(platform.targetGroup, scriptingBackend.scriptingImplementation);

            // Apply defines
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string buildDefines = GenerateDefaultDefines(releaseType, platform, architecture, scriptingBackend, distribution);
            buildDefines = MergeDefines(preBuildDefines, buildDefines);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, buildDefines);

            // Set target settings
            PlayerSettings.companyName = releaseType.companyName;
            PlayerSettings.productName = releaseType.productName;

            // Set bundle info
            PlayerSettings.SetApplicationIdentifier(platform.targetGroup, releaseType.bundleIdentifier);

            // Apply build variant
            platform.ApplyVariant();

            // Generate BuildConstants
            BuildConstantsGenerator.Generate(buildTime, BuildSettings.productParameters.buildVersion, releaseType, platform, architecture, distribution);

            // Refresh scene list to make sure nothing has been deleted or moved
            releaseType.sceneList.Refresh();
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

            PerformPreBuild(out DateTime buildTime);

            for (int i = 0; i < buildConfigs.Length; i++)
            {
                string configKey = buildConfigs[i];

                // Parse build config and perform build.
                string notification = string.Format("Building ({0}/{1}): ", i + 1, buildConfigs.Length);
                BuildSettings.projectConfigurations.ParseKeychain(configKey, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildArchitecture arch,
                    out BuildScriptingBackend scriptingBackend, out BuildDistribution dist);
                bool success = BuildPlayer(notification, releaseType, platform, arch, scriptingBackend, dist, buildTime, options, configKey);

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
            if (BuildSettings.basicSettings.openFolderPostBuild && successCount > 0)
            {
                string outputFolder = Path.GetFullPath(BuildSettings.basicSettings.baseBuildFolder);

                try
                {
                    System.Diagnostics.Process.Start(outputFolder);
                }
                catch
                {
                    Debug.LogWarning("Couldn't open output folder '" + outputFolder + "'");
                }
            }
        }

        private static bool BuildPlayer(string notification, BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, BuildOptions options, string configKey)
        {
            bool success = true;

            if (options == BuildOptions.None)
                options = releaseType.buildOptions;

            // Save current environment settings
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string preBuildCompanyName = PlayerSettings.companyName;
            string preBuildProductName = PlayerSettings.productName;
            string preBuildBundleIdentifier = PlayerSettings.GetApplicationIdentifier(platform.targetGroup);
            ScriptingImplementation preBuildImplementation = PlayerSettings.GetScriptingBackend(platform.targetGroup);

            // Configure environment settings to match the build configuration
            ConfigureEnvironment(releaseType, platform, architecture, scriptingBackend, distribution, buildTime);

            // Generate build path
            string buildPath = GenerateBuildPath(BuildSettings.basicSettings.buildPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime);
            string binName = string.Format(architecture.binaryNameFormat, releaseType.productName.SanitizeFileName());

            // Pre-build actions
            PerformPreBuild(releaseType, platform, architecture, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);

            // Report build starting
            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                notification, configKey,
                true, null));

            // Build player
            FileUtil.DeleteFileOrDirectory(buildPath);

            string error = "";

            BuildReport buildReport = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                locationPathName = Path.Combine(buildPath, binName),
                options = options,
                scenes = releaseType.sceneList.GetActiveSceneFileList(),
                target = architecture.target
            });

            if (buildReport.summary.result == BuildResult.Failed)
                error = buildReport.summary.totalErrors + " occurred.";

            if (!string.IsNullOrEmpty(error))
            {
                success = false;

                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Build Failed:", configKey.ToString() + "\n" + error,
                    true, null));
            }

            // Post-build actions
            PerformPostBuild(releaseType, platform, architecture, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);

            // Restore pre-build environment settings
            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, preBuildDefines);
            PlayerSettings.companyName = preBuildCompanyName;
            PlayerSettings.productName = preBuildProductName;
            PlayerSettings.SetApplicationIdentifier(platform.targetGroup, preBuildBundleIdentifier);
            PlayerSettings.SetScriptingBackend(platform.targetGroup, preBuildImplementation);

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
            PerformBuildActions(BuildSettings.preBuildActions.buildActions, "Pre-");
        }

        private static void PerformPreBuild(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            PerformBuildActions(
                releaseType,
                platform,
                architecture,
                scriptingBackend,
                distribution,
                buildTime, ref options, configKey, buildPath, BuildSettings.preBuildActions.buildActions, "Pre-"
            );
        }

        private static void PerformPostBuild()
        {
            PerformBuildActions(BuildSettings.postBuildActions.buildActions, "Post-");
        }

        private static void PerformPostBuild(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            PerformBuildActions(
                releaseType,
                platform,
                architecture,
                scriptingBackend,
                distribution,
                buildTime, ref options, configKey, buildPath, BuildSettings.postBuildActions.buildActions, "Post-"
            );
        }

        private static void PerformBuildActions(BuildAction[] buildActions, string notificationLabel)
        {
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
                                string.Format("Performing {0}Build Action ({1}/{2}):", notificationLabel, i + 1, buildActions.Length), $"{action}",
                                true, null));

                            action.Execute();
                        }
                        else
                        {
                            BuildNotificationList.instance.AddNotification(new BuildNotification(
                                BuildNotification.Category.Notification,
                                string.Format("Skipping {0}Build Action ({1}/{2}):", notificationLabel, i + 1, buildActions.Length), $"{action}",
                                true, null));
                        }
                    }
                }
            }
        }

        private static void PerformBuildActions(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath, BuildAction[] buildActions, string notificationLabel)
        {
            if (buildActions != null)
            {
                for (int i = 0; i < buildActions.Length; i++)
                {
                    BuildAction action = buildActions[i];

                    // Check if execute method has been overriden.
                    MethodInfo m = action.GetType().GetMethod("PerBuildExecute");

                    if (m.GetBaseDefinition().DeclaringType != m.DeclaringType && action.actionType == BuildAction.ActionType.PerBuild)
                    {
                        // Check build filter and execute if true.
                        if (action.filter == null || action.filter.Evaluate(releaseType, platform, architecture, distribution, configKey) && action.actionEnabled)
                        {
                            BuildNotificationList.instance.AddNotification(new BuildNotification(
                                BuildNotification.Category.Notification,
                                string.Format("Performing {0}Build Action ({1}/{2}):", notificationLabel, i + 1, buildActions.Length), string.Format("{0}: {1}", action, configKey),
                                true, null));

                            action.PerBuildExecute(releaseType, platform, architecture, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);
                        }
                        else
                        {
                            BuildNotificationList.instance.AddNotification(new BuildNotification(
                                BuildNotification.Category.Notification,
                                string.Format("Skipping {0}Build Action ({1}/{2}):", notificationLabel, i + 1, buildActions.Length), string.Format("{0}: {1}", action, configKey),
                                true, null));
                        }
                    }
                }
            }
        }
        #endregion
    }
}
