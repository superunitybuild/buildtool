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

    private List<string> availablePlatformNameList = new List<string>();
    private List<Type> availablePlatformTypeList = new List<Type>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        property.serializedObject.Update();

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
            list.serializedObject.Update();
        //}

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            // Draw all created/enabled platforms.
            int enabledCount = 0;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                SerializedProperty platformEnabled = platformProperty.FindPropertyRelative("enabled");

                if (platformEnabled.boolValue)
                {
                    ++enabledCount;
                    EditorGUILayout.PropertyField(platformProperty, GUILayout.MaxHeight(0));
                }
            }

            if (enabledCount > 0)
                GUILayout.Space(20);

            // Draw all available platforms.
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            index = EditorGUILayout.Popup(index, availablePlatformNameList.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
            if (GUILayout.Button("Add Platform", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)))
            {
                platformList.platforms.Add((BuildPlatform)Activator.CreateInstance(availablePlatformTypeList[index]));
                platformList.platforms[platformList.platforms.Count - 1].enabled = true;

                for (int i = platformList.platforms.Count - 1; i >= 0; i--)
                {
                    if (!platformList.platforms[i].enabled)
                        platformList.platforms.RemoveAt(i);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }

    private void PopulateList()
    {
        Type ti = typeof(BuildPlatform);

        availablePlatformNameList.Clear();
        availablePlatformTypeList.Clear();
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    BuildPlatform instance = (BuildPlatform)Activator.CreateInstance(t);
                    availablePlatformNameList.Add(instance.platformName);
                    availablePlatformTypeList.Add(t);
                }
            }
        }
    }
}

}