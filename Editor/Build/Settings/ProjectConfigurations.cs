using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class ProjectConfigurations
{
    [System.Serializable]
    public class Configuration
    {
        public bool enabled = true;
        public SerializableDictionary<string, Configuration> childConfigurations;
    }

    public SerializableDictionary<string, Configuration> configSet;

    public void Refresh()
    {
        SerializableDictionary<string, Configuration> refreshedConfigSet = new SerializableDictionary<string, Configuration>();

        BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
        for (int i = 0; i < releaseTypes.Length; i++)
        {
            string key = releaseTypes[i].typeName;
            Configuration relConfig = new Configuration();
            SerializableDictionary<string, Configuration> prevChildConfig = null;

            if (refreshedConfigSet.ContainsKey(key))
                continue;

            if (configSet != null && configSet.ContainsKey(key))
            {
                relConfig.enabled = configSet[key].enabled;
                prevChildConfig = configSet[key].childConfigurations;
            }

            relConfig.childConfigurations = RefreshPlatforms(prevChildConfig);

            refreshedConfigSet.Add(key, relConfig);
        }

        configSet = refreshedConfigSet;
    }

    private SerializableDictionary<string, Configuration> RefreshPlatforms(SerializableDictionary<string, Configuration> prevConfigSet)
    {
        SerializableDictionary<string, Configuration> refreshedConfigSet = new SerializableDictionary<string, Configuration>();

        BuildPlatform[] platforms = BuildSettings.platformList.platforms;
        for (int i = 0; i < platforms.Length; i++)
        {
            if (!platforms[i].enabled && platforms[i].atLeastOneArch)
                continue;

            string key = platforms[i].platformName;
            Configuration relConfig = new Configuration();
            SerializableDictionary<string, Configuration> prevChildConfig = null;

            if (refreshedConfigSet.ContainsKey(key))
                continue;

            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
                prevChildConfig = prevConfigSet[key].childConfigurations;
            }

            BuildArchitecture[] architectures = platforms[i].architectures;

            if (architectures.Length > 1)
            {
                relConfig.childConfigurations = RefreshArchitectures(architectures, platforms[i].distributionList.distributions, prevChildConfig);
            }

            refreshedConfigSet.Add(key, relConfig);
        }

        return refreshedConfigSet;
    }

    private SerializableDictionary<string, Configuration> RefreshArchitectures(BuildArchitecture[] architectures, BuildDistribution[] distributions, SerializableDictionary<string, Configuration> prevConfigSet)
    {
        SerializableDictionary<string, Configuration> refreshedConfigSet = new SerializableDictionary<string, Configuration>();

        for (int i = 0; i < architectures.Length; i++)
        {
            if (!architectures[i].enabled)
                continue;

            string key = architectures[i].name;
            Configuration relConfig = new Configuration();
            SerializableDictionary<string, Configuration> prevChildConfig = null;

            if (refreshedConfigSet.ContainsKey(key))
                continue;

            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
                prevChildConfig = prevConfigSet[key].childConfigurations;
            }

            if (distributions.Length > 0)
                relConfig.childConfigurations = RefreshDistributions(distributions, prevChildConfig);

            refreshedConfigSet.Add(key, relConfig);
        }

        return refreshedConfigSet;
    }

    private SerializableDictionary<string, Configuration> RefreshDistributions(BuildDistribution[] distributions, SerializableDictionary<string, Configuration> prevConfigSet)
    {
        SerializableDictionary<string, Configuration> refreshedConfigSet = new SerializableDictionary<string, Configuration>();

        for (int i = 0; i < distributions.Length; i++)
        {
            if (!distributions[i].enabled)
                continue;

            string key = distributions[i].distributionName;
            Configuration relConfig = new Configuration();
            SerializableDictionary<string, Configuration> prevChildConfig = null;

            if (refreshedConfigSet.ContainsKey(key))
                continue;

            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
                prevChildConfig = prevConfigSet[key].childConfigurations;
            }

            refreshedConfigSet.Add(key, relConfig);
        }

        return refreshedConfigSet;
    }
}

}