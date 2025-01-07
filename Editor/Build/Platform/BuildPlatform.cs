using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildPlatform : ScriptableObject
    {
        #region Constants

        protected const string PlayerName = "Player";
        protected const string ServerName = "Dedicated Server";

        protected const string ArchitectureVariantKey = "Architecture";
        protected const string BuildOutputVariantKey = "Build Output";
        protected const string BuildTypeVariantKey = "Build Type";

        #endregion

        public bool enabled = false;
        public BuildDistributionList distributionList = new();
        public BuildTarget[] targets = new BuildTarget[0];
        public BuildVariant[] variants = new BuildVariant[0];
        public BuildScriptingBackend[] scriptingBackends = new BuildScriptingBackend[0];

        public string platformName;
        public BuildTargetGroup targetGroup;

        public virtual void Init()
        {
        }

        public virtual void ApplyVariant()
        {
        }

        #region Public Properties

        public bool atLeastOneTarget
        {
            get
            {
                bool atLeastOneTarget = false;
                for (int i = 0; i < targets.Length && !atLeastOneTarget; i++)
                {
                    atLeastOneTarget |= targets[i].enabled;
                }

                return atLeastOneTarget;
            }
        }

        public bool atLeastOneBackend
        {
            get
            {
                if (scriptingBackends.Length <= 0)
                {
                    return true;
                }

                bool atLeastOneBackend = false;
                for (int i = 0; i < scriptingBackends.Length && !atLeastOneBackend; i++)
                {
                    atLeastOneBackend |= scriptingBackends[i].enabled;
                }

                return atLeastOneBackend;
            }
        }

        public bool atLeastOneDistribution
        {
            get
            {
                bool atLeastOneDist = false;
                for (int i = 0; i < distributionList.distributions.Length && !atLeastOneDist; i++)
                {
                    atLeastOneDist |= distributionList.distributions[i].enabled;
                }

                return atLeastOneDist;
            }
        }

        public virtual bool hasAdditionalOptions => false;

        public string variantKey
        {
            get
            {
                string retVal = "";

                // Build key string.
                if (variants != null && variants.Length > 0)
                {
                    foreach (BuildVariant variant in variants)
                        retVal += variant.variantKey + ",";
                }

                // Remove trailing delimiter.
                if (retVal.Length > 0)
                    retVal = retVal[..^1];

                return retVal;
            }
        }

        #endregion

        public void Draw(SerializedObject obj)
        {
            _ = EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            SerializedProperty targets = obj.FindProperty("targets");

            if (targets.arraySize > 1)
            {
                GUILayout.Label("Targets", UnityBuildGUIUtility.midHeaderStyle);
                for (int i = 0; i < targets.arraySize; i++)
                {
                    SerializedProperty targetProperty = targets.GetArrayElementAtIndex(i);
                    SerializedProperty targetName = targetProperty.FindPropertyRelative("name");
                    SerializedProperty targetEnabled = targetProperty.FindPropertyRelative("enabled");

                    targetEnabled.boolValue = GUILayout.Toggle(targetEnabled.boolValue, targetName.stringValue);
                    _ = targetProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            SerializedProperty scriptingBackendList = obj.FindProperty("scriptingBackends");

            if (scriptingBackendList.arraySize > 1)
            {
                GUILayout.Label("Scripting Backends", UnityBuildGUIUtility.midHeaderStyle);
                for (int i = 0; i < scriptingBackendList.arraySize; i++)
                {
                    SerializedProperty scriptProperty = scriptingBackendList.GetArrayElementAtIndex(i);
                    SerializedProperty scriptName = scriptProperty.FindPropertyRelative("name");
                    SerializedProperty scriptEnabled = scriptProperty.FindPropertyRelative("enabled");

                    scriptEnabled.boolValue = GUILayout.Toggle(scriptEnabled.boolValue, scriptName.stringValue);
                    _ = scriptProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            SerializedProperty variantList = obj.FindProperty("variants");

            if (variantList.arraySize > 0)
            {
                GUILayout.Label("Variant Options", UnityBuildGUIUtility.midHeaderStyle);

                for (int i = 0; i < variantList.arraySize; i++)
                {
                    SerializedProperty variantProperty = variantList.GetArrayElementAtIndex(i);
                    SerializedProperty variantName = variantProperty.FindPropertyRelative("variantName");
                    SerializedProperty variantValues = variantProperty.FindPropertyRelative("values");
                    SerializedProperty selectedVariantIndex = variantProperty.FindPropertyRelative("selectedIndex");
                    SerializedProperty isFlag = variantProperty.FindPropertyRelative("isFlag");

                    List<string> valueNames = new(variantValues.arraySize);
                    for (int j = 0; j < variantValues.arraySize; j++)
                    {
                        valueNames.Add(variantValues.GetArrayElementAtIndex(j).stringValue);
                    }

                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(variantName.stringValue);

                    if (isFlag.boolValue)
                    {
                        // Don't allow 'Nothing' to be selected
                        int selected = selectedVariantIndex.intValue > 0 ? selectedVariantIndex.intValue : -1;

                        selectedVariantIndex.intValue =
                            EditorGUILayout.MaskField(selected, valueNames.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
                    }
                    else
                    {
                        selectedVariantIndex.intValue =
                            EditorGUILayout.Popup(selectedVariantIndex.intValue, valueNames.ToArray(), UnityBuildGUIUtility.popupStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(250));
                    }

                    GUILayout.EndHorizontal();
                }
            }

            if (hasAdditionalOptions)
            {
                GUILayout.Label("Additional Options", UnityBuildGUIUtility.midHeaderStyle);

                DrawAdditionalOptions();
            }

            SerializedProperty distList = obj.FindProperty("distributionList.distributions");

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
                    distName.stringValue = GUILayout.TextField(distName.stringValue.SanitizeFolderName());

                    if (UnityBuildGUIUtility.DeleteButton())
                        distList.SafeDeleteArrayElementAtIndex(i);

                    _ = dist.serializedObject.ApplyModifiedProperties();

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);
            GUILayout.BeginVertical();

            if (GUILayout.Button("Add Distribution", GUILayout.ExpandWidth(true)))
            {
                int addedIndex = distList.arraySize;
                distList.InsertArrayElementAtIndex(addedIndex);

                SerializedProperty addedProperty = distList.GetArrayElementAtIndex(addedIndex);
                addedProperty.FindPropertyRelative("enabled").boolValue = true;
                addedProperty.FindPropertyRelative("distributionName").stringValue = "DistributionName";

                _ = addedProperty.serializedObject.ApplyModifiedProperties();
                _ = distList.serializedObject.ApplyModifiedProperties();
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            _ = obj.ApplyModifiedProperties();
        }

        public virtual void DrawAdditionalOptions()
        {
            /// Override this method to draw additional options for the platform
            /// Requires overriding <see cref="hasAdditionalOptions"/> to return true
        }

        public override string ToString()
        {
            string name = platformName;

            IEnumerable<BuildTarget> enabledTargets = targets.Where(item => item.enabled);
            IEnumerable<BuildScriptingBackend> enabledScriptingBackends = scriptingBackends.Where(item => item.enabled);

            List<string> targetsAndVariants = new();

            if (targets.Length > 1 && enabledTargets.Count() > 0)
                targetsAndVariants.AddRange(enabledTargets.Select(item => item.ToString()));

            if (scriptingBackends.Length > 1 && enabledScriptingBackends.Count() > 0)
                targetsAndVariants.AddRange(enabledScriptingBackends.Select(item => item.ToString()));

            if (variants.Length > 0)
                targetsAndVariants.AddRange(variants.Select(item => item.ToString()));

            name += targetsAndVariants.Count > 0 ?
                " (" + string.Join(", ", targetsAndVariants) + ")" :
                "";

            return name;
        }

        protected static T EnumFlagValueFromKey<T>(string label) where T : Enum
        {
            long result = 0;
            label = label.Replace(" ", "");
            foreach (string split in label.Split('+'))
            {
                result |= Convert.ToInt64((T)Enum.Parse(typeof(T), split));
            }
            return (T)Enum.ToObject(typeof(T), result);
        }

        protected static T EnumValueFromKey<T>(string label)
        {
            return (T)Enum.Parse(typeof(T), label.Replace(" ", ""));
        }

        protected static string[] EnumNamesToArray<T>(bool toWords = false)
        {
            return Enum.GetNames(typeof(T))
                .Select(item => toWords ? UnityBuildGUIUtility.ToWords(item) : item)
                .ToArray();
        }
    }
}
