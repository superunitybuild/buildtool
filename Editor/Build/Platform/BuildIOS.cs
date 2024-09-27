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
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.iOS;

        private const string _deviceTypeVariantId = "Device Type";
        private const string _sdkVersionVariantId = "Target SDK";

        #endregion

        public BuildIOS()
        {
            enabled = false;
            Init();
        }

        public override void Init()
        {
            platformName = _name;
            targetGroup = _targetGroup;

            if (targets == null || targets.Length == 0)
            {
                targets = new BuildTarget[] {
                    new(UnityEditor.BuildTarget.iOS, PlayerName, true, _binaryNameFormat)
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new(ScriptingImplementation.IL2CPP, true),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new(_deviceTypeVariantId, EnumNamesToArray<iOSTargetDevice>(), 0),
                    new(_sdkVersionVariantId, EnumNamesToArray<iOSSdkVersion>(true), 0),
                    new(BuildTypeVariantKey, EnumNamesToArray<XcodeBuildConfig>(), 0),
                };
            }
        }

        public override void ApplyVariant()
        {
            foreach (BuildVariant variantOption in variants)
            {
                string key = variantOption.variantKey;

                switch (variantOption.variantName)
                {
                    case BuildTypeVariantKey:
                        SetBuildType(key);
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

        private void SetBuildType(string key)
        {
            EditorUserBuildSettings.iOSXcodeBuildConfig = EnumValueFromKey<XcodeBuildConfig>(key);
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
