using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BasicSettings))]
public class BasicSettingsDrawer : PropertyDrawer
{
    private bool show = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader("Basic Settings", ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#Basic-Settings");
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

            property.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}