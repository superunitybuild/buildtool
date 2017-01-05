using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildAction))]
public class BuildActionDrawer : PropertyDrawer
{
    private bool show = false;
    private SerializedProperty list = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.BeginHorizontal();
        UnityBuildGUIUtility.DropdownHeader("Release Types", ref show, GUILayout.ExpandWidth(true));
        UnityBuildGUIUtility.HelpButton("Parameter-Details#release-types");
        EditorGUILayout.EndHorizontal();

        if (show)
        {

        }

        EditorGUI.EndProperty();
    }
}

}