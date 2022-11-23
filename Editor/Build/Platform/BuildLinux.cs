using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildLinux : BuildPlatform
    {
        #region Constants

        private const string _name = "Linux";
        private const string _dataDirNameFormat = "{0}_Data";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        #endregion

        public BuildLinux()
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
                    new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", true, "{0}.x86_64"),
    #if !UNITY_2019_2_OR_NEWER
                    new BuildArchitecture(BuildTarget.StandaloneLinuxUniversal, "Linux Universal", false, "{0}"),
                    new BuildArchitecture(BuildTarget.StandaloneLinux, "Linux x86", false, "{0}.x86"),
    #endif
                };
            }


            bool hasIL2Installed = false;

    #if UNITY_2019_2_OR_NEWER
            //On 2019.2 or newer, only check for Linux 64 IL2 installation
            hasIL2Installed = BuildScriptBackend.IsIL2CPPInstalled(_targetGroup, BuildTarget.StandaloneLinux64);
#else
            //On 2019.1 or older, verify both 64 bit and 32 bit installation
            //This is because when we're able to select "Linux x86" in BuildTool, we need the 32-bit dlls
            hasIL2Installed = BuildScriptBackend.IsIL2CPPInstalled(_targetGroup, BuildTarget.StandaloneLinux64)
                && BuildScriptBackend.IsIL2CPPInstalled(_targetGroup, BuildTarget.StandaloneLinux);
#endif

            if (hasIL2Installed)
            {
                if (scriptBackends == null || scriptBackends.Length == 0)
                {
                    scriptBackends = new BuildScriptBackend[]
                    {
                        new BuildScriptBackend(ScriptingImplementation.Mono2x, "Mono 2x Runtime", true),
                        new BuildScriptBackend(ScriptingImplementation.IL2CPP, "IL2CPP Runtime", false),
                    };
                }
            }

        }
    }
}
