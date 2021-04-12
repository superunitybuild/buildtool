using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildOSX : BuildPlatform
    {
        #region Constants

        private const string _name = "OSX";
        private const string _binaryNameFormat = "{0}.app";
        private const string _dataDirNameFormat = "{0}.app/Contents";
        private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

        #endregion

        public BuildOSX()
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
                    new BuildArchitecture(BuildTarget.StandaloneOSX, "OSX", true, _binaryNameFormat),
                };
            }
        }
    }
}
