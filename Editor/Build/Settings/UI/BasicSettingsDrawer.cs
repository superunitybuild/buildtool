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
            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("baseBuildFolder"));
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
}

}