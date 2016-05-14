using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildReleaseTypeList))]
public class BuildReleaseTypeListDrawer : PropertyDrawer
{
    private bool show = false;
    private SerializedProperty list = null;
    private BuildReleaseTypeList typeList = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader("Release Types", ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#Release-Types");
        EditorGUILayout.EndHorizontal();

        list = property.FindPropertyRelative("releastTypes");

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
                int addedIndex = list.arraySize;
                list.InsertArrayElementAtIndex(addedIndex);
                property.serializedObject.ApplyModifiedProperties();
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