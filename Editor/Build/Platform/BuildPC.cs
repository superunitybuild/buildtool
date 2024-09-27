using System;
using System.Collections.Generic;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildPC : BuildPlatform
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

        private enum BuildOutputType
        {
            App,
            VisualStudioSolution
        }

        #endregion

        public BuildPC()
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
                    new(ArchitectureVariantKey, new string[] { ArchitectureIntel64BitId, ArchitectureIntel32BitId }, 0, false),
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
