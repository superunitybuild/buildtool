using System;
using System.Collections.Generic;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildWindows : BuildPlatform
    {
        #region Constants

        private const string _name = "Windows";
        private readonly Dictionary<BuildOutputType, string> _binaryNameFormats = new()
        {
            {BuildOutputType.App, "{0}.exe"},
            {BuildOutputType.VisualStudioSolution, "{0}.sln"},
        };
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        private const string ArchitectureIntel64BitId = "Intel 64-bit";
        private const string ArchitectureIntel32BitId = "Intel 32-bit";
#if UNITY_2023_1_OR_NEWER
        private const string ArchitectureArm64BitId = "ARM 64-bit";
#endif

        private enum BuildOutputType
        {
            App,
            VisualStudioSolution
        }

        #endregion

        public BuildWindows()
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
                    new(UnityEditor.BuildTarget.StandaloneWindows64, PlayerName, true, _binaryNameFormats[0]),
                    new(UnityEditor.BuildTarget.StandaloneWindows64, ServerName, false, _binaryNameFormats[0], (int)StandaloneBuildSubtarget.Server),
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
                    new(ArchitectureVariantKey, new string[] {
                        ArchitectureIntel64BitId, ArchitectureIntel32BitId,
#if UNITY_2023_1_OR_NEWER
                        ArchitectureArm64BitId
#endif
                    }, 0, false),
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
            UnityEditor.BuildTarget target = key switch
            {
                ArchitectureIntel32BitId => UnityEditor.BuildTarget.StandaloneWindows,
                ArchitectureIntel64BitId or _ => UnityEditor.BuildTarget.StandaloneWindows64,
            };

#if UNITY_STANDALONE_WIN && UNITY_2023_1_OR_NEWER
            UnityEditor.WindowsStandalone.UserBuildSettings.architecture = key == ArchitectureArm64BitId ?
                UnityEditor.Build.OSArchitecture.ARM64 :
                UnityEditor.Build.OSArchitecture.x64;
#endif

            _ = EditorUserBuildSettings.SwitchActiveBuildTarget(_targetGroup, target);
        }

        private void SetBuildOutput(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

#if UNITY_STANDALONE_WIN
            UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = outputType == BuildOutputType.VisualStudioSolution;
#endif

            targets[0].binaryNameFormat = _binaryNameFormats[outputType];
        }
    }
}
