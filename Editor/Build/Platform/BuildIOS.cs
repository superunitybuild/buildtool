using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildIOS : BuildPlatform
{
    #region Constants (SET VALUES)

    // TODO: Fix iOS binary/data dir name.
    private const string _name = "iOS";
    private const string _binaryNameFormat = "{0}.apk";
    private const string _dataDirNameFormat = "{0}_Data";

    #endregion

    public BuildIOS()
    {
        enabled = false;
        platformName = _name;
        architectures = new BuildArchitecture[] { 
            new BuildArchitecture(BuildTarget.iOS, "iOS", true)
        };
        variants = new BuildVariant[] {
            new BuildVariant("XCode", "Release", true),
            new BuildVariant("XCode", "Debug", false),
            new BuildVariant("Symlink Unity Libraries", "Disabled", true),
            new BuildVariant("Symlink Unity Libraries", "Enabled", false)
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