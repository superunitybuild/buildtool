//using UnityEditor;

//namespace SuperSystems.UnityBuild
//{

//[System.Serializable]
//public class BuildIOS : BuildPlatform
//{
//    #region Constants

//    // TODO: Fix iOS binary/data dir name.
//    private const string _name = "iOS";
//    private const string _binaryNameFormat = "{0}.apk";
//    private const string _dataDirNameFormat = "{0}_Data";
//    private const BuildTargetGroup _targetGroup = BuildTargetGroup.iOS;

//    #endregion

//    public BuildIOS()
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
//                new BuildArchitecture(BuildTarget.iOS, "iOS", true)
//            };
//        }
//        if (variants == null || variants.Length == 0)
//        {
//            variants = new BuildVariant[] {
//                new BuildVariant("XCode", "Release", true),
//                new BuildVariant("XCode", "Debug", false),
//                new BuildVariant("Symlink Unity Libraries", "Disabled", true),
//                new BuildVariant("Symlink Unity Libraries", "Enabled", false)
//            };
//        }
//    }
//}

//}