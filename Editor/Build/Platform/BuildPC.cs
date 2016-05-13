using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildPC : BuildPlatform
{
    #region Constants (SET VALUES)

    private const string _name = "PC";
    private const string _binaryNameFormat = "{0}.exe";
    private const string _dataDirNameFormat = "{0}_Data";

    #endregion

    public BuildPC()
    {
        enabled = false;
        platformName = _name;
    }

    #region Variables

    private BuildArchitecture[] _architectures = new BuildArchitecture[] { 
        new BuildArchitecture(BuildTarget.StandaloneWindows, "Windows x86", true),
        new BuildArchitecture(BuildTarget.StandaloneWindows64, "Windows x64", false)
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