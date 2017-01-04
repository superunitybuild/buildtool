using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildNotificationList))]
public class BuildNotificationListDrawer : PropertyDrawer
{
    private bool showErrors = true;
    private bool showWarnings = false;
    private bool showNotifications = false;

    private SerializedProperty errorList;
    private SerializedProperty warningList;
    private SerializedProperty notificationsList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        BuildNotificationList.instance.errors.Clear();
        BuildNotificationList.instance.InitializeErrors();
        BuildNotificationList.instance.Refresh();
        DrawErrors();

        EditorGUI.EndProperty();
    }

    private void DrawErrors()
    {
        if (BuildNotificationList.instance.errors.Count == 0)
            return;

        Color defaultBackgroundColor = GUI.backgroundColor;

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        UnityBuildGUIUtility.DropdownHeader(
            "Errors (" + BuildNotificationList.instance.errors.Count + ")",
            ref showErrors,
            GUILayout.ExpandWidth(true));
        GUI.backgroundColor = defaultBackgroundColor;
        EditorGUILayout.EndHorizontal();

        if (showErrors)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            for (int i = 0; i < BuildNotificationList.instance.errors.Count; i++)
            {
                BuildNotification notification = BuildNotificationList.instance.errors[i];
                EditorGUILayout.HelpBox(notification.title + "\n" + notification.details, MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }
    }
}

}