//using UnityEditor;

//namespace SuperSystems.UnityBuild
//{

//[System.Serializable]
//public class BuildAndroid : BuildPlatform
//{
//    #region Constants

//    private const string _name = "Android";
//    private const string _binaryNameFormat = "{0}.apk";
//    private const string _dataDirNameFormat = "{0}_Data";
//    private const BuildTargetGroup _targetGroup = BuildTargetGroup.Android;

//    #endregion

//    public BuildAndroid()
//    {
//        enabled = false;
//        Init();
//    }

//    public override void Init()
//    {
//        platformName = _name;
//        binaryNameFormat = _binaryNameFormat;
//        dataDirNameFormat = _dataDirNameFormat;
//        targetGroup = _targetGroup;

//        if (architectures == null || architectures.Length == 0)
//        {
//            architectures = new BuildArchitecture[] { 
//                new BuildArchitecture(BuildTarget.Android, "Android", true)
//            };
//        }
//        if (variants == null || variants.Length == 0)
//        {
//            variants = new BuildVariant[] {
//                new BuildVariant("Device Type", "FAT", true),
//                new BuildVariant("Device Type", "ARMv7", false),
//                new BuildVariant("Device Type", "x86", false),
//                new BuildVariant("Texture Compression", "ETC (Default)", true),
//                new BuildVariant("Texture Compression", "ETC2", false),
//                new BuildVariant("Texture Compression", "ASTC", false),
//                new BuildVariant("Texture Compression", "DXT", false),
//                new BuildVariant("Texture Compression", "PVRTC", false),
//                new BuildVariant("Texture Compression", "ATC", false),
//                new BuildVariant("Texture Compression", "Generic", false),
//                new BuildVariant("Build System", "Internal (Default)", true),
//                new BuildVariant("Build System", "Gradle", false),
//                new BuildVariant("Build System", "ADT (Legacy)", false)
//            };
//        }
//    }
//}

//}