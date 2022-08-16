using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
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

            List<Type> actionTypes = property.name.ToUpper().Contains("PRE") ?
                BuildActionListUtility.preBuildActions :
                BuildActionListUtility.postBuildActions;

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                DrawActions(property);

                if (list.arraySize > 0)
                    GUILayout.Space(10);

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
                    list.SafeDeleteArrayElementAtIndex(i);
                    --i;
                    continue;
                }

                SerializedObject serializedBuildAction = new SerializedObject(buildAction);

                EditorGUILayout.BeginHorizontal();
                bool show = listEntry.isExpanded;
                string tooltip = buildAction.ToString();
                string text = UnityBuildGUIUtility.ToLabel(tooltip, 50);

                buildAction.actionEnabled = EditorGUILayout.Toggle(buildAction.actionEnabled, GUILayout.Width(15));
                EditorGUI.BeginDisabledGroup(!buildAction.actionEnabled);
                UnityBuildGUIUtility.DropdownHeader(new GUIContent(text, tooltip), ref show, false, GUILayout.ExpandWidth(true));
                EditorGUI.EndDisabledGroup();
                listEntry.isExpanded = show;

                UnityBuildGUIUtility.ReorderArrayControls(list, i);

                if (UnityBuildGUIUtility.DeleteButton())
                {
                    BuildAction[] buildActions = GetBuildActionsForProperty(property);

                    // Destroy underlying object.
                    ScriptableObject.DestroyImmediate(buildActions[i], true);
                    AssetDatabaseUtility.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                    // Remove object reference from list.
                    list.SafeDeleteArrayElementAtIndex(i);
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

                    BuildAction[] buildActions = GetBuildActionsForProperty(property);

                    buildActions[addedIndex] = ScriptableObject.CreateInstance(addedType) as BuildAction;
                    buildActions[addedIndex].name = addedType.Name;
                    buildActions[addedIndex].actionName = addedType.Name;
                    buildActions[addedIndex].filter = new BuildFilter();

                    AssetDatabase.AddObjectToAsset(buildActions[addedIndex], BuildSettings.instance);
                    AssetDatabaseUtility.ImportAsset(AssetDatabase.GetAssetPath(BuildSettings.instance));

                    index = 0;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No Build Actions found.", MessageType.Info);
            }
        }

        private BuildAction[] GetBuildActionsForProperty(SerializedProperty property)
        {
            return property.name.ToUpper().Contains("PRE") ?
                BuildSettings.preBuildActions.buildActions :
                BuildSettings.postBuildActions.buildActions;
        }
    }
}
