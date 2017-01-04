using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class UnityBuildWindow : EditorWindow
{
    public BuildNotificationList notifications = BuildNotificationList.instance;
    private Vector2 scrollPos = Vector2.zero;

    #region MenuItems

    [MenuItem("Window/SuperUnityBuild")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<UnityBuildWindow>();
    }

    #endregion
    
    protected void OnEnable()
    {
        GUIContent title = new GUIContent("SuperUnityBuild");
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

        SerializedObject settings = new SerializedObject(BuildSettings.instance);
        SerializedObject go = new SerializedObject(this);

        EditorGUILayout.LabelField("Super Unity Build", mainTitleStyle);
        EditorGUILayout.LabelField("by Super Systems Softworks", subTitleStyle);
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        EditorGUILayout.PropertyField(settings.FindProperty("_basicSettings"), GUILayout.MaxHeight(0));
        EditorGUILayout.PropertyField(settings.FindProperty("_productParameters"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_releaseTypeList"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_platformList"), GUILayout.MaxHeight(10));

        BuildSettings.projectConfigurations.Refresh();
        EditorGUILayout.PropertyField(settings.FindProperty("_projectConfigurations"), GUILayout.MaxHeight(10));

        EditorGUILayout.PropertyField(go.FindProperty("notifications"), GUILayout.MaxHeight(10));

        GUILayout.Space(30);

        EditorGUILayout.EndScrollView();

        settings.ApplyModifiedProperties();
    }
}

}