using System;
using System.Collections.Generic;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildMac : BuildPlatform
    {
        #region Constants

        private const string _name = "macOS";
        private readonly Dictionary<BuildOutputType, string> _binaryNameFormats = new()
        {
            {BuildOutputType.App, "{0}.app"},
            {BuildOutputType.XcodeProject, "{0}"},
        };
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        private const string ArchitectureIntel64BitId = "Intel 64-bit";
        private const string ArchitectureAppleSiliconId = "Apple silicon";
        private const string ArchitectureUniversalId = "Intel 64-bit + Apple silicon";

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

        public BuildMac()
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
                    new(UnityEditor.BuildTarget.StandaloneOSX, PlayerName, true, _binaryNameFormats[0]),
                    new(UnityEditor.BuildTarget.StandaloneOSX, ServerName, false, _binaryNameFormats[0], (int)StandaloneBuildSubtarget.Server),
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new(ScriptingImplementation.Mono2x, true),
                    new(ScriptingImplementation.IL2CPP, false),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new(ArchitectureVariantKey, new string[] { ArchitectureIntel64BitId, ArchitectureAppleSiliconId, ArchitectureUniversalId }, 0),
                    new(BuildOutputVariantKey, EnumNamesToArray<BuildOutputType>(true), 0)
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
                    case ArchitectureVariantKey:
                        SetArchitecture(key);
                        break;
                    case BuildOutputVariantKey:
                        SetBuildOutput(key);
                        break;
                }
            }
        }

        private void SetArchitecture(string key)
        {
            MacOSArchitecture architecture = key switch
            {
                ArchitectureIntel64BitId => MacOSArchitecture.Intelx64,
                ArchitectureAppleSiliconId => MacOSArchitecture.AppleSilicon,
                ArchitectureUniversalId or _ => MacOSArchitecture.Universal,
            };


#if UNITY_STANDALONE_OSX
            UnityEditor.OSXStandalone.UserBuildSettings.architecture =
#if UNITY_2022_1_OR_NEWER
                (UnityEditor.Build.OSArchitecture)
#else
                (UnityEditor.OSXStandalone.MacOSArchitecture)
#endif
                    architecture;
#endif
        }

        private void SetBuildOutput(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

#if UNITY_STANDALONE_OSX
            UnityEditor.OSXStandalone.UserBuildSettings.createXcodeProject = outputType == BuildOutputType.XcodeProject;
#endif

            targets[0].binaryNameFormat = _binaryNameFormats[outputType];
        }
    }
}
