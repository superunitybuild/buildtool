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
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4_OR_NEWER
        GUIContent title = new GUIContent("SuperUnityBuild");
        titleContent = title;
#else
        title = "SuperUnityBuild";
#endif

        BuildNotificationList.instance.InitializeErrors();

        Undo.undoRedoPerformed += UndoHandler;
    }

    protected void OnDisable()
    {
        Undo.undoRedoPerformed -= UndoHandler;
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

        BuildSettings.Init();
    }

    private void DrawTitle()
    {
        EditorGUILayout.LabelField("Super Unity Build", UnityBuildGUIUtility.mainTitleStyle);
        EditorGUILayout.LabelField("by Super Systems Softworks", UnityBuildGUIUtility.subTitleStyle);
        GUILayout.Space(15);
    }

    private void DrawProperties()
    {
        EditorGUILayout.PropertyField(settings.FindProperty("_basicSettings"), GUILayout.MaxHeight(0));
        EditorGUILayout.PropertyField(settings.FindProperty("_productParameters"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_releaseTypeList"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_platformList"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_preBuildActions"), new GUIContent("Pre-Build Actions"), GUILayout.MaxHeight(10));
        EditorGUILayout.PropertyField(settings.FindProperty("_postBuildActions"), new GUIContent("Post-Build Actions"), GUILayout.MaxHeight(10));

        BuildSettings.projectConfigurations.Refresh();
        EditorGUILayout.PropertyField(settings.FindProperty("_projectConfigurations"), GUILayout.MaxHeight(10));

        EditorGUILayout.PropertyField(go.FindProperty("notifications"), GUILayout.MaxHeight(10));

        settings.ApplyModifiedProperties();
    }

    private void DrawBuildButtons()
    {
        int totalBuildCount = BuildSettings.projectConfigurations.GetEnabledBuildsCount();

        EditorGUI.BeginDisabledGroup(totalBuildCount < 1);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Perform All Enabled Builds (" + totalBuildCount + " Builds)", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)))
        {
            EditorApplication.delayCall += BuildProject.BuildAll;
        }
        GUI.backgroundColor = UnityBuildGUIUtility.defaultBackgroundColor;
        EditorGUI.EndDisabledGroup();
    }

    private void UndoHandler()
    {
        Repaint();
    }

    #endregion
}

}
