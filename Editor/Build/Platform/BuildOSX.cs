using System;
using System.Collections.Generic;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildOSX : BuildPlatform
    {
        #region Constants

        private const string _name = "macOS";
        private Dictionary<BuildOutputType, string> _binaryNameFormats = new Dictionary<BuildOutputType, string>{
            {BuildOutputType.App, "{0}.app"},
            {BuildOutputType.XcodeProject, "{0}.xcodeproj"},
        };
        private const string _dataDirNameFormat = "{0}.app/Contents";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        private const string _buildOutputTypeVariantId = "Build Output";
        private const string _macOSArchitectureVariantId = "macOS Architecture";

        private enum BuildOutputType
        {
            App,
            XcodeProject
        }

        // We need to define our own enum, as UnityEditor.OSXStandalone.MacOSArchitecture
        // is only available when the Mac Build module is installed
        private enum MacOSArchitecture
        {
            Intelx64,
            AppleSilicon,
            Universal
        }

        #endregion

        public BuildOSX()
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
                    new BuildArchitecture(BuildTarget.StandaloneOSX, "macOS", true, _binaryNameFormats[0]),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
#if UNITY_2020_2_OR_NEWER
                    new BuildVariant(_macOSArchitectureVariantId, EnumNamesToArray<MacOSArchitecture>(true), 0),
#endif
                    new BuildVariant(_buildOutputTypeVariantId, EnumNamesToArray<BuildOutputType>(true), 0)
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
                    case _macOSArchitectureVariantId:
                        SetMacOSArchitecture(key);
                        break;
                }
            }
        }

        private void SetBuildOutputType(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

#if UNITY_STANDALONE_OSX
            UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = outputType == BuildOutputType.XcodeProject;
#endif

            architectures[0].binaryNameFormat = _binaryNameFormats[outputType];
        }

        private void SetMacOSArchitecture(string key)
        {
#if UNITY_STANDALONE_OSX
#if UNITY_2022_1_OR_NEWER
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = (UnityEditor.Build.OSArchitecture)EnumValueFromKey<MacOSArchitecture>(key);
#elif UNITY_2020_2_OR_NEWER
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = (UnityEditor.OSXStandalone.MacOSArchitecture)EnumValueFromKey<MacOSArchitecture>(key);
#endif
#endif
        }
    }
}
