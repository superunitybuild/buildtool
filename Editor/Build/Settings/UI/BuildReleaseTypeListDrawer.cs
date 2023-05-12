using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(BuildReleaseTypeList))]
    public class BuildReleaseTypeListDrawer : PropertyDrawer
    {
        private SerializedProperty list = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginHorizontal();

            bool show = property.isExpanded;
            UnityBuildGUIUtility.DropdownHeader("Release Types", ref show, false, GUILayout.ExpandWidth(true));
            property.isExpanded = show;

            UnityBuildGUIUtility.HelpButton("Parameter-Details#release-types");
            EditorGUILayout.EndHorizontal();

            list = property.FindPropertyRelative("releaseTypes");

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty typeProperty = list.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(typeProperty, GUILayout.MaxHeight(0));
                }

                GUILayout.Space(20);

                if (GUILayout.Button("Add Release Type", GUILayout.ExpandWidth(true)))
                {
                    // Add new entry.
                    int addedIndex = list.arraySize;
                    list.InsertArrayElementAtIndex(addedIndex);

                    // Set default values.
                    SerializedProperty addedEntry = list.GetArrayElementAtIndex(addedIndex);
                    addedEntry.FindPropertyRelative("typeName").stringValue = "NewReleaseType";
                    addedEntry.FindPropertyRelative("productName").stringValue = Application.productName;
                    addedEntry.FindPropertyRelative("bundleIdentifier").stringValue = Application.identifier;
                    addedEntry.FindPropertyRelative("companyName").stringValue = Application.companyName;

                    list.serializedObject.ApplyModifiedProperties();

                    BuildSettings.releaseTypeList.releaseTypes[BuildSettings.releaseTypeList.releaseTypes.Length - 1].sceneList = new SceneList();

                    GUIUtility.keyboardControl = 0;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }
    }
}
