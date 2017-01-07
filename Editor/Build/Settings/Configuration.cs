
namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class Configuration
{
    public bool enabled = true;
    public string[] childKeys = null;
}

[System.Serializable]
public class ConfigDictionary : SerializableDictionary<string, Configuration> { }

}