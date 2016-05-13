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
    }

    #region Variables

    private BuildArchitecture[] _architectures = new BuildArchitecture[] { 
        new BuildArchitecture(BuildTarget.StandaloneOSXUniversal, "OSX Universal", true),
        new BuildArchitecture(BuildTarget.StandaloneOSXIntel, "OSX x86", false),
        new BuildArchitecture(BuildTarget.StandaloneOSXIntel64, "OSX x64", false)
    };

    #endregion

    #region Methods & Properties (DO NOT EDIT)

    public BuildArchitecture[] architectures
    {
        get { return _architectures; }
    }

    public string binaryNameFormat
    {
        get { return _binaryNameFormat; }
    }

    public string dataDirNameFormat
    {
        get { return _dataDirNameFormat; }
    }

    #endregion
}

}