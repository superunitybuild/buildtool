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
        UnityBuildGUIUtility.DropdownHeader("Filter", ref show, false, GUILayout.ExpandWidth(true));
        property.isExpanded = show;
        EditorGUILayout.EndHorizontal();

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            EditorGUILayout.BeginHorizontal();
            SerializedProperty condition = property.FindPropertyRelative("condition");

            EditorGUILayout.LabelField("Run this BuildAction if", GUILayout.Width(130));
            BuildFilter.FilterCondition modifiedCondition = (BuildFilter.FilterCondition)EditorGUILayout.EnumPopup((BuildFilter.FilterCondition)condition.enumValueIndex, GUILayout.Width(95));
            condition.enumValueIndex = (int)modifiedCondition;
            EditorGUILayout.LabelField("of these conditions is/are true.", GUILayout.ExpandWidth(false));

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty listEntry = list.GetArrayElementAtIndex(i);
                SerializedProperty testType = listEntry.FindPropertyRelative("type");
                SerializedProperty testComparison = listEntry.FindPropertyRelative("comparison");
                SerializedProperty testValue = listEntry.FindPropertyRelative("test");

                BuildFilter.FilterType modifiedType = (BuildFilter.FilterType)EditorGUILayout.EnumPopup((BuildFilter.FilterType)testType.enumValueIndex, GUILayout.ExpandWidth(false), GUILayout.Width(140));
                BuildFilter.FilterComparison modifiedComparison = (BuildFilter.FilterComparison)EditorGUILayout.EnumPopup((BuildFilter.FilterComparison)testComparison.enumValueIndex, GUILayout.ExpandWidth(false), GUILayout.Width(100));
                testType.enumValueIndex = (int)modifiedType;
                testComparison.enumValueIndex = (int)modifiedComparison;
                
                testValue.stringValue = GUILayout.TextField(testValue.stringValue);

                if (GUILayout.Button("X", UnityBuildGUIUtility.helpButtonStyle))
                {
                    list.DeleteArrayElementAtIndex(i);
                    list.serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Condition", GUILayout.ExpandWidth(true)))
            {
                AddClause();
            }

            EditorGUILayout.EndVertical();
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
