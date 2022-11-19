using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildScriptBackend
    {
        public ScriptingImplementation scriptImplementation;
        public string name;
        public bool enabled;

        public BuildScriptBackend(ScriptingImplementation scriptImplementation, string name, bool enabled)
        {
            this.scriptImplementation = scriptImplementation;
            this.name = name;
            this.enabled = enabled;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
