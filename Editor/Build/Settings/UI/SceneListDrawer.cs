using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(SceneList))]
public class SceneListDrawer : PropertyDrawer
{
    private int index = 0;
    private List<SceneList.Scene> availableScenesList = null;
    private SerializedProperty list;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader("SceneList", ref show, false, GUILayout.ExpandWidth(true));
        property.isExpanded = show;

        EditorGUILayout.EndHorizontal();

        //Refresh all scene lists.
        for (int i = 0; i < BuildSettings.releaseTypeList.releaseTypes.Length; i++)
        {
            BuildReleaseType rt = BuildSettings.releaseTypeList.releaseTypes[i];
            rt.sceneList.Refresh();
        }

        list = property.FindPropertyRelative("enabledScenes");
        PopulateSceneList();

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);

                string filePath = platformProperty.FindPropertyRelative("filePath").stringValue;
                string sceneName = Path.GetFileNameWithoutExtension(filePath);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextArea(sceneName + " (" + filePath + ")");

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
                }

                property.serializedObject.ApplyModifiedProperties();

                PopulateSceneList();

                EditorGUILayout.EndHorizontal();
            }

            if (list.arraySize > 0)
            {
                GUILayout.Space(20);
            }

            if (availableScenesList.Count > 0)
            {   
                GUILayout.BeginHorizontal();

                string[] sceneStringList = new string[availableScenesList.Count];
                for (int i = 0; i < sceneStringList.Length; i++)
                {
                    sceneStringList[i] = Path.GetFileNameWithoutExtension(availableScenesList[i].filePath) + " (" + availableScenesList[i].filePath.Replace("/", "\\") + ")";
                }

                index = EditorGUILayout.Popup(index, sceneStringList, UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Add Scene", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)) && index < availableScenesList.Count)
                {
                    int addedIndex = list.arraySize;
                    SceneList.Scene scene = availableScenesList[index];
                    list.InsertArrayElementAtIndex(addedIndex);
                    list.GetArrayElementAtIndex(addedIndex).FindPropertyRelative("filePath").stringValue = scene.filePath;

                    availableScenesList.RemoveAt(index);

                    index = 0;
                }

                GUILayout.EndHorizontal();
            }

            list.serializedObject.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Refresh Scene List", GUILayout.ExpandWidth(true)))
            {
                PopulateSceneList();
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }

    private void PopulateSceneList()
    {
        if (availableScenesList == null)
            availableScenesList = new List<SceneList.Scene>();
        else
            availableScenesList.Clear();

        string[] allScenes = SceneList.GetListOfAllScenes();

        for (int i = 0; i < allScenes.Length; i++)
        {
            bool sceneAlreadyAdded = false;
            for (int j = 0; j < list.arraySize; j++)
            {
                if (Path.Equals(list.GetArrayElementAtIndex(j).FindPropertyRelative("filePath").stringValue, allScenes[i]))
                {
                    sceneAlreadyAdded = true;
                    break;
                }
            }

            if (!sceneAlreadyAdded)
            {
                SceneList.Scene scene = new SceneList.Scene();
                scene.filePath = allScenes[i];
                availableScenesList.Add(scene);
            }
        }
    }
}

}