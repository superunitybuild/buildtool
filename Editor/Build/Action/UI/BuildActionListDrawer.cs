using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildActionList))]
public class BuildActionListDrawer : PropertyDrawer
{
    private bool show = false;
    private SerializedProperty list = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader(label.text, ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#build-actions");
        EditorGUILayout.EndHorizontal();

        if (show)
        {

        }

        EditorGUI.EndProperty();
    }
}

}