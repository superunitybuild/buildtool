using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildOSX : BuildPlatform
    {
        // We need to define our own enum, as UnityEditor.OSXStandalone.MacOSArchitecture
        // is only available when the Mac Build module is installed
        private enum MacOSArchitecture
        {
            Intelx64,
            AppleSilicon,
            Universal
        }

        #region Constants

        private const string _name = "macOS";
        private const string _binaryNameFormat = "{0}.app";
        private const string _dataDirNameFormat = "{0}.app/Contents";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        private const string _macOSArchitectureVariantId = "macOS Architecture";

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
                    new BuildArchitecture(BuildTarget.StandaloneOSX, "macOS", true, _binaryNameFormat),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
#if UNITY_2020_2_OR_NEWER
                    new BuildVariant(_macOSArchitectureVariantId, EnumNamesToArray<MacOSArchitecture>(true), 0),
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
                    case _macOSArchitectureVariantId:
                        SetMacOSArchitecture(key);
                        break;
                }
            }
        }

        private void SetMacOSArchitecture(string key)
        {
#if UNITY_2020_2_OR_NEWER && UNITY_STANDALONE_OSX
            UnityEditor.OSXStandalone.UserBuildSettings.architecture = (UnityEditor.OSXStandalone.MacOSArchitecture)EnumValueFromKey<MacOSArchitecture>(key);
#endif
        }
    }
}
