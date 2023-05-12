using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(ProjectConfigurations))]
    public class ProjectConfigurationsDrawer : PropertyDrawer
    {
        private bool show;

        private SerializedProperty showViewOptions;
        private SerializedProperty showConfigs;
        private SerializedProperty showBuildInfo;
        private SerializedProperty hideDisabled;
        private SerializedProperty treeView;
        private SerializedProperty selectedKeyChain;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            showViewOptions = property.FindPropertyRelative("showViewOptions");
            showConfigs = property.FindPropertyRelative("showConfigs");
            showBuildInfo = property.FindPropertyRelative("showBuildInfo");
            hideDisabled = property.FindPropertyRelative("hideDisabled");
            treeView = property.FindPropertyRelative("treeView");
            selectedKeyChain = property.FindPropertyRelative("selectedKeyChain");

            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginHorizontal();

            show = property.isExpanded;
            UnityBuildGUIUtility.DropdownHeader("Build Configurations", ref show, false, GUILayout.ExpandWidth(true));
            property.isExpanded = show;

            UnityBuildGUIUtility.HelpButton("Parameter-Details#build-configurations");
            EditorGUILayout.EndHorizontal();

            Color defaultBackgroundColor = GUI.backgroundColor;

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                EditorGUILayout.BeginHorizontal();
                show = showViewOptions.isExpanded;
                UnityBuildGUIUtility.DropdownHeader("View Options", ref show, false, GUILayout.ExpandWidth(true));
                showViewOptions.isExpanded = show;
                EditorGUILayout.EndHorizontal();

                if (show)
                {
                    EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                    hideDisabled.boolValue = EditorGUILayout.ToggleLeft("Hide disabled configurations", hideDisabled.boolValue);
                    treeView.boolValue = EditorGUILayout.ToggleLeft("Show full configurations tree", treeView.boolValue);

                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                show = showConfigs.isExpanded;
                UnityBuildGUIUtility.DropdownHeader("Configurations", ref show, false, GUILayout.ExpandWidth(true));
                showConfigs.isExpanded = show;
                EditorGUILayout.EndHorizontal();

                if (show)
                {
                    EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                    if (BuildSettings.projectConfigurations.configSet.Keys.Count > 0)
                    {
                        BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
                        for (int i = 0; i < releaseTypes.Length; i++)
                        {
                            string key = releaseTypes[i].typeName;
                            Configuration config = BuildSettings.projectConfigurations.configSet[key];
                            DisplayConfigTree(key, config, 0);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Configuration info. Please add a Release Type.", MessageType.Error);
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                show = showBuildInfo.isExpanded;
                UnityBuildGUIUtility.DropdownHeader("Build Info", ref show, false, GUILayout.ExpandWidth(true));
                showBuildInfo.isExpanded = show;
                EditorGUILayout.EndHorizontal();

                if (show)
                {
                    EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                    if (string.IsNullOrEmpty(selectedKeyChain.stringValue))
                    {
                        EditorGUILayout.HelpBox("Click a build configuration above in \"Configurations\" to view full details.", MessageType.Info);
                    }
                    else
                    {
                        BuildReleaseType releaseType;
                        BuildPlatform platform;
                        BuildArchitecture architecture;
                        BuildDistribution distribution;
                        BuildScriptingBackend scriptingBackend;
                        BuildOptions buildOptions = BuildOptions.None;

                        bool parseSuccess = BuildSettings.projectConfigurations.ParseKeychain(selectedKeyChain.stringValue, out releaseType, out platform,
                            out architecture, out scriptingBackend, out distribution);

                        if (parseSuccess)
                        {
                            string defines = BuildProject.GenerateDefaultDefines(releaseType, platform, architecture, scriptingBackend, distribution);

                            EditorGUILayout.LabelField("Misc Info", UnityBuildGUIUtility.midHeaderStyle);
                            EditorGUILayout.LabelField("Defines:");
                            EditorGUILayout.LabelField(defines, EditorStyles.wordWrappedLabel);

                            if (releaseType != null)
                            {
                                buildOptions = releaseType.buildOptions;
                                EditorGUILayout.LabelField("Release Type", UnityBuildGUIUtility.midHeaderStyle);
                                EditorGUILayout.LabelField("Type Name:\t" + releaseType.typeName);

                                if (!string.IsNullOrEmpty(releaseType.bundleIdentifier))
                                    EditorGUILayout.LabelField("Bundle Identifier:\t" + releaseType.bundleIdentifier);

                                EditorGUILayout.LabelField("Product Name:\t" + releaseType.productName);
                            }

                            if (platform != null)
                            {
                                EditorGUILayout.LabelField("Platform", UnityBuildGUIUtility.midHeaderStyle);
                                EditorGUILayout.LabelField("Name:\t\t" + platform.platformName);
                            }

                            if (architecture != null)
                            {
                                EditorGUILayout.LabelField("Architecture", UnityBuildGUIUtility.midHeaderStyle);
                                EditorGUILayout.LabelField("Name:\t\t" + architecture.name);
                            }

                            if (distribution != null)
                            {
                                EditorGUILayout.LabelField("Distribution", UnityBuildGUIUtility.midHeaderStyle);
                                EditorGUILayout.LabelField("Name:\t\t" + distribution.distributionName);
                            }

                            if (scriptingBackend != null)
                            {
                                EditorGUILayout.LabelField("Scripting Backend", UnityBuildGUIUtility.midHeaderStyle);
                                EditorGUILayout.LabelField("Name:\t\t" + scriptingBackend.name);
                            }

                            GUILayout.Space(20);
                            GUI.backgroundColor = Color.green;
                            if (GUILayout.Button("Build", GUILayout.ExpandWidth(true)))
                            {
                                EditorApplication.delayCall += () =>
                                    BuildProject.BuildSingle(selectedKeyChain.stringValue, buildOptions);
                            }
                            if (GUILayout.Button("Build and Run", GUILayout.ExpandWidth(true)))
                            {
                                buildOptions |= BuildOptions.AutoRunPlayer;
                                BuildOptions finalBuildOptions = buildOptions;
                                EditorApplication.delayCall += () =>
                                    BuildProject.BuildSingle(selectedKeyChain.stringValue, finalBuildOptions);
                            }

                            EditorGUI.BeginDisabledGroup((buildOptions & BuildOptions.Development) != BuildOptions.Development);
                            if (GUILayout.Button("Build and Run with Profiler", GUILayout.ExpandWidth(true)))
                            {
                                buildOptions |= BuildOptions.AutoRunPlayer;
                                buildOptions |= BuildOptions.ConnectWithProfiler;
                                BuildOptions finalBuildOptions = buildOptions;
                                EditorApplication.delayCall += () =>
                                    BuildProject.BuildSingle(selectedKeyChain.stringValue, finalBuildOptions);
                            }
                            EditorGUI.EndDisabledGroup();
                            GUI.backgroundColor = defaultBackgroundColor;

                            if (GUILayout.Button(new GUIContent("Configure Editor Environment", "Switches platform, refreshes BuildConstants, applies scripting defines and variant settings and sets Build Settings scene list to match the selected build configuration"), GUILayout.ExpandWidth(true)))
                            {
                                // Update Editor environment settings to match selected build configuration
                                BuildProject.ConfigureEditor(selectedKeyChain.stringValue, buildOptions);

                                // Apply scene list
                                BuildProject.SetEditorBuildSettingsScenes(releaseType);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Could not parse selected configuration. It may no longer be valid due to a change. Select again.", MessageType.Info);
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                property.serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }

        private void DisplayConfigTree(string key, Configuration config, int depth, bool enabled = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!enabled);

            bool displayButton = depth >= 2 && (config.childKeys == null || config.childKeys.Length == 0);

            if (treeView.boolValue)
            {
                GUILayout.Space(20 * depth);

                config.enabled = EditorGUILayout.Toggle(config.enabled, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(10));
            }

            if (displayButton)
            {
                string tooltip = key.Replace(",", ", ");
                string text;

                if (treeView.boolValue)
                {
                    string[] split = key.Split('/');
                    text = split[split.Length - 1];
                }
                else
                {
                    text = tooltip;
                    config.enabled = EditorGUILayout.Toggle(config.enabled, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(10));
                }

                if (GUILayout.Button(new GUIContent(text, tooltip), UnityBuildGUIUtility.dropdownHeaderStyle))
                {
                    selectedKeyChain.stringValue = key;
                }
            }
            else if (treeView.boolValue)
            {
                EditorGUILayout.LabelField(key, UnityBuildGUIUtility.midHeaderStyle);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (!treeView.boolValue && displayButton)
            {
                GUILayout.Space(5);
            }

            if (config.childKeys != null && config.childKeys.Length > 0 && (!hideDisabled.boolValue || config.enabled))
            {
                foreach (string childKey in config.childKeys)
                {
                    Configuration childConfig = BuildSettings.projectConfigurations.configSet[childKey];
                    DisplayConfigTree(childKey, childConfig, depth + 1, config.enabled && enabled);
                }
            }
        }
    }
}
