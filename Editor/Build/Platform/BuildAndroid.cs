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

    private const string _deviceTypeVariantId = "Device Type";
    private const string _textureCompressionVariantId = "Texture Compression";
    private const string _buildSystemVariantId = "Build System";
    private const string _splitAPKsVariantId = "Split APKs";
    
    #endregion

    public BuildAndroid()
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
                new BuildArchitecture(BuildTarget.Android, "Android", true, _binaryNameFormat)
            };
        }
        if (variants == null || variants.Length == 0)
        {
            variants = new BuildVariant[] {
#if UNITY_2018_1_OR_NEWER
                new BuildVariant(_deviceTypeVariantId, new string[] { "ARMv7", "ARM64", "All" }, 0),
#else
                new BuildVariant(_deviceTypeVariantId, new string[] { "FAT", "ARMv7", "x86" }, 0),
#endif
                new BuildVariant(_textureCompressionVariantId, new string[] { "ETC", "ETC2", "ASTC", "DXT", "PVRTC", "ATC", "Generic" }, 0),
                new BuildVariant(_buildSystemVariantId, new string[] { "Internal", "Gradle", "ADT (Legacy)" }, 0),
                new BuildVariant(_splitAPKsVariantId, new string[] { "Disabled", "Enabled" }, 0)
            };
        }
    }

    public override void ApplyVariant()
    {
        foreach (var variantOption in variants)
        {
            switch (variantOption.variantName)
            {
                case _deviceTypeVariantId:
                    SetDeviceType(variantOption.variantKey);
                    break;
                case _textureCompressionVariantId:
                    SetTextureCompression(variantOption.variantKey);
                    break;
                case _buildSystemVariantId:
                    SetBuildSystem(variantOption.variantKey);
                    break;
                case _splitAPKsVariantId:
                    SetSplitAPKs(variantOption.variantKey);
                    break;
            }
        }
    }

    private void SetDeviceType(string key)
    {
#if UNITY_2018_1_OR_NEWER
        PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)System.Enum.Parse(typeof(AndroidArchitecture), key);
#else
        PlayerSettings.Android.targetDevice = (AndroidTargetDevice)System.Enum.Parse(typeof(AndroidTargetDevice), key);
#endif
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

    private void SetSplitAPKs(string key)
    {
        PlayerSettings.Android.buildApkPerCpuArchitecture = key == "Enabled";
    }
}

}