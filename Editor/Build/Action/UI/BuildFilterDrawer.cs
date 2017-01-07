using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildFilter))]
public class BuildFilterDrawer : PropertyDrawer
{
    private SerializedProperty list;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        list = property.FindPropertyRelative("clauses");

        // If no clauses present, only display a button to add one.
        if (list.arraySize == 0)
        {
            if (GUILayout.Button("Add Filter", GUILayout.ExpandWidth(true)))
            {
                AddClause();
                property.isExpanded = true;
            }

            return;
        }

        EditorGUILayout.BeginHorizontal();
        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader("Filter", ref show, GUILayout.ExpandWidth(true));
        property.isExpanded = show;
        EditorGUILayout.EndHorizontal();

        if (show)
        {

        }

        EditorGUI.EndProperty();
    }

    private void AddClause()
    {
        // Add.
        int addedIndex = list.arraySize;
        list.InsertArrayElementAtIndex(addedIndex);

        // Initialize.
        SerializedProperty addedClause = list.GetArrayElementAtIndex(addedIndex);
        addedClause.FindPropertyRelative("type").enumValueIndex = 0;
        addedClause.FindPropertyRelative("comparison").enumValueIndex = 0;
        addedClause.FindPropertyRelative("test").stringValue = string.Empty;

        // Apply.
        list.serializedObject.ApplyModifiedProperties();
    }
}

}
