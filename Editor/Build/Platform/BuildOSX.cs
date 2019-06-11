using UnityEditor;

namespace SuperSystems.UnityBuild
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
#if UNITY_2017_3_OR_NEWER
                new BuildArchitecture(BuildTarget.StandaloneOSX, "OSX", true, _binaryNameFormat),
#else
                new BuildArchitecture(BuildTarget.StandaloneOSXUniversal, "OSX Universal", true, _binaryNameFormat),
                new BuildArchitecture(BuildTarget.StandaloneOSXIntel, "OSX Intel", false, _binaryNameFormat),
                new BuildArchitecture(BuildTarget.StandaloneOSXIntel64, "OSX Intel64", false, _binaryNameFormat)
#endif
            };
        }
    }
}

}