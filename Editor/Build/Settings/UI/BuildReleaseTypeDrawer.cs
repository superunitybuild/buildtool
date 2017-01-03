using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildReleaseType))]
public class BuildReleaseTypeDrawer : PropertyDrawer
{
    private bool show = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        char chr = Event.current.character;
        if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') && chr != '-' && chr != '_' && chr != ' ')
        {
            Event.current.character = '\0';
        }

        UnityBuildGUIUtility.DropdownHeader(property.FindPropertyRelative("typeName").stringValue, ref show, GUILayout.ExpandWidth(true));

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            GUILayout.Label("Basic Info", UnityBuildGUIUtility.midHeaderStyle);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("typeName"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("bundleIndentifier"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("productName"));

            GUILayout.Space(20);
            GUILayout.Label("Build Options", UnityBuildGUIUtility.midHeaderStyle);

            SerializedProperty developmentBuild = property.FindPropertyRelative("developmentBuild");
            SerializedProperty allowDebugging = property.FindPropertyRelative("allowDebugging");
            developmentBuild.boolValue = EditorGUILayout.ToggleLeft(" Development Build", developmentBuild.boolValue);
            allowDebugging.boolValue = EditorGUILayout.ToggleLeft(" Script Debugging", allowDebugging.boolValue);



            EditorGUILayout.PropertyField(property.FindPropertyRelative("sceneList"));
            
            property.serializedObject.ApplyModifiedProperties();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.MaxWidth(150)))
            {
                BuildReleaseType[] types = BuildSettings.Instance._releaseTypeList.releaseTypes;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].typeName == property.FindPropertyRelative("typeName").stringValue)
                    {
                        ArrayUtility.RemoveAt<BuildReleaseType>(ref BuildSettings.Instance._releaseTypeList.releaseTypes, i);
                        GUIUtility.keyboardControl = 0;
                        break;
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUI.EndProperty();
    }
}

}