using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomEditor(typeof(BuildSettings))]
public class BuildSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_basicSettings"), true);
        serializedObject.ApplyModifiedProperties();
    }
}

}