using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

public class UnityBuildWindow : EditorWindow
{
    #region MenuItems

    [MenuItem("Window/UnityBuild")]
    [MenuItem("Build/Edit Settings")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<UnityBuildWindow>();
    }

    #endregion

    protected void OnEnable()
    {
        GUIContent title = new GUIContent("UnityBuild");
        titleContent = title;
    }

    protected void OnGUI()
    {
        GUIStyle mainTitleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        mainTitleStyle.fontSize = 18;
        mainTitleStyle.fontStyle = FontStyle.Bold;
        mainTitleStyle.fixedHeight = 30;

        GUIStyle subTitleStyle = new GUIStyle(mainTitleStyle);
        subTitleStyle.fontSize = 11;
        subTitleStyle.fontStyle = FontStyle.Normal;

        SerializedObject obj = new SerializedObject(BuildSettings.Instance);

        EditorGUILayout.LabelField("Super Unity Build", mainTitleStyle);
        EditorGUILayout.LabelField("by Super Systems Softworks", subTitleStyle);
        EditorGUILayout.PropertyField(obj.FindProperty("_basicSettings"), GUILayout.MaxHeight(20));
        EditorGUILayout.PropertyField(obj.FindProperty("_platformList"), GUILayout.MaxHeight(10));

        obj.ApplyModifiedProperties();
    }
}

}