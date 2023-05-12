using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(BuildNotificationList))]
    public class BuildNotificationListDrawer : PropertyDrawer
    {
        private bool show = true;

        private SerializedProperty errorList;
        private SerializedProperty warningList;
        private SerializedProperty notificationsList;

        private GUIContent clearButtonContent = new GUIContent("X", "Clear");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            DrawErrors();
            DrawWarnings();
            DrawNotifications();

            EditorGUI.EndProperty();
        }

        private void DrawErrors()
        {
            int errorCount = 0;
            int clearableCount = 0;
            for (int i = 0; i < BuildNotificationList.instance.errors.Count; i++)
            {
                BuildNotification notification = BuildNotificationList.instance.errors[i];

                if (notification.valid == null || notification.valid())
                    ++errorCount;
                if (notification.clearable)
                    ++clearableCount;
            }

            if (errorCount == 0)
                return;

            Color defaultBackgroundColor = GUI.backgroundColor;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.red;
            UnityBuildGUIUtility.DropdownHeader(
                $"Errors ({errorCount})",
                ref show, true,
                GUILayout.ExpandWidth(true));

            if (clearableCount > 0)
            {
                if (GUILayout.Button(clearButtonContent, UnityBuildGUIUtility.helpButtonStyle))
                    BuildNotificationList.instance.RefreshErrors();
            }

            GUI.backgroundColor = defaultBackgroundColor;
            EditorGUILayout.EndHorizontal();

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                for (int i = 0; i < BuildNotificationList.instance.errors.Count; i++)
                {
                    BuildNotification notification = BuildNotificationList.instance.errors[i];

                    if (notification.valid == null || notification.valid())
                        EditorGUILayout.HelpBox(notification.title + "\n" + notification.details, MessageType.Error);
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(15);
        }

        private void DrawWarnings()
        {
            int warningCount = 0;
            int clearableCount = 0;
            for (int i = 0; i < BuildNotificationList.instance.warnings.Count; i++)
            {
                BuildNotification notification = BuildNotificationList.instance.warnings[i];

                if (notification.valid == null || notification.valid())
                    ++warningCount;
                if (notification.clearable)
                    ++clearableCount;
            }

            if (warningCount == 0)
                return;

            Color defaultBackgroundColor = GUI.backgroundColor;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.yellow;
            UnityBuildGUIUtility.DropdownHeader(
                $"Warnings ({warningCount})",
                ref show, true,
                GUILayout.ExpandWidth(true));

            if (clearableCount > 0)
            {
                if (GUILayout.Button(clearButtonContent, UnityBuildGUIUtility.helpButtonStyle))
                    BuildNotificationList.instance.RefreshWarnings();
            }

            GUI.backgroundColor = defaultBackgroundColor;
            EditorGUILayout.EndHorizontal();

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                for (int i = 0; i < BuildNotificationList.instance.warnings.Count; i++)
                {
                    BuildNotification notification = BuildNotificationList.instance.warnings[i];

                    if (notification.valid == null || notification.valid())
                        EditorGUILayout.HelpBox(notification.title + "\n" + notification.details, MessageType.Warning);
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(15);
        }

        private void DrawNotifications()
        {
            int warningCount = 0;
            int clearableCount = 0;
            for (int i = 0; i < BuildNotificationList.instance.notifications.Count; i++)
            {
                BuildNotification notification = BuildNotificationList.instance.notifications[i];

                if (notification.valid == null || notification.valid())
                    ++warningCount;
                if (notification.clearable)
                    ++clearableCount;
            }

            if (warningCount == 0)
                return;

            Color defaultBackgroundColor = GUI.backgroundColor;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            UnityBuildGUIUtility.DropdownHeader(
                $"Log ({warningCount})",
                ref show, true,
                GUILayout.ExpandWidth(true));

            if (clearableCount > 0)
            {
                if (GUILayout.Button(clearButtonContent, UnityBuildGUIUtility.helpButtonStyle))
                    BuildNotificationList.instance.RefreshNotifications();
            }

            GUI.backgroundColor = defaultBackgroundColor;
            EditorGUILayout.EndHorizontal();

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                for (int i = BuildNotificationList.instance.notifications.Count - 1; i >= 0; i--)
                {
                    BuildNotification notification = BuildNotificationList.instance.notifications[i];

                    if (notification.valid == null || notification.valid())
                        EditorGUILayout.HelpBox(notification.title + "\n" + notification.details, MessageType.None);
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}
