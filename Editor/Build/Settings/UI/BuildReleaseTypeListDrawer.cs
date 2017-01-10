using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
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
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Release Type", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150)))
            {
                // Add new entry.
                int addedIndex = list.arraySize;
                list.InsertArrayElementAtIndex(addedIndex);
                
                // Set default values.
                SerializedProperty addedEntry = list.GetArrayElementAtIndex(addedIndex);
                addedEntry.FindPropertyRelative("typeName").stringValue = "NewReleaseType";
                addedEntry.FindPropertyRelative("productName").stringValue = Application.productName;

                list.serializedObject.ApplyModifiedProperties();

                BuildSettings.releaseTypeList.releaseTypes[BuildSettings.releaseTypeList.releaseTypes.Length - 1].sceneList = new SceneList();

                GUIUtility.keyboardControl = 0;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}