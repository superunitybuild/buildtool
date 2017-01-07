using System.Collections.Generic;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class ProjectConfigurations
{
    // Data
    public ConfigDictionary configSet;

    // View
    public bool showViewOptions = false;
    public bool showConfigs = false;
    public bool showBuildInfo = false;

    public bool hideDisabled = false;
    public bool treeView = false;

    public string selectedKeyChain = string.Empty;

    public void Refresh()
    {
        ConfigDictionary refreshedConfigSet = new ConfigDictionary();

        BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
        for (int i = 0; i < releaseTypes.Length; i++)
        {
            string key = releaseTypes[i].typeName;
            Configuration relConfig = new Configuration();

            // Check for duplicate.
            if (refreshedConfigSet.ContainsKey(key))
                continue;

            // Copy old setting if it exists.
            if (configSet != null && configSet.ContainsKey(key))
            {
                relConfig.enabled = configSet[key].enabled;
            }

            // Get child keys.
            relConfig.childKeys = RefreshPlatforms(key, refreshedConfigSet, configSet);

            // Save configuration.
            refreshedConfigSet.Add(key, relConfig);
        }

        configSet = refreshedConfigSet;
    }

    public string[] BuildAllKeychains()
    {
        List<string> keychains = new List<string>();

        foreach (string key in configSet.Keys)
        {
            Configuration config = configSet[key];
            BuildKeychainsRecursive(ref keychains, config, key, "", 0);
        }

        return keychains.ToArray();
    }

    public int GetEnabledBuildsCount()
    {
        int count = 0;

        foreach (string key in configSet.Keys)
        {
            Configuration config = configSet[key];
            NavigateTree(key, ref config, 0, ref count);
        }

        return count;
    }

    private void NavigateTree(string key, ref Configuration config, int depth, ref int count)
    {
        if (depth >= 2 && config.enabled && (config.childConfigurations == null || config.childConfigurations.Count == 0))
        {
            ++count;
        }
        else if (config.enabled)
        {
            foreach (string childKey in config.childConfigurations.Keys)
            {
                Configuration childConfig = config.childConfigurations[childKey];
                NavigateTree(childKey, ref childConfig, depth + 1, ref count);
            }
        }
    }

    private void BuildKeychainsRecursive(ref List<string> keychains, Configuration config, string key, string currentKeychain, int depth)
    {
        if (depth >= 2 && config.enabled && (config.childConfigurations == null || config.childConfigurations.Count == 0))
        {
            keychains.Add(currentKeychain + "/" + key);
        }
        else if (config.childConfigurations != null && config.childConfigurations.Count > 0 && config.enabled)
        {
            if (string.IsNullOrEmpty(currentKeychain))
                currentKeychain = key;
            else
                currentKeychain += "/" + key;

            foreach (string childKey in config.childConfigurations.Keys)
            {
                Configuration childConfig = config.childConfigurations[childKey];
                BuildKeychainsRecursive(ref keychains, childConfig, childKey, currentKeychain, depth + 1);
            }
        }
    }

    public bool ParseKeychain(string keychain, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildArchitecture architecture, out BuildDistribution distribution)
    {
        bool success = false;
        string[] keys = keychain.Split('/');
        int keyCount = keys.Length;
        int targetKey = 0;
        Configuration childConfig = null;

        releaseType = null;
        platform = null;
        architecture = null;
        distribution = null;

        // Parse release type.
        if (keyCount > targetKey && configSet.ContainsKey(keys[targetKey]))
        {
            for (int i = 0; i < BuildSettings.releaseTypeList.releaseTypes.Length; i++)
            {
                BuildReleaseType rt = BuildSettings.releaseTypeList.releaseTypes[i];

                if (keys[targetKey] == rt.typeName)
                {
                    releaseType = rt;
                    childConfig = configSet[keys[targetKey]];
                    break;
                }
            }
        }

        if (releaseType == null)
            return false;

        // Parse platform.
        ++targetKey;
        if (keyCount > targetKey && childConfig != null && childConfig.childConfigurations != null && childConfig.childConfigurations.ContainsKey(keys[targetKey]))
        {
            for (int i = 0; i < BuildSettings.platformList.platforms.Length; i++)
            {
                BuildPlatform p = BuildSettings.platformList.platforms[i];

                if (keys[targetKey] == p.platformName)
                {
                    platform = p;
                    childConfig = childConfig.childConfigurations[keys[targetKey]];
                    break;
                }
            }
        }

        if (platform == null)
            return false;

        // Parse architecture.
        if (platform.architectures.Length == 1)
        {
            // Only one architecture, so it won't even appear in dictionary. Just get it directly.
            architecture = platform.architectures[0];
            success = true;

            if (childConfig.childConfigurations.ContainsKey(keys[targetKey]))
            {
                childConfig = childConfig.childConfigurations[keys[targetKey]];
            }
            else
            {
                childConfig = null;
            }
        }
        else
        {
            ++targetKey;
            if (keyCount > targetKey && childConfig != null && childConfig.childConfigurations != null && childConfig.childConfigurations.ContainsKey(keys[targetKey]))
            {
                for (int i = 0; i < platform.architectures.Length; i++)
                {
                    BuildArchitecture arch = platform.architectures[i];

                    if (keys[targetKey] == arch.name)
                    {
                        architecture = arch;
                        childConfig = childConfig.childConfigurations[keys[targetKey]];
                        success = true;
                        break;
                    }
                }
            }
        }

        if (architecture == null)
            return false;

        // TODO: Parse variants.

        // Parse distribution.
        ++targetKey;
        if (keyCount > targetKey && childConfig != null && childConfig.childConfigurations != null && childConfig.childConfigurations.ContainsKey(keys[targetKey]))
        {
            success = false;
            for (int i = 0; i < platform.distributionList.distributions.Length; i++)
            {
                BuildDistribution dist = platform.distributionList.distributions[i];

                if (keys[targetKey] == dist.distributionName)
                {
                    distribution = dist;
                    success = true;
                    break;
                }
            }
        }

        return success;
    }

    private string[] RefreshPlatforms(string keyChain, ConfigDictionary refreshedConfigSet, ConfigDictionary prevConfigSet)
    {
        List<string> childKeys = new List<string>();

        BuildPlatform[] platforms = BuildSettings.platformList.platforms;
        for (int i = 0; i < platforms.Length; i++)
        {
            // Skip if platform is disabled or if it doesn't have any enabled architectures.
            if (!platforms[i].enabled || !platforms[i].atLeastOneArch)
                continue;

            string key = keyChain + "/" + platforms[i].platformName;
            Configuration relConfig = new Configuration();

            // Check for duplicate key.
            if (refreshedConfigSet.ContainsKey(key))
                continue;

            // Copy previous settings if they exist.
            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
            }

            // Refresh architectures.
            BuildArchitecture[] architectures = platforms[i].architectures;
            if (architectures.Length > 0)
                relConfig.childKeys = RefreshArchitectures(key, refreshedConfigSet, architectures, platforms[i].distributionList.distributions, prevConfigSet);

            // Save configuration.
            refreshedConfigSet.Add(key, relConfig);

            // Add key to list to send back to parent.
            childKeys.Add(key);
        }

        return childKeys.ToArray();
    }

    private string[] RefreshArchitectures(string keyChain, ConfigDictionary refreshedConfigSet, BuildArchitecture[] architectures, BuildDistribution[] distributions, ConfigDictionary prevConfigSet)
    {
        List<string> childKeys = new List<string>();

        for (int i = 0; i < architectures.Length; i++)
        {
            // Skip if architecture is disabled.
            if (!architectures[i].enabled)
                continue;

            string key = keyChain + "/" + architectures[i].name;
            Configuration relConfig = new Configuration();

            // Check for a duplicate key.
            if (refreshedConfigSet.ContainsKey(key))
                continue;

            // Copy previous settings if they exist.
            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
            }

            // Refresh distributions.
            if (distributions.Length > 0)
                relConfig.childKeys = RefreshDistributions(key, refreshedConfigSet, distributions, prevConfigSet);

            // Save configuration.
            refreshedConfigSet.Add(key, relConfig);

            // Add key to list to send back to parent.
            childKeys.Add(key);
        }

        return childKeys.ToArray();
    }

    private string[] RefreshDistributions(string keyChain, ConfigDictionary refreshedConfigSet, BuildDistribution[] distributions, ConfigDictionary prevConfigSet)
    {
        List<string> childKeys = new List<string>();

        for (int i = 0; i < distributions.Length; i++)
        {
            if (!distributions[i].enabled)
                continue;

            string key = keyChain + "/" + distributions[i].distributionName;
            Configuration relConfig = new Configuration();

            if (refreshedConfigSet.ContainsKey(key))
                continue;

            if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
            {
                relConfig.enabled = prevConfigSet[key].enabled;
            }

            refreshedConfigSet.Add(key, relConfig);

            // Add key to list to send back to parent.
            childKeys.Add(key);
        }

        return childKeys.ToArray();
    }
}

}