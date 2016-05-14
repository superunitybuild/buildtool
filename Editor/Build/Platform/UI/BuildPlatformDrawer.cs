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
        UnityBuildGUIUtility.DropdownHeader(property.FindPropertyRelative("platformName").stringValue, ref show);
        property.isExpanded = show;

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            SerializedProperty archList = property.FindPropertyRelative("architectures");

            if (archList.arraySize > 1)
            {
                for (int i = 0; i < archList.arraySize; i++)
                {
                    SerializedProperty archProperty = archList.GetArrayElementAtIndex(i);
                    SerializedProperty archName = archProperty.FindPropertyRelative("name");
                    SerializedProperty archEnabled = archProperty.FindPropertyRelative("enabled");

                    archEnabled.boolValue = GUILayout.Toggle(archEnabled.boolValue, archName.stringValue);
                    archProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.MaxWidth(150)))
            {
                property.FindPropertyRelative("enabled").boolValue = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }
}

}