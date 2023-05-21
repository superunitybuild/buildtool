using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
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
                    new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", true, "{0}.x86_64")
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
