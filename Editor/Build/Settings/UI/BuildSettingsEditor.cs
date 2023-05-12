using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomEditor(typeof(BuildSettings))]
    public class BuildSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Color defaultBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Open SuperUnityBuild", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)))
                UnityBuildWindow.ShowWindow();

            GUI.backgroundColor = defaultBackgroundColor;
        }
    }
}
