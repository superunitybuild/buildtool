using System;
namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class Configuration
    {
        public bool enabled = true;
        public string[] childKeys = null;
    }

    [Serializable]
    public class ConfigDictionary : SerializableDictionary<string, Configuration> { }
}
