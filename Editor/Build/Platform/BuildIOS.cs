using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildIOS : BuildPlatform
    {
        #region Constants

        private const string _name = "iOS";
        private const string _binaryNameFormat = "";
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.iOS;

        private const string _deviceTypeVariantId = "Device Type";
        private const string _sdkVersionVariantId = "Target SDK";
        private const string _buildConfigTypeVariantId = "Build Type";

        #endregion

        public BuildIOS()
        {
            enabled = false;
            Init();
        }

        public override void Init()
        {
            platformName = _name;
            dataDirNameFormat = _dataDirNameFormat;
            targetGroup = _targetGroup;

            if (architectures == null || architectures.Length == 0)
            {
                architectures = new BuildArchitecture[] {
                    new BuildArchitecture(BuildTarget.iOS, "iOS", true, _binaryNameFormat)
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new BuildScriptingBackend(ScriptingImplementation.IL2CPP, true),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new BuildVariant(_deviceTypeVariantId, EnumNamesToArray<iOSTargetDevice>(), 0),
                    new BuildVariant(_sdkVersionVariantId, EnumNamesToArray<iOSSdkVersion>(true), 0),
                    #if UNITY_2021_2_OR_NEWER
                    new BuildVariant(_buildConfigTypeVariantId, EnumNamesToArray<XcodeBuildConfig>(), 0),
                    #else
                    new BuildVariant(_buildConfigTypeVariantId, EnumNamesToArray<iOSBuildType>(), 0),
                    #endif
                };
            }
        }

        public override void ApplyVariant()
        {
            foreach (var variantOption in variants)
            {
                string key = variantOption.variantKey;

                switch (variantOption.variantName)
                {
                    case _buildConfigTypeVariantId:
                        SetBuildConfigType(key);
                        break;
                    case _deviceTypeVariantId:
                        SetDeviceType(key);
                        break;
                    case _sdkVersionVariantId:
                        SetSdkVersion(key);
                        break;
                }
            }
        }

        private void SetBuildConfigType(string key)
        {
#if UNITY_2021_2_OR_NEWER
            EditorUserBuildSettings.iOSXcodeBuildConfig = EnumValueFromKey<XcodeBuildConfig>(key);
#else
            EditorUserBuildSettings.iOSBuildConfigType = EnumValueFromKey<iOSBuildType>(key);
#endif
        }

        private void SetDeviceType(string key)
        {
            PlayerSettings.iOS.targetDevice = EnumValueFromKey<iOSTargetDevice>(key);
        }

        private void SetSdkVersion(string key)
        {
            PlayerSettings.iOS.sdkVersion = EnumValueFromKey<iOSSdkVersion>(key);
        }
    }
}
