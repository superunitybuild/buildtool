using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildPlatformList))]
public class BuildPlatformListDrawer : PropertyDrawer
{
    private bool show = false;
    private int index = 0;
    private SerializedProperty list = null;
    private BuildPlatformList platformList = null;
    private List<string> availablePlatformList = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader("Build Platforms", ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#Build-Platforms");
        EditorGUILayout.EndHorizontal();

        if (list == null)
        {
            list = property.FindPropertyRelative("platforms");
            platformList = fieldInfo.GetValue(property.serializedObject.targetObject) as BuildPlatformList;
        }

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");

                string platformName = platformList.platforms[i].platformName;

                if (platformEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(platformProperty, GUILayout.MaxHeight(0));
                    if (availablePlatformList.Contains(platformName))
                        availablePlatformList.Remove(platformName);
                }
                else if (!availablePlatformList.Contains(platformName))
                {
                    availablePlatformList.Add(platformName);
                }
            }

            if (availablePlatformList.Count > 0)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                index = EditorGUILayout.Popup(index, availablePlatformList.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
                if (GUILayout.Button("Add Platform", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)))
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

                    index = 0;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}