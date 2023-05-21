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
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.WSA;

        private const string _architectureVariantId = "Architecture";
        private const string _buildTypeVariantId = "Build Type";
#if !UNITY_2021_2_OR_NEWER
        private const string _targetDeviceVariantId = "Target Device";
#endif
        #endregion

        public BuildUWP()
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
                    new BuildArchitecture(BuildTarget.WSAPlayer, "UWP", true, _binaryNameFormat),
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new BuildScriptingBackend(ScriptingImplementation.IL2CPP, true),
                };
            }

            if (variants == null || variants.Length == 0)
            {
                variants = new BuildVariant[] {
#if !UNITY_2021_2_OR_NEWER
                    new BuildVariant(_targetDeviceVariantId, EnumNamesToArray<WSASubtarget>(true).ToArray(), 0),
#endif
                    new BuildVariant(_architectureVariantId, new string[] { "x86", "x64", "ARM", "ARM64" }, 0),
                    new BuildVariant(_buildTypeVariantId, EnumNamesToArray<WSAUWPBuildType>(true).ToArray(), 0)
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
                    case _architectureVariantId:
                        SetArchitecture(key);
                        break;
                    case _buildTypeVariantId:
                        SetBuildType(key);
                        break;
#if !UNITY_2021_2_OR_NEWER
                    case _targetDeviceVariantId:
                        SetTargetDevice(key);
                        break;
#endif
                }
            }
        }

        private void SetArchitecture(string key)
        {
            EditorUserBuildSettings.wsaArchitecture = key;
        }

        private void SetBuildType(string key)
        {
            EditorUserBuildSettings.wsaUWPBuildType = EnumValueFromKey<WSAUWPBuildType>(key);
        }

#if !UNITY_2021_2_OR_NEWER
        private void SetTargetDevice(string key)
        {
            EditorUserBuildSettings.wsaSubtarget = EnumValueFromKey<WSASubtarget>(key);
        }
#endif
    }
}
