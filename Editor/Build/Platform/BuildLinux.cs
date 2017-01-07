using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildLinux : BuildPlatform
{
    #region Constants

    private const string _name = "Linux";
    private const string _binaryNameFormat = "{0}.x86";
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
        binaryNameFormat = _binaryNameFormat;
        dataDirNameFormat = _dataDirNameFormat;
        targetGroup = _targetGroup;

        if (architectures == null || architectures.Length == 0)
        {
            architectures = new BuildArchitecture[] { 
                new BuildArchitecture(BuildTarget.StandaloneLinuxUniversal, "Linux Universal", true),
                new BuildArchitecture(BuildTarget.StandaloneLinux, "Linux x86", false),
                new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", false)
            };
        }
    }
}

}