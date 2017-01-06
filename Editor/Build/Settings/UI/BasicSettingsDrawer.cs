using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BasicSettings))]
public class BasicSettingsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUILayout.BeginHorizontal();

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader("Basic Settings", ref show, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        UnityBuildGUIUtility.HelpButton("Parameter-Details#basic-settings");
        EditorGUILayout.EndHorizontal();

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            GUILayout.Label("Build Path Options", UnityBuildGUIUtility.midHeaderStyle);

            //EditorGUI.BeginDisabledGroup(true);
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("baseBuildFolder"));
            //EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Base Build Folder", property.FindPropertyRelative("baseBuildFolder").stringValue);
            if (GUILayout.Button("...", UnityBuildGUIUtility.helpButtonStyle))
            {
                EditorApplication.delayCall += SetBaseBuildFolder;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(property.FindPropertyRelative("buildPath"));

            GUILayout.Space(20);
            GUILayout.Label("Post-Build Options", UnityBuildGUIUtility.midHeaderStyle);

            SerializedProperty openBuildFolderAfterBuild = property.FindPropertyRelative("openFolderPostBuild");
            openBuildFolderAfterBuild.boolValue = EditorGUILayout.ToggleLeft(" Open output folder after build", openBuildFolderAfterBuild.boolValue);

            if (GUILayout.Button("Open Build Folder", GUILayout.ExpandWidth(true)))
            {
                string path = BuildSettings.basicSettings.baseBuildFolder;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                System.Diagnostics.Process.Start(path);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }

    private void SetBaseBuildFolder()
    {
        string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..")) + Path.DirectorySeparatorChar;
        string directory = EditorUtility.OpenFolderPanel("Choose location for build output", projectPath, "");
        
        // Canceled.
        if (string.IsNullOrEmpty(directory))
        {
            return;
        }

        // Normalize path separators.
        directory = Path.GetFullPath(directory);

        // Check for bad choice.
        if (directory.Contains(Path.GetFullPath(Application.dataPath)))
        {
            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Warning,
                "Build Path in Assets",
                "Putting build output into the Assets directory is likely to cause issues.",
                true, null));
        }

        // If relative to project path, reduce the filepath to just what we need.
        if (directory.Contains(projectPath))
            directory = directory.Replace(projectPath, "");

        // Save setting.
        BuildSettings.basicSettings.baseBuildFolder = directory;
    }
}

}