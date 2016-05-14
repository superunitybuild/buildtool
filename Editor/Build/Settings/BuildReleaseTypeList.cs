using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildReleaseTypeList
{
    [SerializeField]
    public BuildReleaseType[] releastTypes = new BuildReleaseType[] {
    };
}

}