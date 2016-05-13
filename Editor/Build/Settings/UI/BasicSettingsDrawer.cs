using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BasicSettings))]
public class BasicSettingsDrawer : PropertyDrawer
{
    private bool show = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        UnityBuildGUIStyles.DropdownHeader("Basic Settings", ref show);

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIStyles.dropdownContentStyle);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("executableName"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("baseBuildFolder"));
            EditorGUILayout.HelpBox("Valid tokens for build path: $YEAR, $MONTH, $DAY, $TIME, $RELEASE_TYPE, $PLATFORM, $ARCHITECTURE, $VERSION", MessageType.Info);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("buildPath"));
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("scenesInBuild"));
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("copyToBuild"));

            //SerializedProperty openBuildFolderAfterBuild = property.FindPropertyRelative("openBuildFolderAfterBuild");
            //openBuildFolderAfterBuild.boolValue = EditorGUILayout.ToggleLeft("  Open build folder after build", openBuildFolderAfterBuild.boolValue);

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}