using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildReleaseType
{
    public string typeName = "Type Name";
    public string bundleIndentifier = Application.bundleIdentifier;
    public string productName = Application.productName;
}

}