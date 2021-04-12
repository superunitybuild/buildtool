using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildIOS : BuildPlatform
    {
        #region Constants

        private const string _name = "iOS";
        private const string _binaryNameFormat = "{0}";
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

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new BuildVariant(_deviceTypeVariantId, EnumNamesToArray<iOSTargetDevice>(), 0),
                    new BuildVariant(_sdkVersionVariantId, EnumNamesToArray<iOSSdkVersion>(true), 0),
                    new BuildVariant(_buildConfigTypeVariantId, EnumNamesToArray<iOSBuildType>(), 0),
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
            EditorUserBuildSettings.iOSBuildConfigType = EnumValueFromKey<iOSBuildType>(key);
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
