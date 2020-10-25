using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildWebGL : BuildPlatform
{
    #region Constants

    private const string _name = "WebGL";
    private const string _binaryNameFormat = "{0}";
    private const string _dataDirNameFormat = "{0}_Data";
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
        dataDirNameFormat = _dataDirNameFormat;
        targetGroup = _targetGroup;

        if (architectures == null || architectures.Length == 0)
        {
            architectures = new BuildArchitecture[] { 
                new BuildArchitecture(BuildTarget.WebGL, "WebGL", true, _binaryNameFormat),
            };
        }
    }
}

}