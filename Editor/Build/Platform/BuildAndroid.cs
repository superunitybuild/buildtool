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
        private readonly Dictionary<BinaryType, string> _binaryNameFormats = new()
        {
            {BinaryType.APK, "{0}.apk"},
            {BinaryType.SplitAPK, "{0}"},
            {BinaryType.AAB, "{0}.aab"},
        };
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

        private const string _apkExpansionFilesTypeVariantId = "APK Expansion Type";
        private const string _binaryTypeVariantId = "Binary Type";
        private const string _createSymbolsVariantId = "Create symbols.zip";
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
            targetGroup = _targetGroup;

            if (targets == null || targets.Length == 0)
            {
                targets = new BuildTarget[] {
                    new(UnityEditor.BuildTarget.Android, PlayerName, true, _binaryNameFormats[0])
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new(ScriptingImplementation.Mono2x, false),
                    new(ScriptingImplementation.IL2CPP, true),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                string[] androidSdkVersionStrings = EnumNamesToArray<AndroidSdkVersions>()
                    .Select(i => i.Replace(_androidApiLevelEnumPrefix, ""))
                    .ToArray();

                string[] createSymbolsOptions = EnumNamesToArray<AndroidCreateSymbols>().ToArray();

                variants = new BuildVariant[] {
                    new(ArchitectureVariantKey, EnumNamesToArray<AndroidArchitecture>()
                        .Skip(1)
                        .SkipLast()
                        .ToArray(),
                    3, true),
                    new(_textureCompressionVariantId, EnumNamesToArray<MobileTextureSubtarget>(), 0),
                    new(BuildOutputVariantKey, EnumNamesToArray<BuildOutputType>(true), 0),
                    new(_binaryTypeVariantId, EnumNamesToArray<BinaryType>(true), 0),
                    new(_apkExpansionFilesTypeVariantId, EnumNamesToArray<ApkExpansionFilesType>(true), 0),
                    new(_minSdkVersionVariantId, androidSdkVersionStrings, 0),
                    new(_targetSdkVersionVariantId, androidSdkVersionStrings, 0),
                    new(_createSymbolsVariantId, createSymbolsOptions, 0),
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
                    case _apkExpansionFilesTypeVariantId:
                        SetApkExpansionFilesType(key);
                        break;
                    case ArchitectureVariantKey:
                        SetArchitecture(key);
                        break;
                    case _binaryTypeVariantId:
                        SetBinaryType(key);
                        break;
                    case BuildOutputVariantKey:
                        SetBuildOutput(key);
                        break;
                    case _createSymbolsVariantId:
                        SetCreateSymbols(key);
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

        private UnityEditor.AndroidSdkVersions GetAndroidSdkVersionFromKey(string key)
        {
            return (UnityEditor.AndroidSdkVersions)EnumValueFromKey<AndroidSdkVersions>(_androidApiLevelEnumPrefix + key);
        }

        private void SetApkExpansionFilesType(string key)
        {
            ApkExpansionFilesType expansionFilesType = EnumValueFromKey<ApkExpansionFilesType>(key);

#if UNITY_2023_1_OR_NEWER
            PlayerSettings.Android.splitApplicationBinary = expansionFilesType == ApkExpansionFilesType.SplitAppBinary;
#else
            PlayerSettings.Android.useAPKExpansionFiles = expansionFilesType == ApkExpansionFilesType.SplitAppBinary;
#endif
        }

        private void SetArchitecture(string key)
        {
            PlayerSettings.Android.targetArchitectures = EnumFlagValueFromKey<AndroidArchitecture>(key);
        }

        private void SetBinaryType(string key)
        {
            BinaryType binaryType = EnumValueFromKey<BinaryType>(key);

            bool buildAppBundle = binaryType == BinaryType.AAB;
            bool splitAPK = binaryType == BinaryType.SplitAPK;

            EditorUserBuildSettings.buildAppBundle = buildAppBundle;
            PlayerSettings.Android.buildApkPerCpuArchitecture = splitAPK && !buildAppBundle;

            targets[0].binaryNameFormat = _binaryNameFormats[binaryType];
        }

        private void SetBuildOutput(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

            bool exportProject = outputType == BuildOutputType.GradleProject;

            EditorUserBuildSettings.exportAsGoogleAndroidProject = exportProject;

            // Override binary name format set by Binary Type variant if exporting Gradle project
            if (exportProject)
                targets[0].binaryNameFormat = "{0}";
        }

        private void SetCreateSymbols(string key)
        {
            EditorUserBuildSettings.androidCreateSymbols = EnumValueFromKey<AndroidCreateSymbols>(key);
        }

        private void SetMinSdkVersion(string key)
        {
            PlayerSettings.Android.minSdkVersion = GetAndroidSdkVersionFromKey(key);
        }

        private void SetTargetSdkVersion(string key)
        {
            PlayerSettings.Android.targetSdkVersion = GetAndroidSdkVersionFromKey(key);
        }

        private void SetTextureCompression(string key)
        {
            EditorUserBuildSettings.androidBuildSubtarget = EnumValueFromKey<MobileTextureSubtarget>(key);
        }
    }

    /// <summary>
    /// Fork of <see cref="UnityEditor.AndroidSdkVersions"/> to enable SuperUnityBuild to be kept up-to-date with
    /// Android API levels, independent of Unity Editor releases
    /// </summary>
    public enum AndroidSdkVersions
    {
        /// <summary>
        /// Sets the target API level automatically, according to the latest installed SDK
        /// on your computer.
        /// </summary>
        AndroidApiLevelAuto = 0,
        /// <summary>
        /// Android 4.1, "Jelly Bean", API level 16.
        /// </summary>
        [Obsolete("Minimum supported Android API level is 22 (Android 5.1 Lollipop). Please use AndroidApiLevel22 or higher", true)]
        AndroidApiLevel16 = 16,
        /// <summary>
        /// Android 4.2, "Jelly Bean", API level 17.
        /// </summary>
        [Obsolete("Minimum supported Android API level is 22 (Android 5.1 Lollipop). Please use AndroidApiLevel22 or higher", true)]
        AndroidApiLevel17 = 17,
        /// <summary>
        /// Android 4.3, "Jelly Bean", API level 18.
        /// </summary>
        [Obsolete("Minimum supported Android API level is 22 (Android 5.1 Lollipop). Please use AndroidApiLevel22 or higher", true)]
        AndroidApiLevel18 = 18,
        /// <summary>
        /// Android 4.4, "KitKat", API level 19.
        /// </summary>
        [Obsolete("Minimum supported Android API level is 22 (Android 5.1 Lollipop). Please use AndroidApiLevel22 or higher", true)]
        AndroidApiLevel19 = 19,
        /// <summary>
        /// Android 5.0, "Lollipop", API level 21.
        /// </summary>
        [Obsolete("Minimum supported Android API level is 22 (Android 5.1 Lollipop). Please use AndroidApiLevel22 or higher", true)]
        AndroidApiLevel21 = 21,
        /// <summary>
        /// Android 5.1, "Lollipop", API level 22.
        /// </summary>
        AndroidApiLevel22 = 22,
        /// <summary>
        /// Android 6.0, "Marshmallow", API level 23.
        /// </summary>
        AndroidApiLevel23 = 23,
        /// <summary>
        /// Android 7.0, "Nougat", API level 24.
        /// </summary>
        AndroidApiLevel24 = 24,
        /// <summary>
        /// Android 7.1, "Nougat", API level 25.
        /// </summary>
        AndroidApiLevel25 = 25,
        /// <summary>
        /// Android 8.0, "Oreo", API level 26.
        /// </summary>
        AndroidApiLevel26 = 26,
        /// <summary>
        /// Android 8.1, "Oreo", API level 27.
        /// </summary>
        AndroidApiLevel27 = 27,
        /// <summary>
        /// Android 9.0, "Pie", API level 28.
        /// </summary>
        AndroidApiLevel28 = 28,
        /// <summary>
        /// Android 10.0, "Android Q", API level 29.
        /// </summary>
        AndroidApiLevel29 = 29,
        /// <summary>
        /// Android 11.0, "Red Velvet Cake", API level 30.
        /// </summary>
        AndroidApiLevel30 = 30,
        /// <summary>
        /// Android 12.0, "Snow Cone", API level 31.
        /// </summary>
        AndroidApiLevel31 = 31,
        /// <summary>
        /// Android 12.1, "Android 12L", API level 32.
        /// </summary>
        AndroidApiLevel32 = 32,
        /// <summary>
        /// Android 13.0, "Tiramisu", API level 33.
        /// </summary>
        AndroidApiLevel33 = 33,
        /// <summary>
        /// Android 14.0, "Upside Down Cake", API level 34.
        /// </summary>
        AndroidApiLevel34 = 34,
        /// <summary>
        /// Android 15.0, "Vanilla Ice Cream", API level 35.
        /// </summary>
        AndroidApiLevel35 = 35
    }
}
