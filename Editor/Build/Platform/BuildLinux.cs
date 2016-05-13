using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildLinux : BuildPlatform
{
    #region Constants (SET VALUES)

    private const string _name = "Linux";
    private const string _binaryNameFormat = "{0}.x86";
    private const string _dataDirNameFormat = "{0}_Data";

    #endregion

    public BuildLinux()
    {
        enabled = false;
        platformName = _name;
    }

    #region Variables

    private BuildArchitecture[] _architectures = new BuildArchitecture[] { 
        new BuildArchitecture(BuildTarget.StandaloneLinuxUniversal, "Linux Universal", true),
        new BuildArchitecture(BuildTarget.StandaloneLinux, "Linux x86", false),
        new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", false)
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