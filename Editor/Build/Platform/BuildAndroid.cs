using System;
using System.Linq;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildAndroid : BuildPlatform
    {
        #region Constants

        private const string _name = "Android";
        private const string _binaryNameFormat = "{0}.apk";
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _deviceTypeVariantId = "Device Type";
        private const string _textureCompressionVariantId = "Texture Compression";
        private const string _buildSystemVariantId = "Build System";
        private const string _splitApksVariantId = "Split APKs";
        private const string _minSdkVersionVariantId = "Min SDK Version";

        private const string _androidApiLevelEnumPrefix = "AndroidApiLevel";

        #endregion

        public BuildAndroid()
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
                    new BuildArchitecture(BuildTarget.Android, "Android", true, _binaryNameFormat)
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new BuildVariant(_deviceTypeVariantId, EnumNamesToArray<AndroidArchitecture>()
                        .Skip(1)
                        .ToArray(),
                    0),
                    new BuildVariant(_textureCompressionVariantId, EnumNamesToArray<MobileTextureSubtarget>(), 0),
#if UNITY_2019_1_OR_NEWER
                    new BuildVariant(_buildSystemVariantId, new string[] { "Gradle" }, 0),
#else
                    new BuildVariant(_buildSystemVariantId, new string[] { "Internal", "Gradle" }, 0),
#endif
#if UNITY_2018_2_OR_NEWER
                    new BuildVariant(_splitApksVariantId, new string[] { "Disabled", "Enabled" }, 0),
#endif
                    new BuildVariant(_minSdkVersionVariantId, EnumNamesToArray<AndroidSdkVersions>()
                        .Select(i => i.Replace(_androidApiLevelEnumPrefix, ""))
                        .ToArray(),
                    0)
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
                    case _deviceTypeVariantId:
                        SetDeviceType(key);
                        break;
                    case _textureCompressionVariantId:
                        SetTextureCompression(key);
                        break;
                    case _buildSystemVariantId:
                        SetBuildSystem(key);
                        break;
#if UNITY_2018_2_OR_NEWER
                    case _splitApksVariantId:
                        SetSplitApks(key);
                        break;
#endif
                    case _minSdkVersionVariantId:
                        SetMinSdkVersion(key);
                        break;
                }
            }
        }

        private void SetDeviceType(string key)
        {
            PlayerSettings.Android.targetArchitectures = EnumValueFromKey<AndroidArchitecture>(key);
        }

        private void SetTextureCompression(string key)
        {
            EditorUserBuildSettings.androidBuildSubtarget
                = EnumValueFromKey<MobileTextureSubtarget>(key);
        }

        private void SetBuildSystem(string key)
        {
            EditorUserBuildSettings.androidBuildSystem
                = EnumValueFromKey<AndroidBuildSystem>(key);
        }

#if UNITY_2018_2_OR_NEWER
        private void SetSplitApks(string key)
        {
            PlayerSettings.Android.buildApkPerCpuArchitecture = key == "Enabled";
        }
#endif

        private void SetMinSdkVersion(string key)
        {
            PlayerSettings.Android.minSdkVersion
                = EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }
    }
}
