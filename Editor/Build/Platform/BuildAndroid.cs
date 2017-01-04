using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildAndroid : BuildPlatform
{
    #region Constants (SET VALUES)

    private const string _name = "Android";
    private const string _binaryNameFormat = "{0}.apk";
    private const string _dataDirNameFormat = "{0}_Data";

    #endregion

    public BuildAndroid()
    {
        enabled = false;
        platformName = _name;
        architectures = new BuildArchitecture[] { 
            new BuildArchitecture(BuildTarget.Android, "Android", true)
        };
        variants = new BuildVariant[] {
            new BuildVariant("Device Type", "FAT", true),
            new BuildVariant("Device Type", "ARMv7", false),
            new BuildVariant("Device Type", "x86", false),
            new BuildVariant("Texture Compression", "ETC (Default)", true),
            new BuildVariant("Texture Compression", "ETC2", false),
            new BuildVariant("Texture Compression", "ASTC", false),
            new BuildVariant("Texture Compression", "DXT", false),
            new BuildVariant("Texture Compression", "PVRTC", false),
            new BuildVariant("Texture Compression", "ATC", false),
            new BuildVariant("Texture Compression", "Generic", false),
            new BuildVariant("Build System", "Internal (Default)", true),
            new BuildVariant("Build System", "Gradle", false),
            new BuildVariant("Build System", "ADT (Legacy)", false)
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