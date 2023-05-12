using System;
using System.Collections.Generic;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildPC : BuildPlatform
    {
        #region Constants

        private const string _name = "PC";
        private Dictionary<BuildOutputType, string> _binaryNameFormats = new Dictionary<BuildOutputType, string>{
            {BuildOutputType.App, "{0}.exe"},
            {BuildOutputType.VisualStudioSolution, "{0}.sln"},
        };
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        private const string _buildOutputTypeVariantId = "Build Output";

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
            dataDirNameFormat = _dataDirNameFormat;
            targetGroup = _targetGroup;

            if (architectures == null || architectures.Length == 0)
            {
                architectures = new BuildArchitecture[] {
                    new BuildArchitecture(BuildTarget.StandaloneWindows, "Windows x86", true, _binaryNameFormats[0]),
                    new BuildArchitecture(BuildTarget.StandaloneWindows64, "Windows x64", false, _binaryNameFormats[0])
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new BuildScriptingBackend(ScriptingImplementation.Mono2x, true),
                    new BuildScriptingBackend(ScriptingImplementation.IL2CPP, false),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
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
                }
            }
        }

        private void SetBuildOutputType(string key)
        {
            BuildOutputType outputType = EnumValueFromKey<BuildOutputType>(key);

#if UNITY_STANDALONE_WIN
            UnityEditor.WindowsStandalone.UserBuildSettings.createSolution = outputType == BuildOutputType.VisualStudioSolution;
#endif

            architectures[0].binaryNameFormat = _binaryNameFormats[outputType];
        }
    }
}
