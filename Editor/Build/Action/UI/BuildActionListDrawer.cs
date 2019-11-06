using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildActionList))]
public class BuildActionListDrawer : PropertyDrawer
{
    private int index = 0;
    private SerializedProperty list = null;

    [SerializeField]
    private BuildAction buildAction;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader(label.text, ref show, false, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        UnityBuildGUIUtility.HelpButton("Parameter-Details#prepost-build-actions");
        EditorGUILayout.EndHorizontal();

        list = property.FindPropertyRelative("buildActions");

        List<Type> actionTypes;
        if (property.name.ToUpper().Contains("PRE"))
        {
            actionTypes = BuildActionListUtility.preBuildActions;
        }
        else
        {
            actionTypes = BuildActionListUtility.postBuildActions;
        }

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            DrawActions(property);

            if (list.arraySize > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            DrawActionList(property, actionTypes);

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }

    private void DrawActions(SerializedProperty property)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty listEntry = list.GetArrayElementAtIndex(i);

            BuildAction buildAction = listEntry.objectReferenceValue as BuildAction;
            if (buildAction == null)
            {
                list.DeleteArrayElementAtIndex(i);
                --i;
                continue;
            }

            SerializedObject serializedBuildAction = new SerializedObject(buildAction);

            EditorGUILayout.BeginHorizontal();
            bool show = listEntry.isExpanded;

            buildAction.actionEnabled = EditorGUILayout.Toggle(buildAction.actionEnabled, GUILayout.Width(15));
            EditorGUI.BeginDisabledGroup(!buildAction.actionEnabled);
            UnityBuildGUIUtility.DropdownHeader(buildAction.actionName, ref show, false, GUILayout.ExpandWidth(true));
            EditorGUI.EndDisabledGroup();
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
                BuildAction[] buildActions;
                if (property.name.ToUpper().Contains("PRE"))
                {
                    buildActions = BuildSettings.preBuildActions.buildActions;
                }
                else
                {
                    buildActions = BuildSettings.postBuildActions.buildActions;
                }

                // Destroy underlying object.
                ScriptableObject.DestroyImmediate(buildActions[i], true);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                // Remove object reference from list.
                // TODO: Why do I need to call this twice? First call nulls reference, second one then deletes null entry.
                list.DeleteArrayElementAtIndex(i);
                list.DeleteArrayElementAtIndex(i);
                show = false;
            }

            EditorGUILayout.EndHorizontal();

            if (show && buildAction.actionEnabled)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);
                buildAction.Draw(serializedBuildAction);
                EditorGUILayout.EndVertical();
            }
        }
    }

    private void DrawActionList(SerializedProperty property, List<Type> actionTypes)
    {
        if (actionTypes.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();

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

                //buildActions[addedIndex] = Activator.CreateInstance(addedType) as BuildAction;
                buildActions[addedIndex] = ScriptableObject.CreateInstance(addedType) as BuildAction;
                buildActions[addedIndex].name = addedType.Name;
                buildActions[addedIndex].actionName = addedType.Name;
                buildActions[addedIndex].filter = new BuildFilter();

                AssetDatabase.AddObjectToAsset(buildActions[addedIndex], BuildSettings.instance);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                index = 0;
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("No Build Actions found.", MessageType.Info);
        }
    }
}

}