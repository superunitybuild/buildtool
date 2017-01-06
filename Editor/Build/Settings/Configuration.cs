
namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class Configuration
{
    public bool enabled = true;
    public ConfigDictionary childConfigurations;
}

[System.Serializable]
public class ConfigDictionary : SerializableDictionary<string, Configuration> { }

}