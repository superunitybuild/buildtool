using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildOSX : BuildPlatform
{
    #region Constants (SET VALUES)

    private const string _name = "OSX";
    private const string _binaryNameFormat = "{0}.app";
    private const string _dataDirNameFormat = "{0}.app/Contents";

    #endregion

    public BuildOSX()
    {
        enabled = false;
        platformName = _name;
        architectures = new BuildArchitecture[] { 
            new BuildArchitecture(BuildTarget.StandaloneOSXUniversal, "OSX Universal", true),
            new BuildArchitecture(BuildTarget.StandaloneOSXIntel, "OSX x86", false),
            new BuildArchitecture(BuildTarget.StandaloneOSXIntel64, "OSX x64", false)
        };
    }

    #region Methods & Properties (DO NOT EDIT)

    public override string binaryNameFormat
    {
        get { return _binaryNameFormat; }
    }

    public override string dataDirNameFormat
    {
        get { return _dataDirNameFormat; }
    }

    #endregion
}

}