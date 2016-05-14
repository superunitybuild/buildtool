using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class Configuration
{
    public bool enabled = true;
    public SerializableDictionary<string, Configuration> childConfigurations;
}

}