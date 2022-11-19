using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildScriptBackend
    {
        public ScriptingImplementation scriptBackend;
        public string name;
        public bool enabled;

        public BuildScriptBackend(ScriptingImplementation scriptBackend, string name, bool enabled)
        {
            this.scriptBackend = scriptBackend;
            this.name = name;
            this.enabled = enabled;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
