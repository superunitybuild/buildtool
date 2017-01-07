using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildActionList))]
public class BuildActionListDrawer : PropertyDrawer
{
    private bool show = false;
    private int index = 0;
    private SerializedProperty list = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader(label.text, ref show, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        UnityBuildGUIUtility.HelpButton("Parameter-Details#build-actions");
        EditorGUILayout.EndHorizontal();

        list = property.FindPropertyRelative("buildActions");

        List<Type> actionTypes;
        if (property.name.ToUpper().Contains("PRE"))
        {
            actionTypes = PreBuildAction.preBuildActions;
        }
        else
        {
            actionTypes = PostBuildAction.postBuildActions;
        }

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty listEntry = list.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                show = listEntry.isExpanded;
                UnityBuildGUIUtility.DropdownHeader(listEntry.FindPropertyRelative("name").stringValue, ref show, GUILayout.ExpandWidth(true));
                listEntry.isExpanded = show;

                EditorGUI.BeginDisabledGroup(i == 0);
                if (GUILayout.Button("↑↑", UnityBuildGUIUtility.helpButtonStyle))
                {
                    list.MoveArrayElement(i, 0);
                }
                if (GUILayout.Button("↑", UnityBuildGUIUtility.helpButtonStyle))
                {
                    list.MoveArrayElement(i, i - 1);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(i == list.arraySize - 1);
                if (GUILayout.Button("↓", UnityBuildGUIUtility.helpButtonStyle))
                {
                    list.MoveArrayElement(i, i + 1);
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("X", UnityBuildGUIUtility.helpButtonStyle))
                {
                    list.DeleteArrayElementAtIndex(i);
                    show = false;
                }

                EditorGUILayout.EndHorizontal();

                if (show)
                {
                    EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle); 
                    EditorGUILayout.PropertyField(listEntry, GUILayout.Height(0));
                    EditorGUILayout.EndVertical();
                }
            }

            if (list.arraySize > 0)
            {
                GUILayout.Space(20);
            }

            if (actionTypes.Count > 0)
            {
                GUILayout.BeginHorizontal();

                string[] buildActionNameList = new string[actionTypes.Count];
                for (int i = 0; i < buildActionNameList.Length; i++)
                {
                    buildActionNameList[i] = actionTypes[i].Name;
                }

                index = EditorGUILayout.Popup(index, buildActionNameList, UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Add Build Action", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)) && index < actionTypes.Count)
                {
                    Type addedType = actionTypes[index];

                    int addedIndex = list.arraySize;
                    list.InsertArrayElementAtIndex(addedIndex);
                    list.serializedObject.ApplyModifiedProperties();

                    BuildAction[] buildActions;
                    if (property.name.ToUpper().Contains("PRE"))
                    {
                        buildActions = BuildSettings.preBuildActions.buildActions;
                    }
                    else
                    {
                        buildActions = BuildSettings.postBuildActions.buildActions;
                    }

                    buildActions[addedIndex] = Activator.CreateInstance(addedType) as BuildAction;
                    buildActions[addedIndex].name = addedType.Name;
                    buildActions[addedIndex].filter = new BuildFilter();

                    index = 0;
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No Build Actions found.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}