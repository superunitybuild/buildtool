using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildAndroid : BuildPlatform
{
    #region Constants

    private const string _name = "Android";
    private const string _binaryNameFormat = "{0}.apk";
    private const string _dataDirNameFormat = "{0}_Data";
    private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

    #endregion

    public BuildAndroid()
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
                new BuildArchitecture(BuildTarget.Android, "Android", true)
            };
        }
        if (variants == null || variants.Length == 0)
        {
            variants = new BuildVariant[] {
                new BuildVariant("Device Type", new string[] { "FAT", "ARMv7", "x86" }, 0),
                new BuildVariant("Texture Compression", new string[] { "ETC", "ETC2", "ASTC", "DXT", "PVRTC", "ATC", "Generic" }, 0),
                new BuildVariant("Build System", new string[] { "Internal", "Gradle", "ADT (Legacy)" }, 0)
            };
        }
    }

    public override void ApplyVariant()
    {
        foreach (var variantOption in variants)
        {
            switch (variantOption.variantName)
            {
                case "DeviceType":
                    SetDeviceType(variantOption.variantKey);
                    break;
                case "Texture Compression":
                    SetTextureCompression(variantOption.variantKey);
                    break;
                case "Build System":
                    SetBuildSystem(variantOption.variantKey);
                    break;
            }
        }
    }

    private void SetDeviceType(string key)
    {
        PlayerSettings.Android.targetDevice = (AndroidTargetDevice)System.Enum.Parse(typeof(AndroidTargetDevice), key);
    }

    private void SetTextureCompression(string key)
    {
        EditorUserBuildSettings.androidBuildSubtarget
            = (MobileTextureSubtarget)System.Enum.Parse(typeof(MobileTextureSubtarget), key);
    }

    private void SetBuildSystem(string key)
    {
        EditorUserBuildSettings.androidBuildSystem
            = (AndroidBuildSystem)System.Enum.Parse(typeof(AndroidBuildSystem), key);
    }
}

}