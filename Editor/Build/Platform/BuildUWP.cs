using System;
using System.Linq;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildUWP : BuildPlatform
    {
        #region Constants

        private const string _name = "UWP";
        private const string _binaryNameFormat = "";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.WSA;

        #endregion

        public BuildUWP()
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
                    new(UnityEditor.BuildTarget.WSAPlayer, PlayerName, true, _binaryNameFormat),
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new(ScriptingImplementation.IL2CPP, true),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
                    new(ArchitectureVariantKey, new string[] { "x86", "x64", "ARM", "ARM64" }, 0),
                    new(BuildOutputVariantKey, EnumNamesToArray<WSAUWPBuildType>(true).ToArray(), 0)
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
            EditorUserBuildSettings.wsaArchitecture = key;
        }

        private void SetBuildOutput(string key)
        {
            EditorUserBuildSettings.wsaUWPBuildType = EnumValueFromKey<WSAUWPBuildType>(key);
        }
    }
}
