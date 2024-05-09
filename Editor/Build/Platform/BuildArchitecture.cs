using System;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildArchitecture
    {
        public BuildTarget target;
        public StandaloneBuildSubtarget subtarget;
        public string name;
        public bool enabled;
        public string binaryNameFormat;

        public BuildArchitecture(BuildTarget target, string name, bool enabled, string binaryNameFormat, StandaloneBuildSubtarget subtarget = StandaloneBuildSubtarget.Player)
        {
            this.target = target;
            this.subtarget = subtarget;
            this.name = name;
            this.enabled = enabled;
            this.binaryNameFormat = binaryNameFormat;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
