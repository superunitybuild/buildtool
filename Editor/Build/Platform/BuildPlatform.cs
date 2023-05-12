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
        public bool enabled = false;
        public BuildDistributionList distributionList = new BuildDistributionList();
        public BuildArchitecture[] architectures = new BuildArchitecture[0];
        public BuildVariant[] variants = new BuildVariant[0];
        public BuildScriptingBackend[] scriptingBackends = new BuildScriptingBackend[0];

        public string platformName;
        public string dataDirNameFormat;
        public BuildTargetGroup targetGroup;

        public virtual void Init()
        {
        }

        public virtual void ApplyVariant()
        {
        }

        #region Public Properties

        public bool atLeastOneArch
        {
            get
            {
                bool atLeastOneArch = false;
                for (int i = 0; i < architectures.Length && !atLeastOneArch; i++)
                {
                    atLeastOneArch |= architectures[i].enabled;
                }

                return atLeastOneArch;
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

        public string variantKey
        {
            get
            {
                string retVal = "";

                // Build key string.
                if (variants != null && variants.Length > 0)
                {
                    foreach (var variant in variants)
                        retVal += variant.variantKey + ",";
                }

                // Remove trailing delimiter.
                if (retVal.Length > 0)
                    retVal = retVal.Substring(0, retVal.Length - 1);

                return retVal;
            }
        }

        #endregion

        public void Draw(SerializedObject obj)
        {
            EditorGUILayout.BeginVertical(UnityBuildGUIUtility.dropdownContentStyle);

            SerializedProperty archList = obj.FindProperty("architectures");

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
                    scriptProperty.serializedObject.ApplyModifiedProperties();
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

                    List<string> valueNames = new List<string>(variantValues.arraySize);
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
                    distName.stringValue = BuildProject.SanitizeFolderName(GUILayout.TextField(distName.stringValue));

                    if (UnityBuildGUIUtility.DeleteButton())
                        distList.SafeDeleteArrayElementAtIndex(i);

                    dist.serializedObject.ApplyModifiedProperties();

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

                addedProperty.serializedObject.ApplyModifiedProperties();
                distList.serializedObject.ApplyModifiedProperties();
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            obj.ApplyModifiedProperties();
        }

        public override string ToString()
        {
            string name = platformName;

            IEnumerable<BuildArchitecture> enabledArchitectures = architectures.Where(item => item.enabled);
            IEnumerable<BuildScriptingBackend> enabledscriptingBackends = scriptingBackends.Where(item => item.enabled);

            List<string> architecturesAndVariants = new List<string>();

            if (architectures.Length > 1 && enabledArchitectures.Count() > 0)
                architecturesAndVariants.AddRange(enabledArchitectures.Select(item => item.ToString()));

            if (scriptingBackends.Length > 1 && enabledscriptingBackends.Count() > 0)
                architecturesAndVariants.AddRange(enabledscriptingBackends.Select(item => item.ToString()));

            if (variants.Length > 0)
                architecturesAndVariants.AddRange(variants.Select(item => item.ToString()));

            name += architecturesAndVariants.Count > 0 ?
                " (" + string.Join(", ", architecturesAndVariants) + ")" :
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
