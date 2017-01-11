
namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildDistribution
{
    public string distributionName;
    public bool enabled;

    public BuildDistribution()
    {
        this.distributionName = string.Empty;
        this.enabled = true;
    }

    public BuildDistribution(string distributionName, bool enabled)
    {
        this.distributionName = distributionName;
        this.enabled = enabled;
    }
}

}