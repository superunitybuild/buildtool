using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildReleaseType))]
public class BuildReleaseTypeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        char chr = Event.current.character;
        if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') && chr != '-' && chr != '_' && chr != ' ')
        {
            Event.current.character = '\0';
        }

        bool show = false;
        UnityBuildGUIUtility.DropdownHeader(property.FindPropertyRelative("typeName").stringValue, ref show);

        if (true)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            EditorGUILayout.PropertyField(property.FindPropertyRelative("typeName"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("bundleIndentifier"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("productName"));

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