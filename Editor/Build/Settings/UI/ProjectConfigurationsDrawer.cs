using System;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
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
                    BuildArchitecture arch;
                    BuildDistribution dist;

                    bool parseSuccess = BuildSettings.projectConfigurations.ParseKeychain(selectedKeyChain.stringValue, out releaseType, out platform, out arch, out dist);

                    if (parseSuccess)
                    {
                        string defines = BuildProject.GenerateDefaultDefines(releaseType, platform, arch, dist);

                        EditorGUILayout.LabelField("Misc Info", UnityBuildGUIUtility.midHeaderStyle);
                        EditorGUILayout.LabelField("Defines:");
                        EditorGUILayout.LabelField(defines, EditorStyles.wordWrappedLabel);

                        if (releaseType != null)
                        {
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

                        if (arch != null)
                        {
                            EditorGUILayout.LabelField("Architecture", UnityBuildGUIUtility.midHeaderStyle);
                            EditorGUILayout.LabelField("Name:\t\t" + arch.name);
                        }

                        if (dist != null)
                        {
                            EditorGUILayout.LabelField("Distribution", UnityBuildGUIUtility.midHeaderStyle);
                            EditorGUILayout.LabelField("Name:\t\t" + dist.distributionName);
                        }

                        GUILayout.Space(20);
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button("Build", GUILayout.ExpandWidth(true)))
                        {
                            EditorApplication.delayCall += () =>
                                BuildProject.BuildSingle(selectedKeyChain.stringValue);
                        }
                        if (GUILayout.Button("Build and Run", GUILayout.ExpandWidth(true)))
                        {
                            EditorApplication.delayCall += () =>
                                BuildProject.BuildSingle(selectedKeyChain.stringValue, BuildOptions.AutoRunPlayer);
                        }

                        EditorGUI.BeginDisabledGroup(!releaseType.developmentBuild);
                        if (GUILayout.Button("Build and Run w/ Profiler", GUILayout.ExpandWidth(true)))
                        {
                            EditorApplication.delayCall += () =>
                                BuildProject.BuildSingle(selectedKeyChain.stringValue, BuildOptions.AutoRunPlayer | BuildOptions.ConnectWithProfiler);
                        }
                        EditorGUI.EndDisabledGroup();
                        GUI.backgroundColor = defaultBackgroundColor;

                        if (GUILayout.Button("Refresh BuildConstants and Apply Defines", GUILayout.ExpandWidth(true)))
                        {
                            #if UNITY_5_6_OR_NEWER
                                EditorUserBuildSettings.SwitchActiveBuildTarget(platform.targetGroup, arch.target);
                            #else
                                EditorUserBuildSettings.SwitchActiveBuildTarget(arch.target);
                            #endif
                            
                            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
                            string appliedDefines = BuildProject.MergeDefines(currentDefines, defines);
                            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, appliedDefines);

                            BuildConstantsGenerator.Generate(DateTime.Now, BuildSettings.productParameters.lastGeneratedVersion, releaseType, platform, arch, dist);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Could not parse selected configuration. It may no longer be valid due to a changes. Select again.", MessageType.Info);
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

        bool displayButton = (depth >= 2 && (config.childKeys == null || config.childKeys.Length == 0));

        if (treeView.boolValue)
        {
            GUILayout.Space(20 * depth);

            config.enabled = EditorGUILayout.Toggle(config.enabled, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(10));
        }

        if (displayButton)
        {
            string displayText;

            if (treeView.boolValue)
            {
                string[] split = key.Split('/');
                displayText = split[split.Length - 1];
            }
            else
            {
                displayText = key;
                config.enabled = EditorGUILayout.Toggle(config.enabled, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(10));
            }

            if (GUILayout.Button(displayText, UnityBuildGUIUtility.dropdownHeaderStyle))
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