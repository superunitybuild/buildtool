using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildPlatformList))]
public class BuildPlatformListDrawer : PropertyDrawer
{
    private bool show = true;
    private int index = 0;
    private SerializedProperty list = null;
    private BuildPlatformList platformList = null;
    private List<string> availablePlatformList = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        UnityBuildGUIStyles.DropdownHeader("Build Platforms", ref show);

        if (list == null)
        {
            list = property.FindPropertyRelative("platforms");
            platformList = fieldInfo.GetValue(property.serializedObject.targetObject) as BuildPlatformList;
        }

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIStyles.dropdownContentStyle);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");

                string platformName = platformList.platforms[i].platformName;

                if (platformEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(platformProperty);
                    if (availablePlatformList.Contains(platformName))
                        availablePlatformList.Remove(platformName);
                }
                else if (!availablePlatformList.Contains(platformName))
                {
                    availablePlatformList.Add(platformName);
                }
            }

            index = EditorGUILayout.Popup(index, availablePlatformList.ToArray());
            if (GUILayout.Button("Add Platform"))
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                    string platformName = platformList.platforms[i].platformName;

                    if (availablePlatformList[index] == platformName)
                    {   
                        SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");
                        platformEnabled.boolValue = true;

                        platformProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}