using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildArchitecture
{
    public BuildTarget target;
    public string name;
    public bool enabled;

    public BuildArchitecture(BuildTarget target, string name, bool enabled)
    {
        this.target = target;
        this.name = name;
        this.enabled = enabled;
    }
}

}