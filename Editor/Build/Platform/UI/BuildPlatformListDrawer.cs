using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildPlatformList))]
public class BuildPlatformListDrawer : PropertyDrawer
{
    private int index = 0;
    private SerializedProperty list = null;
    private BuildPlatformList platformList = null;
    private List<string> availablePlatformList = new List<string>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader("Build Platforms", ref show, false, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        UnityBuildGUIUtility.HelpButton("Parameter-Details#build-platforms");
        EditorGUILayout.EndHorizontal();

        //if (list == null)
        //{
            platformList = fieldInfo.GetValue(property.serializedObject.targetObject) as BuildPlatformList;
            PopulateList();
            list = property.FindPropertyRelative("platforms");
        //}

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            int enabledCount = 0;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");

                string platformName = platformList.platforms[i].platformName;

                if (platformEnabled.boolValue)
                {
                    ++enabledCount;
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
                if (enabledCount > 0)
                {
                    GUILayout.Space(20);
                }

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

    private void PopulateList()
    {
        Type ti = typeof(BuildPlatform);
        List<BuildPlatform> platforms = new List<BuildPlatform>(platformList.platforms);
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    BuildPlatform instance = (BuildPlatform)Activator.CreateInstance(t);
                    bool alreadyPresent = false;
                    for (int i = 0; i < platforms.Count; i++)
                    {
                        if (platforms[i].platformName.Equals(instance.platformName))
                        {
                            alreadyPresent = true;
                            BuildPlatform oldInstance = platforms[i];

                            instance.enabled = oldInstance.enabled;

                            if (instance.enabled)
                            {
                                instance.architectures = oldInstance.architectures;
                                instance.variants = oldInstance.variants;
                                instance.distributionList = oldInstance.distributionList;
                            }

                            platforms[i] = instance;
                            break;
                        }
                    }

                    if (!alreadyPresent)
                    {
                        platforms.Add(instance);
                    }
                }
            }
        }

        platformList.platforms = platforms.ToArray();
    }
}

}