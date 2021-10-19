using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildAndroid : BuildPlatform
    {
        #region Constants

        private const string _name = "Android";
        private Dictionary<BuildOutputType, string> _binaryNameFormats = new Dictionary<BuildOutputType, string>{
            {BuildOutputType.APK, "{0}.apk"},
            {BuildOutputType.SplitAPK, "{0}"},
            {BuildOutputType.AAB, "{0}.aab"},
        };
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _buildOutputTypeVariantId = "Build Output";
        private const string _deviceTypeVariantId = "Device Type";
        private const string _textureCompressionVariantId = "Texture Compression";
        private const string _minSdkVersionVariantId = "Min SDK Version";

        private const string _androidApiLevelEnumPrefix = "AndroidApiLevel";

        private enum BuildOutputType
        {
            APK,
            SplitAPK,
            AAB
        }
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
                    new BuildArchitecture(BuildTarget.Android, "Android", true, _binaryNameFormats[0])
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
                    new BuildVariant(_buildOutputTypeVariantId, EnumNamesToArray<BuildOutputType>(true), 0),
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
                    case _buildOutputTypeVariantId:
                        SetBuildOutputType(key);
                        break;
                    case _deviceTypeVariantId:
                        SetDeviceType(key);
                        break;
                    case _textureCompressionVariantId:
                        SetTextureCompression(key);
                        break;
                    case _minSdkVersionVariantId:
                        SetMinSdkVersion(key);
                        break;
                }
            }
        }

        private void SetBuildOutputType(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

            bool buildAppBundle = outputType == BuildOutputType.AAB;
            bool splitAPK = outputType == BuildOutputType.SplitAPK;

            EditorUserBuildSettings.buildAppBundle = buildAppBundle;
            PlayerSettings.Android.buildApkPerCpuArchitecture = splitAPK && !buildAppBundle;

            architectures[0].binaryNameFormat = _binaryNameFormats[outputType];
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

        private void SetMinSdkVersion(string key)
        {
            PlayerSettings.Android.minSdkVersion
                = EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }
    }
}
