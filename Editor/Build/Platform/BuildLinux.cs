using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildLinux : BuildPlatform
{
    #region Constants

    private const string _name = "Linux";
    private const string _dataDirNameFormat = "{0}_Data";
    private const BuildTargetGroup _targetGroup = BuildTargetGroup.Standalone;

    #endregion

    public BuildLinux()
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
                new BuildArchitecture(BuildTarget.StandaloneLinux64, "Linux x64", false, "{0}.x86_64"),
#if !UNITY_2019_2_OR_NEWER
                new BuildArchitecture(BuildTarget.StandaloneLinuxUniversal, "Linux Universal", true, "{0}"),
                new BuildArchitecture(BuildTarget.StandaloneLinux, "Linux x86", false, "{0}.x86"),
#endif
            };

#if UNITY_2018_3_OR_NEWER
            architectures[0].enabled = true;
#endif

#if UNITY_2018_3_OR_NEWER && !UNITY_2019_2_OR_NEWER
            architectures[1].enabled = false;
#endif
            }
        }
    }

}
