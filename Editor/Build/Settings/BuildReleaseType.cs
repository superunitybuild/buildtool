using UnityEditor;
using UnityEngine.Serialization;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildReleaseType
{
    public string typeName = string.Empty;
    [FormerlySerializedAs("bundleIndentifier")]
    public string bundleIdentifier = string.Empty;
    public string companyName = string.Empty;
    public string productName = string.Empty;

    public bool developmentBuild = false;
    public bool allowDebugging = false;
    public bool enableHeadlessMode = false;
    public string customDefines = string.Empty;

    public SceneList sceneList = new SceneList();
}

}