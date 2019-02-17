using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildPC : BuildPlatform
{
    #region Constants

    private const string _name = "PC";
    private const string _binaryNameFormat = "{0}.exe";
    private const string _dataDirNameFormat = "{0}_Data";
    private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

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
                new BuildArchitecture(BuildTarget.StandaloneWindows, "Windows x86", true, _binaryNameFormat),
                new BuildArchitecture(BuildTarget.StandaloneWindows64, "Windows x64", false, _binaryNameFormat)
            };
        }
    }
}

}