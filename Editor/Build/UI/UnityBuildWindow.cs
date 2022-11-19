using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class UnityBuildWindow : EditorWindow
    {
        public BuildSettings currentBuildSettings;
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
                window = GetWindow<UnityBuildWindow>(inspWndType);
            }
            else
            {
                window = GetWindow<UnityBuildWindow>();
            }

            window.Show();
        }

        #endregion

        #region Unity Events

        protected void OnEnable()
        {
            GUIContent icon = EditorGUIUtility.IconContent("Packages/com.github.superunitybuild.buildtool/Editor/Assets/Textures/icon.png");
            GUIContent title = new GUIContent("SuperUnityBuild", icon.image);
            titleContent = title;

            BuildNotificationList.instance.InitializeErrors();
            currentBuildSettings = BuildSettings.instance;

            Undo.undoRedoPerformed += UndoHandler;
        }

        protected void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoHandler;
        }

        protected void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

            DrawTitle();

            Init();

            GUILayout.Space(15);

            settings.Update();
            go.Update();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.ExpandHeight(false));

            DrawProperties();

            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);

            DrawBuildButtons();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (go == null)
                go = new SerializedObject(this);

            // Add field to switch the BuildSettings asset
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);
            currentBuildSettings = EditorGUILayout.ObjectField("Build Settings", currentBuildSettings, typeof(BuildSettings), false) as BuildSettings;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            // Override a 'None' selection for the BuildSettings asset
            if (currentBuildSettings == null)
            {
                currentBuildSettings = BuildSettings.instance;
            }

            if (currentBuildSettings != BuildSettings.instance)
            {
                BuildSettings.instance = currentBuildSettings;
                settings = null;
            }

            if (settings == null)
                settings = new SerializedObject(BuildSettings.instance);

            BuildSettings.Init();
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
            EditorGUILayout.LabelField("SuperUnityBuild", UnityBuildGUIUtility.mainTitleStyle);
            GUILayout.Space(30);
            EditorGUILayout.EndVertical();
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

            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
            EditorGUI.BeginDisabledGroup(totalBuildCount < 1);
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button($"Perform All Enabled Builds ({totalBuildCount} Builds)", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)))
                EditorApplication.delayCall += BuildProject.BuildAll;

            GUI.backgroundColor = UnityBuildGUIUtility.defaultBackgroundColor;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void UndoHandler()
        {
            Repaint();
        }

        #endregion
    }
}
