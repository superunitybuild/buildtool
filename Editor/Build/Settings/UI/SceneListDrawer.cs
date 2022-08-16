using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(SceneList))]
    public class SceneListDrawer : PropertyDrawer
    {
        private const int AUTO_COLLAPSE_SIZE = 50;

        private SerializedProperty list;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginHorizontal();

            bool show = property.isExpanded;
            UnityBuildGUIUtility.DropdownHeader("Scene List", ref show, false, GUILayout.ExpandWidth(true));
            property.isExpanded = show;

            EditorGUILayout.EndHorizontal();

            // Refresh all scene lists.
            for (int i = 0; i < BuildSettings.releaseTypeList.releaseTypes.Length; i++)
            {
                BuildReleaseType rt = BuildSettings.releaseTypeList.releaseTypes[i];
                rt.sceneList.Refresh();
            }

            list = property.FindPropertyRelative("enabledScenes");

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                SerializedProperty platformProperty;
                string fileGUID;
                string filePath;
                string sceneName = "N/A";
                if (list.arraySize > 0)
                {
                    platformProperty = list.GetArrayElementAtIndex(0);
                    fileGUID = platformProperty.FindPropertyRelative("fileGUID").stringValue;
                    filePath = AssetDatabase.GUIDToAssetPath(fileGUID);
                    sceneName = Path.GetFileNameWithoutExtension(filePath);
                }

                EditorGUILayout.BeginHorizontal();

                show = list.isExpanded;
                UnityBuildGUIUtility.DropdownHeader(string.Format("Scenes ({0}) (First Scene: {1})", list.arraySize, sceneName), ref show, false, GUILayout.ExpandWidth(true));
                list.isExpanded = show;

                EditorGUILayout.EndHorizontal();

                if (show)
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        platformProperty = list.GetArrayElementAtIndex(i);
                        fileGUID = platformProperty.FindPropertyRelative("fileGUID").stringValue;
                        filePath = AssetDatabase.GUIDToAssetPath(fileGUID);
                        sceneName = Path.GetFileNameWithoutExtension(filePath);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.TextArea($"{sceneName} ({filePath})");

                        UnityBuildGUIUtility.ReorderArrayControls(list, i);

                        if (UnityBuildGUIUtility.DeleteButton())
                            list.SafeDeleteArrayElementAtIndex(i);

                        property.serializedObject.ApplyModifiedProperties();

                        EditorGUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(20);

                Rect dropArea = GUILayoutUtility.GetRect(0, 50.0f, GUILayout.ExpandWidth(true));
                GUI.Box(dropArea, "Drag and Drop scene files here to add to list.", UnityBuildGUIUtility.dragDropStyle);
                Event currentEvent = Event.current;

                switch (currentEvent.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (dropArea.Contains(currentEvent.mousePosition))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if (currentEvent.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                foreach (Object obj in DragAndDrop.objectReferences)
                                {
                                    if (obj.GetType() == typeof(SceneAsset))
                                    {
                                        string objFilepath = AssetDatabase.GetAssetPath(obj);
                                        string objGUID = AssetDatabase.AssetPathToGUID(objFilepath);
                                        AddScene(objGUID);
                                    }
                                }

                                if (list.arraySize >= AUTO_COLLAPSE_SIZE)
                                    list.isExpanded = false;
                            }
                        }
                        break;
                }

                if (GUILayout.Button("Clear Scene List", GUILayout.ExpandWidth(true)))
                    list.ClearArray();

                if (GUILayout.Button("Add Scene Files from Build Settings", GUILayout.ExpandWidth(true)))
                    GetSceneFilesFromBuildSettings();

                if (GUILayout.Button("Add Scene File Directory", GUILayout.ExpandWidth(true)))
                    GetSceneFileDirectory("Add Scene Files");

                if (GUILayout.Button("Set First Scene by File", GUILayout.ExpandWidth(true)))
                    SetFirstSceneByFile();

                list.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }

        private bool CheckForDuplicate(string newFileGUID)
        {
            bool duplicateFound = false;

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                string fileGUID = platformProperty.FindPropertyRelative("fileGUID").stringValue;

                if (fileGUID == newFileGUID)
                {
                    duplicateFound = true;
                    break;
                }
            }

            return duplicateFound;
        }

        private void GetSceneFileDirectory(string message)
        {
            string directory = EditorUtility.OpenFolderPanel(message, Application.dataPath, "");

            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            string[] files = Directory.GetFiles(directory, "*.unity", SearchOption.AllDirectories);
            foreach (string filepath in files)
            {
                string fullpath = "Assets" + Path.GetFullPath(filepath).Substring(Path.GetFullPath(Application.dataPath).Length);
                string objGUID = AssetDatabase.AssetPathToGUID(fullpath);
                AddScene(objGUID);
            }

            if (list.arraySize >= AUTO_COLLAPSE_SIZE)
                list.isExpanded = false;
        }

        private void GetSceneFilesFromBuildSettings()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            for (int i = 0; i < scenes.Length; i++)
            {
                AddScene(AssetDatabase.AssetPathToGUID(scenes[i].path));
            }
        }

        private void SetFirstSceneByFile()
        {
            string filepath = EditorUtility.OpenFilePanel("Select Scene File", Application.dataPath, "unity");

            if (string.IsNullOrEmpty(filepath))
            {
                return;
            }

            string fullpath = "Assets" + Path.GetFullPath(filepath).Substring(Path.GetFullPath(Application.dataPath).Length);
            string objGUID = AssetDatabase.AssetPathToGUID(fullpath);

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty platformProperty = list.GetArrayElementAtIndex(i);
                string fileGUID = platformProperty.FindPropertyRelative("fileGUID").stringValue;

                if (fileGUID == objGUID)
                {
                    list.SafeDeleteArrayElementAtIndex(i);
                    break;
                }
            }

            list.InsertArrayElementAtIndex(0);
            list.GetArrayElementAtIndex(0).FindPropertyRelative("fileGUID").stringValue = objGUID;
        }

        private void AddScene(string objGUID)
        {
            if (!string.IsNullOrEmpty(objGUID) && !CheckForDuplicate(objGUID))
            {
                int addedIndex = list.arraySize;
                list.InsertArrayElementAtIndex(addedIndex);
                list.GetArrayElementAtIndex(addedIndex).FindPropertyRelative("fileGUID").stringValue = objGUID;
            }
        }
    }
}
