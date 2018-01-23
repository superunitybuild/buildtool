using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BasicSettings))]
public class BasicSettingsDrawer : PropertyDrawer
{
    private string lastBuildPath = string.Empty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUILayout.BeginHorizontal();

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader("Basic Settings", ref show, false, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        UnityBuildGUIUtility.HelpButton("Parameter-Details#basic-settings");
        EditorGUILayout.EndHorizontal();

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            GUILayout.Label("Build Path Options", UnityBuildGUIUtility.midHeaderStyle);
            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("baseBuildFolder"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("buildPath"));

            GUILayout.Space(20);
            GUILayout.Label("Post-Build Options", UnityBuildGUIUtility.midHeaderStyle);

            SerializedProperty openBuildFolderAfterBuild = property.FindPropertyRelative("openFolderPostBuild");
            openBuildFolderAfterBuild.boolValue = EditorGUILayout.ToggleLeft(" Open output folder after build", openBuildFolderAfterBuild.boolValue);

            string buildPath = Path.GetFullPath(Path.Combine(BuildSettings.basicSettings.baseBuildFolder, BuildSettings.basicSettings.buildPath));
            if (!string.Equals(buildPath, lastBuildPath))
            {
                lastBuildPath = buildPath;

                if (buildPath.Contains(Path.GetFullPath(Application.dataPath)))
                {
                    BuildNotificationList.instance.AddNotification(new BuildNotification(
                        BuildNotification.Category.Warning,
                        "Build Folder in Assets.",
                        "Putting build output in Assets is generally a bad idea.",
                        true, null));
                }
            }

            if (GUILayout.Button("Open Build Folder", GUILayout.ExpandWidth(true)))
            {
                string path = BuildSettings.basicSettings.baseBuildFolder;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                System.Diagnostics.Process.Start(path);
            }

            property.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}