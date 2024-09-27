using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private const string BUILD_TARGET = "BUILD_TARGET_";
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

        /// <summary>
        /// Special user function that runs ConfigureEnvironment on the selected build.
        /// Unlike BuildPlayer(), this does not revert previous settings such as PlayerPrefs
        /// and instead directly matches the environment and generates BuildConstants.
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="options"></param>
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
            _ = BuildSettings.projectConfigurations.ParseKeychain(configKey, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildTarget target,
                out BuildScriptingBackend scriptingBackend, out BuildDistribution distribution);
            string constantsFileLocation = BuildSettings.basicSettings.constantsFileLocation;

            // Configure environment
            ConfigureEnvironment(releaseType, platform, target, scriptingBackend, distribution, configureTime, constantsFileLocation);

            // Run pre-build actions that have opted in to configuring the Editor
            BuildAction[] buildActions = BuildSettings.preBuildActions.buildActions.Where(item => item.configureEditor).ToArray();

            PerformBuildActions(
                releaseType,
                platform,
                target,
                scriptingBackend,
                distribution,
                configureTime, ref options, configKey, null, buildActions, "'Configure Editor' Pre-"
            );
        }

        public static string GenerateDefaultDefines(BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution)
        {
            List<string> defines = new();

            if (releaseType != null)
                defines.Add($"{BUILD_TYPE}{releaseType.typeName.SanitizeDefine()}");

            if (platform != null)
                defines.Add($"{BUILD_PLATFORM}{platform.platformName.SanitizeDefine()}");

            if (target != null)
                defines.Add($"{BUILD_TARGET}{target.name.SanitizeDefine()}");

            if (scriptingBackend != null)
                defines.Add($"{BUILD_BACKEND}{scriptingBackend.name.SanitizeDefine()}");

            if (distribution != null)
                defines.Add($"{BUILD_DIST}{distribution.distributionName.SanitizeDefine()}");

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
            List<string> defines = new(defineString.Split(';'));
            List<BuildReleaseType> releaseTypes = new(BuildSettings.releaseTypeList.releaseTypes);

            for (int i = 0; i < defines.Count; i++)
            {
                string testString = defines[i];

                // Check for default define strings.
                if (testString.StartsWith(BUILD_TYPE) ||
                    testString.StartsWith(BUILD_PLATFORM) ||
                    testString.StartsWith(BUILD_TARGET) ||
                    testString.StartsWith(BUILD_BACKEND) ||
                    testString.StartsWith(BUILD_DIST))
                {
                    defines.RemoveAt(i);
                    --i;
                    continue;
                }

                // Check for custom define strings.
                foreach (BuildReleaseType rt in releaseTypes)
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
            string version = productParameters.versionTemplate;

            version = TokensUtility.ResolveBuildNumberToken(version);
            version = TokensUtility.ResolveBuildTimeUtilityTokens(version, buildTime);
            version = TokensUtility.ResolveBuildTimeTokens(version, buildTime);
            version = TokensUtility.ResolveBuildWordTokens(version);

            productParameters.buildVersion = version;
            PlayerSettings.bundleVersion = version;

            // Increment build numbers for supported platforms
            PlayerSettings.Android.bundleVersionCode++;
            PlayerSettings.iOS.buildNumber = $"{int.Parse(PlayerSettings.iOS.buildNumber) + 1}";
            PlayerSettings.macOS.buildNumber = $"{int.Parse(PlayerSettings.macOS.buildNumber) + 1}";

            return version;
        }

        public static string GenerateBuildPath(string prototype, BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime)
        {
            string resolvedPath = TokensUtility.ResolveBuildConfigurationTokens(prototype, releaseType, platform, target, scriptingBackend, distribution, buildTime);
            string buildPath = Path.Combine(BuildSettings.basicSettings.baseBuildFolder, resolvedPath);
            buildPath = Path.GetFullPath(buildPath).TrimEnd('\\').TrimEnd('/');

            return buildPath;
        }

        public static void SetEditorBuildSettingsScenes(BuildReleaseType releaseType)
        {
            // Create EditorBuildSettingsScene instances from release type scene list
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new();

            List<SceneList.Scene> releaseScenes = releaseType.sceneList.releaseScenes;
            for (int i = 0; i < releaseScenes.Count; i++)
            {
                SceneList.Scene thisScene = releaseScenes[i];
                editorBuildSettingsScenes.Add(
                    new EditorBuildSettingsScene(releaseType.sceneList.SceneGUIDToPath(thisScene.fileGUID),
                    thisScene.sceneActive));
            }

            // Set the Build Settings scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        #endregion

        #region Private Methods

        private static void ConfigureEnvironment(BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, string constantsFileLocation)
        {
            // Switch to target build platform
            _ = EditorUserBuildSettings.SwitchActiveBuildTarget(platform.targetGroup, target.type);

            // Apply standalone build subtarget, if necessary
            if (target.isStandalone)
                EditorUserBuildSettings.standaloneBuildSubtarget = (StandaloneBuildSubtarget)target.subtarget;

            // Adjust scripting backend
            PlayerSettings.SetScriptingBackend(platform.targetGroup, scriptingBackend.scriptingImplementation);

            // Apply defines
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string buildDefines = GenerateDefaultDefines(releaseType, platform, target, scriptingBackend, distribution);
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
            BuildConstantsGenerator.Generate(buildTime, constantsFileLocation, BuildSettings.productParameters.buildVersion,
                BuildSettings.productParameters.buildCounter, releaseType, platform, scriptingBackend, target, distribution);

            // Refresh scene list to make sure nothing has been deleted or moved
            releaseType.sceneList.Refresh();
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
                string constantsFileLocation = BuildSettings.basicSettings.constantsFileLocation;
                _ = BuildSettings.projectConfigurations.ParseKeychain(configKey, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildTarget target,
                    out BuildScriptingBackend scriptingBackend, out BuildDistribution distribution);
                bool success = BuildPlayer(notification, releaseType, platform, target, scriptingBackend, distribution, buildTime, options,
                    constantsFileLocation, configKey);

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
            StringBuilder sb = new();

            _ = failCount == 0
                ? sb.AppendFormat("{0} successful build{1}. No failures. ✔️",
                    successCount, successCount > 1 ? "s" : "")
                : successCount == 0
                    ? sb.AppendFormat("No successful builds. {0} failure{1}. ✖️",
                                    failCount, failCount > 1 ? "s" : "")
                    : sb.AppendFormat("{0} successful build{1}. {2} failure{3}.",
                                    successCount, successCount > 1 ? "s" : "",
                                    failCount, failCount > 1 ? "s" : "");

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
                    _ = System.Diagnostics.Process.Start(outputFolder);
                }
                catch
                {
                    Debug.LogWarning("Couldn't open output folder '" + outputFolder + "'");
                }
            }
        }

        private static bool BuildPlayer(string notification, BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target,
            BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, BuildOptions options,
            string constantsFileLocation, string configKey)
        {
            bool success = true;

            if (options == BuildOptions.None)
            {
                options = releaseType.buildOptions;
            }

            // Save current environment settings
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string preBuildCompanyName = PlayerSettings.companyName;
            string preBuildProductName = PlayerSettings.productName;
            string preBuildBundleIdentifier = PlayerSettings.GetApplicationIdentifier(platform.targetGroup);
            ScriptingImplementation preBuildImplementation = PlayerSettings.GetScriptingBackend(platform.targetGroup);

            // Configure environment settings to match the build configuration
            ConfigureEnvironment(releaseType, platform, target, scriptingBackend, distribution, buildTime, constantsFileLocation);

            // Generate build path
            string buildPath = GenerateBuildPath(BuildSettings.basicSettings.buildPath, releaseType, platform, target, scriptingBackend, distribution, buildTime);
            string finalBuildName = releaseType.productName;
            if (!releaseType.syncAppNameWithProduct)
            {
                finalBuildName = releaseType.appBuildName;
            }
            string binName = string.Format(target.binaryNameFormat, finalBuildName.SanitizeFileName());

            // Pre-build actions
            PerformPreBuild(releaseType, platform, target, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);

            // Report build starting
            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification,
                notification, configKey,
                true, null));

            // Ensure settings are synchronized
            AssetDatabase.SaveAssets();

            // Build player
            _ = FileUtil.DeleteFileOrDirectory(buildPath);

            string error = "";
            BuildPlayerOptions playerOptions = new()
            {
                locationPathName = Path.Combine(buildPath, binName),
                options = options,
                scenes = releaseType.sceneList.GetActiveSceneFileList(),
                target = target.type
            };

            // Apply standalone build subtarget, if necessary
            if (target.isStandalone)
                playerOptions.subtarget = target.subtarget;

            BuildReport buildReport = BuildPipeline.BuildPlayer(playerOptions);

            if (buildReport.summary.result == BuildResult.Failed)
            {
                error = buildReport.summary.totalErrors + " occurred.";
            }

            if (!string.IsNullOrEmpty(error))
            {
                success = false;

                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Build Failed:", configKey.ToString() + "\n" + error,
                    true, null));
            }

            // Post-build actions
            PerformPostBuild(releaseType, platform, target, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);

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
                _ = GenerateVersionString(BuildSettings.productParameters, buildTime);
            }

            // Run pre-build actions.
            PerformBuildActions(BuildSettings.preBuildActions.buildActions, "Pre-");
        }

        private static void PerformPreBuild(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildTarget target,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            PerformBuildActions(
                releaseType,
                platform,
                target,
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
            BuildTarget target,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            PerformBuildActions(
                releaseType,
                platform,
                target,
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
            BuildTarget target,
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
                        if (action.filter == null || (action.filter.Evaluate(releaseType, platform, target, distribution, configKey) && action.actionEnabled))
                        {
                            BuildNotificationList.instance.AddNotification(new BuildNotification(
                                BuildNotification.Category.Notification,
                                string.Format("Performing {0}Build Action ({1}/{2}):", notificationLabel, i + 1, buildActions.Length), string.Format("{0}: {1}", action, configKey),
                                true, null));

                            action.PerBuildExecute(releaseType, platform, target, scriptingBackend, distribution, buildTime, ref options, configKey, buildPath);
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
