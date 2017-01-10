using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(FilePathAttribute))]
public class FilePathDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            base.OnGUI(position, property, label);

        EditorGUI.BeginProperty(position, label, property);

        FilePathAttribute filePathAttr = attribute as FilePathAttribute;

        EditorGUILayout.BeginHorizontal();

        if (filePathAttr.allowManualEdit)
            property.stringValue = EditorGUILayout.TextField(label, property.stringValue);
        else
            EditorGUILayout.TextField(label, property.stringValue);

        if (GUILayout.Button("...", UnityBuildGUIUtility.helpButtonStyle))
        {
            SetPath(property, filePathAttr);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndProperty();
    }

    private void SetPath(SerializedProperty property, FilePathAttribute filePathAttr)
    {
        string directory;
        if (filePathAttr.folder)
            directory = EditorUtility.OpenFolderPanel(filePathAttr.message, filePathAttr.projectPath, filePathAttr.initialNameOrFilter);
        else
            directory = EditorUtility.OpenFilePanel(filePathAttr.message, filePathAttr.projectPath, filePathAttr.initialNameOrFilter);

        // Canceled.
        if (string.IsNullOrEmpty(directory))
        {
            return;
        }

        // Normalize path separators.
        directory = Path.GetFullPath(directory);

        // If relative to project path, reduce the filepath to just what we need.
        if (directory.Contains(filePathAttr.projectPath))
            directory = directory.Replace(filePathAttr.projectPath, "");

        // Save setting.
        property.stringValue = directory;
    }
}

}
