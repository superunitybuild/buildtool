using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(ProductParameters))]
public class ProductParametersDrawer : PropertyDrawer
{
    private bool show = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader("Product Parameters", ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#Product-Parameters");
        EditorGUILayout.EndHorizontal();

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("version"));

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Build Count: " + property.FindPropertyRelative("buildCounter").intValue.ToString(), EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MinWidth(100));
            GUILayout.Space(20);
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
            {
                property.FindPropertyRelative("buildCounter").intValue = 0;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            property.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}