using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildWebGL : BuildPlatform
    {
        #region Constants

        private const string _name = "WebGL";
        private const string _binaryNameFormat = "{0}";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.WebGL;

        #endregion

        public BuildWebGL()
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
                    new(UnityEditor.BuildTarget.WebGL, PlayerName, true, _binaryNameFormat),
                };
            }

            if (scriptingBackends == null || scriptingBackends.Length == 0)
            {
                scriptingBackends = new BuildScriptingBackend[]
                {
                    new(ScriptingImplementation.IL2CPP, true),
                };
            }
        }
    }
}
