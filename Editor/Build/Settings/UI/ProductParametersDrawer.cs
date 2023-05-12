using System;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomPropertyDrawer(typeof(ProductParameters))]
    public class ProductParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            EditorGUILayout.BeginHorizontal();

            bool show = property.isExpanded;
            UnityBuildGUIUtility.DropdownHeader("Product Parameters", ref show, false, GUILayout.ExpandWidth(true));
            property.isExpanded = show;

            UnityBuildGUIUtility.HelpButton("Parameter-Details#product-parameters");
            EditorGUILayout.EndHorizontal();

            if (show)
            {
                EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

                // Temporarily override GUI label width size
                float currentLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200;

                SerializedProperty autoGenerate = property.FindPropertyRelative("autoGenerate");
                SerializedProperty syncWithPlayerSettings = property.FindPropertyRelative("syncWithPlayerSettings");

                EditorGUI.BeginDisabledGroup(syncWithPlayerSettings.boolValue);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("buildVersion"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(property.FindPropertyRelative("versionTemplate"));

                EditorGUILayout.PropertyField(property.FindPropertyRelative("autoGenerate"), new GUIContent("Auto-Generate Version"));
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(autoGenerate.boolValue);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("syncWithPlayerSettings"), new GUIContent("Sync Version with Player Settings"));
                EditorGUI.EndDisabledGroup();

                if (syncWithPlayerSettings.boolValue)
                {
                    property.FindPropertyRelative("versionTemplate").stringValue = PlayerSettings.bundleVersion;
                    property.FindPropertyRelative("buildVersion").stringValue = PlayerSettings.bundleVersion;
                }
                else
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("buildCounter"));

                    if (GUILayout.Button("Reset Build Counter", GUILayout.ExpandWidth(true)))
                        property.FindPropertyRelative("buildCounter").intValue = 0;
                }

                if (!autoGenerate.boolValue && !syncWithPlayerSettings.boolValue && GUILayout.Button("Generate Version Now", GUILayout.ExpandWidth(true)))
                {
                    BuildProject.GenerateVersionString(BuildSettings.productParameters, DateTime.Now);
                }

                property.serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndVertical();

                // Reset GUI label width size
                EditorGUIUtility.labelWidth = currentLabelWidth;
            }

            EditorGUI.EndProperty();
        }
    }
}
