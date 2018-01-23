using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[CustomPropertyDrawer(typeof(BuildPlatform))]
public class BuildPlatformDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        bool show = property.isExpanded;
        UnityBuildGUIUtility.DropdownHeader(property.FindPropertyRelative("platformName").stringValue, ref show, false);
        property.isExpanded = show;

        if (show)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            SerializedProperty archList = property.FindPropertyRelative("architectures");

            if (archList.arraySize > 1)
            {
                GUILayout.Label("Architectures", UnityBuildGUIUtility.midHeaderStyle);
                for (int i = 0; i < archList.arraySize; i++)
                {
                    SerializedProperty archProperty = archList.GetArrayElementAtIndex(i);
                    SerializedProperty archName = archProperty.FindPropertyRelative("name");
                    SerializedProperty archEnabled = archProperty.FindPropertyRelative("enabled");

                    archEnabled.boolValue = GUILayout.Toggle(archEnabled.boolValue, archName.stringValue);
                    archProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            SerializedProperty variantList = property.FindPropertyRelative("variants");

            if (variantList.arraySize > 0)
            {
                GUILayout.Label("Variant Options", UnityBuildGUIUtility.midHeaderStyle);

                for (int i = 0; i < variantList.arraySize; i++)
                {
                    SerializedProperty variantProperty = variantList.GetArrayElementAtIndex(i);
                    SerializedProperty variantName = variantProperty.FindPropertyRelative("variantName");
                    SerializedProperty variantValues = variantProperty.FindPropertyRelative("values");
                    SerializedProperty selectedVariantIndex = variantProperty.FindPropertyRelative("selectedIndex");

                    List<string> valueNames = new List<string>(variantValues.arraySize);
                    for (int j = 0; j < variantValues.arraySize; j++)
                    {
                        valueNames.Add(variantValues.GetArrayElementAtIndex(j).stringValue);
                    }

                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(variantName.stringValue);
                    selectedVariantIndex.intValue =
                        EditorGUILayout.Popup(selectedVariantIndex.intValue, valueNames.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));

                    GUILayout.EndHorizontal();
                }
            }

            SerializedProperty distList = property.FindPropertyRelative("distributionList.distributions");

            if (distList.arraySize > 0)
            {
                GUILayout.Label("Distributions", UnityBuildGUIUtility.midHeaderStyle);

                for (int i = 0; i < distList.arraySize; i++)
                {
                    SerializedProperty dist = distList.GetArrayElementAtIndex(i);
                    SerializedProperty distEnabled = dist.FindPropertyRelative("enabled");
                    SerializedProperty distName = dist.FindPropertyRelative("distributionName");

                    GUILayout.BeginHorizontal();

                    distEnabled.boolValue = GUILayout.Toggle(distEnabled.boolValue, GUIContent.none, GUILayout.ExpandWidth(false));
                    distName.stringValue = BuildProject.SanitizeFolderName(GUILayout.TextField(distName.stringValue));

                    if (GUILayout.Button("X", UnityBuildGUIUtility.helpButtonStyle))
                    {
                        distList.DeleteArrayElementAtIndex(i);
                    }

                    dist.serializedObject.ApplyModifiedProperties();

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Add Distribution", GUILayout.MaxWidth(150)))
            {
                int addedIndex = distList.arraySize;
                distList.InsertArrayElementAtIndex(addedIndex);

                SerializedProperty addedProperty = distList.GetArrayElementAtIndex(addedIndex);
                addedProperty.FindPropertyRelative("enabled").boolValue = true;
                addedProperty.FindPropertyRelative("distributionName").stringValue = "DistributionName";

                addedProperty.serializedObject.ApplyModifiedProperties();
                distList.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
                GUIUtility.keyboardControl = 0;
            }
            if (GUILayout.Button("Delete Platform", GUILayout.MaxWidth(150)))
            {
                property.FindPropertyRelative("enabled").boolValue = false;
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }
}

}