using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class UnityBuildWindow : EditorWindow
{
    public BuildNotificationList notifications = BuildNotificationList.instance;
    private Vector2 scrollPos = Vector2.zero;

    private SerializedObject settings;
    private SerializedObject go;

    #region MenuItems

    [MenuItem("Window/SuperUnityBuild")]
    public static void ShowWindow()
    {
        // Get Inspector type, so we can try to autodock beside it.
        Assembly editorAsm = typeof(Editor).Assembly;
        Type inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");

        // Get and show window.
        UnityBuildWindow window;
        if (inspWndType != null)
        {
            window = EditorWindow.GetWindow<UnityBuildWindow>(inspWndType);
        }
        else
        {
            window = EditorWindow.GetWindow<UnityBuildWindow>();
        }

        window.Show();
    }

    #endregion

    #region Unity Events

    protected void OnEnable()
    {
        GUIContent title = new GUIContent("SuperUnityBuild");
        titleContent = title;

        BuildNotificationList.instance.errors.Clear();
        BuildNotificationList.instance.InitializeErrors();
    }

    protected void OnInspectorUpdate()
    {
        Repaint();
    }

    protected void OnGUI()
    {
        Init();

        settings.Update();
        go.Update();

        DrawTitle();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        DrawProperties();
        DrawBuildButtons();
        GUILayout.Space(30);

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region Private Methods

    private void Init()
    {
        if (go == null)
            go = new SerializedObject(this);

        if (settings == null)
            settings = new SerializedObject(BuildSettings.instance);
    }

    private void DrawTitle()
    {
        GUIStyle mainTitleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        mainTitleStyle.fontSize = 18;
        mainTitleStyle.fontStyle = FontStyle.Bold;
        mainTitleStyle.fixedHeight = 35;

        GUIStyle subTitleStyle = new GUIStyle(mainTitleStyle);
        subTitleStyle.fontSize = 9;
        subTitleStyle.fontStyle = FontStyle.Normal;

        EditorGUILayout.LabelField("Super Unity Build", mainTitleStyle);
        EditorGUILayout.LabelField("by Super Systems Softworks", subTitleStyle);
        GUILayout.Space(15);
    }

    private void DrawProperties()
    {
        EditorGUILayout.PropertyField(settings.FindProperty("_basicSettings"), GUILayout.MaxHeight(0));
        EditorGUILayout.PropertyField(settings.FindProperty("_productParameters"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_releaseTypeList"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_platformList"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_preBuildActions"), new GUIContent("Pre-Build Actions"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_preBuildActions"), new GUIContent("Post-Build Actions"), GUILayout.MaxHeight(10));

        BuildSettings.projectConfigurations.Refresh();
        EditorGUILayout.PropertyField(settings.FindProperty("_projectConfigurations"), GUILayout.MaxHeight(10));

        EditorGUILayout.PropertyField(go.FindProperty("notifications"), GUILayout.MaxHeight(10));

        settings.ApplyModifiedProperties();
    }

    private void DrawBuildButtons()
    {
        Color defaultBackgroundColor = GUI.backgroundColor;

        int totalBuildCount = BuildSettings.projectConfigurations.GetEnabledBuildsCount();

        EditorGUI.BeginDisabledGroup(totalBuildCount < 1);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Perform All Enabled Builds (" + totalBuildCount + " Builds)", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)))
        {
            EditorApplication.delayCall += BuildProject.BuildAll;
        }
        GUI.backgroundColor = defaultBackgroundColor;
        EditorGUI.EndDisabledGroup();
    }

    #endregion
}

}
