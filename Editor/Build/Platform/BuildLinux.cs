using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildLinux : BuildPlatform
    {
        #region Constants

        private const string _name = "Linux";
        private readonly string _binaryNameFormat = "{0}.x86_64";
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

            if (targets == null || targets.Length == 0)
            {
                targets = new BuildTarget[] {
                    new(UnityEditor.BuildTarget.StandaloneLinux64, PlayerName, true, _binaryNameFormat),
                    new(UnityEditor.BuildTarget.StandaloneLinux64, ServerName, false, _binaryNameFormat, (int)StandaloneBuildSubtarget.Server),
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

        }
    }
}
