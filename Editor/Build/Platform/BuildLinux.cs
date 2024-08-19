using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildLinux : BuildPlatform
    {
        #region Constants

        private const string _name = "Linux";
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
            targetGroup = _targetGroup;

            if (architectures == null || architectures.Length == 0)
            {
                architectures = new BuildArchitecture[] {
                    new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", false, "{0}.x86_64", StandaloneBuildSubtarget.Player),
                    new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64 Server", true, "{0}.x86_64", StandaloneBuildSubtarget.Server),
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

        }
    }
}
