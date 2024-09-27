using System;
using UnityBuildTarget = UnityEditor.BuildTarget;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildTarget
    {
        public UnityBuildTarget type;
        public string name;
        public bool enabled;
        public string binaryNameFormat;
        public int subtarget;

        public bool isStandalone => type is
            UnityBuildTarget.StandaloneLinux64 or
            UnityBuildTarget.StandaloneOSX or
            UnityBuildTarget.StandaloneWindows or UnityBuildTarget.StandaloneWindows64;

        public BuildTarget(UnityBuildTarget type, string name, bool enabled, string binaryNameFormat, int subtarget = 0)
        {
            this.type = type;
            this.name = name;
            this.enabled = enabled;
            this.binaryNameFormat = binaryNameFormat;
            this.subtarget = subtarget;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
