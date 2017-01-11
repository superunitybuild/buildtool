
namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildVariant
{
    public string type;
    public string name;
    public bool enabled;

    public BuildVariant(string type, string name, bool enabled)
    {
        this.type = type;
        this.name = name;
        this.enabled = enabled;
    }
}

}