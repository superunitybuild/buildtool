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
        private Dictionary<BinaryType, string> _binaryNameFormats = new Dictionary<BinaryType, string>{
            {BinaryType.APK, "{0}.apk"},
            {BinaryType.SplitAPK, "{0}"},
            {BinaryType.AAB, "{0}.aab"},
        };
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _apkExpansionFilesTypeVariantId = "APK Expansion Type";
        private const string _buildOutputTypeVariantId = "Build Output";
        private const string _binaryTypeVariantId = "Binary Type";
        private const string _deviceTypeVariantId = "Device Type";
        private const string _textureCompressionVariantId = "Texture Compression";
        private const string _minSdkVersionVariantId = "Min SDK Version";
        private const string _targetSdkVersionVariantId = "Target SDK Version";

        private const string _androidApiLevelEnumPrefix = "AndroidApiLevel";

        private enum ApkExpansionFilesType
        {
            SingleBinary,
            SplitAppBinary,
        }

        private enum BuildOutputType
        {
            App,
            GradleProject
        }

        private enum BinaryType
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
                string[] androidSdkVersionStrings = EnumNamesToArray<AndroidSdkVersions>()
                    .Select(i => i.Replace(_androidApiLevelEnumPrefix, ""))
                    .ToArray();

                variants = new BuildVariant[] {
                    new BuildVariant(_deviceTypeVariantId, EnumNamesToArray<AndroidArchitecture>()
                        .Skip(1)
                        .ToArray(),
                    0),
                    new BuildVariant(_textureCompressionVariantId, EnumNamesToArray<MobileTextureSubtarget>(), 0),
                    new BuildVariant(_buildOutputTypeVariantId, EnumNamesToArray<BuildOutputType>(true), 0),
                    new BuildVariant(_binaryTypeVariantId, EnumNamesToArray<BinaryType>(true), 0),
                    new BuildVariant(_apkExpansionFilesTypeVariantId, EnumNamesToArray<ApkExpansionFilesType>(true), 0),
                    new BuildVariant(_minSdkVersionVariantId, androidSdkVersionStrings, 0),
                    new BuildVariant(_targetSdkVersionVariantId, androidSdkVersionStrings, 0),
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
                    case _apkExpansionFilesTypeVariantId:
                        SetApkExpansionFilesType(key);
                        break;
                    case _binaryTypeVariantId:
                        SetBinaryType(key);
                        break;
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
                    case _targetSdkVersionVariantId:
                        SetTargetSdkVersion(key);
                        break;
                }
            }
        }

        private AndroidSdkVersions GetAndroidSdkVersionFromKey(string key)
        {
            return EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }

        private void SetBinaryType(string key)
        {
            BinaryType binaryType = EnumValueFromKey<BinaryType>(key);

            bool buildAppBundle = binaryType == BinaryType.AAB;
            bool splitAPK = binaryType == BinaryType.SplitAPK;

            EditorUserBuildSettings.buildAppBundle = buildAppBundle;
            PlayerSettings.Android.buildApkPerCpuArchitecture = splitAPK && !buildAppBundle;

            architectures[0].binaryNameFormat = _binaryNameFormats[binaryType];
        }

        private void SetBuildOutputType(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

            bool exportProject = outputType == BuildOutputType.GradleProject;

            EditorUserBuildSettings.exportAsGoogleAndroidProject = exportProject;

            // Override binary name format set by Binary Type variant if exporting Gradle project
            if (exportProject)
                architectures[0].binaryNameFormat = "{0}";
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
            PlayerSettings.Android.minSdkVersion = GetAndroidSdkVersionFromKey(key);
        }

        private void SetTargetSdkVersion(string key)
        {
            PlayerSettings.Android.targetSdkVersion = GetAndroidSdkVersionFromKey(key);
        }

        private void SetApkExpansionFilesType(string key)
        {
            ApkExpansionFilesType expansionFilesType = EnumValueFromKey<ApkExpansionFilesType>(key);

            PlayerSettings.Android.useAPKExpansionFiles = expansionFilesType == ApkExpansionFilesType.SplitAppBinary;
        }
    }
}
