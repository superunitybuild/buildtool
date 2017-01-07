using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildAction))]
public class BuildActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        DrawBuildAction(property);
        DrawBuildActionSettings(property);

        EditorGUI.EndProperty();
    }

    private void DrawBuildAction(SerializedProperty property)
    {
        EditorGUILayout.PropertyField(property.FindPropertyRelative("filter"), GUILayout.Height(0));
    }

    protected virtual void DrawBuildActionSettings(SerializedProperty property)
    {
    }
}

}
