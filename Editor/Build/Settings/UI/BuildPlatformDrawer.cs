using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildPlatform))]
public class BuildPlatformDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        bool show = property.isExpanded;
        UnityBuildGUIStyles.DropdownHeader(property.FindPropertyRelative("platformName").stringValue, ref show);
        property.isExpanded = show;

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIStyles.dropdownContentStyle);

            if (GUILayout.Button("Delete"))
            {
                property.FindPropertyRelative("enabled").boolValue = false;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}