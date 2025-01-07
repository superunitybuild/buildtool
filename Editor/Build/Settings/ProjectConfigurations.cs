using System;
using System.Collections.Generic;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
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
            ConfigDictionary refreshedConfigSet = new();

            BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
            for (int i = 0; i < releaseTypes.Length; i++)
            {
                string key = releaseTypes[i].typeName;
                Configuration relConfig = new();

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
            List<string> keychains = new();

            BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
            for (int i = 0; i < releaseTypes.Length; i++)
            {
                string key = releaseTypes[i].typeName;

                if (configSet.ContainsKey(key))
                {
                    Configuration config = configSet[key];
                    BuildKeychainsRecursive(ref keychains, config, key, 0);
                }
            }

            return keychains.ToArray();
        }

        public int GetEnabledBuildsCount()
        {
            int count = 0;

            BuildReleaseType[] releaseTypes = BuildSettings.releaseTypeList.releaseTypes;
            for (int i = 0; i < releaseTypes.Length; i++)
            {
                string key = releaseTypes[i].typeName;

                if (configSet.ContainsKey(key))
                {
                    Configuration config = configSet[key];
                    NavigateTree(key, config, 0, ref count);
                }
            }

            return count;
        }

        private void NavigateTree(string key, Configuration config, int depth, ref int count)
        {
            if (depth >= 2 && config.enabled && (config.childKeys == null || config.childKeys.Length == 0))
            {
                ++count;
            }
            else if (config.enabled && config.childKeys != null)
            {
                foreach (string childKey in config.childKeys)
                {
                    NavigateTree(childKey, configSet[childKey], depth + 1, ref count);
                }
            }
        }

        private void BuildKeychainsRecursive(ref List<string> keychains, Configuration config, string key, int depth)
        {
            if (depth >= 2 && config.enabled && (config.childKeys == null || config.childKeys.Length == 0))
            {
                keychains.Add(key);
            }
            else if (config.childKeys != null && config.childKeys.Length > 0 && config.enabled)
            {
                foreach (string childKey in config.childKeys)
                {
                    BuildKeychainsRecursive(ref keychains, configSet[childKey], childKey, depth + 1);
                }
            }
        }

        public bool ParseKeychain(string keychain, out BuildReleaseType releaseType, out BuildPlatform platform, out BuildTarget target,
            out BuildScriptingBackend scriptingBackend, out BuildDistribution distribution)
        {
            bool success = false;
            string[] keys = keychain.Split('/');
            int keyCount = keys.Length;
            int targetKey = 0;

            releaseType = null;
            platform = null;
            target = null;
            distribution = null;
            scriptingBackend = null;

            // Parse release type.
            if (keyCount > targetKey)
            {
                for (int i = 0; i < BuildSettings.releaseTypeList.releaseTypes.Length; i++)
                {
                    BuildReleaseType rt = BuildSettings.releaseTypeList.releaseTypes[i];

                    if (keys[targetKey] == rt.typeName)
                    {
                        releaseType = rt;
                        break;
                    }
                }
            }

            if (releaseType == null)
                return false;

            // Parse platform.
            if (keyCount > ++targetKey)
            {
                // Scan ahead and try to parse a variant key.
                string variantKey = "";
                if (keys[targetKey + 1].Contains("("))
                {
                    int startIndex = keys[targetKey + 1].IndexOf('(');
                    int endIndex = keys[targetKey + 1].IndexOf(')');
                    variantKey = keys[targetKey + 1].Substring(startIndex + 1, endIndex - startIndex - 1);

                    keys[targetKey + 1] = keys[targetKey + 1].Remove(startIndex).Trim();
                }

                for (int i = 0; i < BuildSettings.platformList.platforms.Count; i++)
                {
                    BuildPlatform p = BuildSettings.platformList.platforms[i];

                    if (keys[targetKey] == p.platformName && p.variantKey == variantKey)
                    {
                        platform = p;
                        break;
                    }
                }
            }

            if (platform == null)
                return false;

            // Parse target.
            if (platform.targets.Length == 1)
            {
                // Only one target, so it won't even appear in dictionary. Just get it directly.
                ++targetKey;
                target = platform.targets[0];
                success = true;
            }
            else if (keyCount > ++targetKey)
            {
                for (int i = 0; i < platform.targets.Length; i++)
                {
                    BuildTarget t = platform.targets[i];

                    if (keys[targetKey] == t.name)
                    {
                        target = t;
                        success = true;
                        break;
                    }
                }
            }

            if (target == null)
                return false;

            // Parse scripting backend
            if (platform.scriptingBackends.Length == 0)
            {
                // If no scripting backends are available,
                // use Mono2x by default
                scriptingBackend = new BuildScriptingBackend(UnityEditor.ScriptingImplementation.Mono2x, true);
                success = true;
            }
            else if (keyCount > ++targetKey)
            {
                // Else search for existing backends
                success = false;
                for (int i = 0; i < platform.scriptingBackends.Length; i++)
                {
                    BuildScriptingBackend backend = platform.scriptingBackends[i];

                    if (keys[targetKey] == backend.name)
                    {
                        scriptingBackend = backend;
                        success = true;
                        break;
                    }
                }
            }

            if (scriptingBackend == null)
                return false;

            // TODO: Parse variants.

            // Parse distribution.
            if (keyCount > ++targetKey)
            {
                success = false;
                for (int i = 0; i < platform.distributionList.distributions.Length; i++)
                {
                    BuildDistribution d = platform.distributionList.distributions[i];

                    if (keys[targetKey] == d.distributionName)
                    {
                        distribution = d;
                        success = true;
                        break;
                    }
                }
            }

            return success;
        }

        private string[] RefreshPlatforms(string keyChain, ConfigDictionary refreshedConfigSet, ConfigDictionary prevConfigSet)
        {
            List<string> childKeys = new();

            List<BuildPlatform> platforms = BuildSettings.platformList.platforms;
            for (int i = 0; i < platforms.Count; i++)
            {
                // Skip if platform is disabled or if it doesn't have any
                // enabled targets or scripting backends
                if (!platforms[i].enabled || !platforms[i].atLeastOneTarget || !platforms[i].atLeastOneBackend)
                    continue;

                string key = keyChain + "/" + platforms[i].platformName;
                Configuration relConfig = new();

                // Check for duplicate key.
                if (refreshedConfigSet.ContainsKey(key))
                    continue;

                // Copy previous settings if they exist.
                if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
                {
                    relConfig.enabled = prevConfigSet[key].enabled;
                }

                // Refresh targets.
                BuildTarget[] targets = platforms[i].targets;
                BuildScriptingBackend[] scriptingBackends = platforms[i].scriptingBackends;
                if (targets.Length > 0)
                    relConfig.childKeys = RefreshTargets(key, refreshedConfigSet, platforms[i].variantKey, targets,
                        scriptingBackends, platforms[i].distributionList.distributions, prevConfigSet);

                // Scan ahead for other versions of this platform with different variants.
                for (int j = i; j < platforms.Count; j++)
                {
                    BuildPlatform otherPlatform = platforms[j];
                    if (otherPlatform.platformName == platforms[i].platformName && otherPlatform.enabled && otherPlatform.atLeastOneTarget
                        && otherPlatform.atLeastOneBackend)
                    {
                        List<string> currentKeys = new(relConfig.childKeys);
                        string[] additionalKeys = RefreshTargets(key, refreshedConfigSet, otherPlatform.variantKey, otherPlatform.targets,
                            otherPlatform.scriptingBackends, otherPlatform.distributionList.distributions, prevConfigSet);

                        for (int k = 0; k < additionalKeys.Length; k++)
                        {
                            if (!currentKeys.Contains(additionalKeys[k]))
                                currentKeys.Add(additionalKeys[k]);
                        }

                        relConfig.childKeys = currentKeys.ToArray();
                    }
                }

                // Save configuration.
                refreshedConfigSet.Add(key, relConfig);

                // Add key to list to send back to parent.
                childKeys.Add(key);
            }

            return childKeys.ToArray();
        }

        private string[] RefreshTargets(string keyChain, ConfigDictionary refreshedConfigSet, string variantKey, BuildTarget[] targets,
            BuildScriptingBackend[] scriptingBackends, BuildDistribution[] distributions, ConfigDictionary prevConfigSet)
        {
            List<string> childKeys = new();

            for (int i = 0; i < targets.Length; i++)
            {
                // Skip if target is disabled.
                if (!targets[i].enabled)
                    continue;

                string key = keyChain + "/" + targets[i].name;
                if (variantKey.Length > 0)
                    key += " (" + variantKey + ")";

                Configuration relConfig = new();

                // Check for a duplicate key.
                if (refreshedConfigSet.ContainsKey(key))
                    continue;

                // Copy previous settings if they exist.
                if (prevConfigSet != null && prevConfigSet.ContainsKey(key))
                {
                    relConfig.enabled = prevConfigSet[key].enabled;
                }

                // Refresh scripting backends
                if (scriptingBackends.Length > 0)
                {
                    relConfig.childKeys = RefreshBackends(key, refreshedConfigSet, scriptingBackends, distributions, prevConfigSet);
                }
                else
                {
                    // If scripting backends is empty, don't miss the distributions
                    relConfig.childKeys = RefreshDistributions(key, refreshedConfigSet, distributions, prevConfigSet);
                }

                // Save configuration.
                refreshedConfigSet.Add(key, relConfig);

                // Add key to list to send back to parent.
                childKeys.Add(key);
            }

            return childKeys.ToArray();
        }

        private string[] RefreshBackends(string keyChain, ConfigDictionary refreshedConfigSet, BuildScriptingBackend[] scriptingBackends,
            BuildDistribution[] distributions, ConfigDictionary prevConfigSet)
        {
            List<string> childKeys = new();

            for (int i = 0; i < scriptingBackends.Length; i++)
            {
                // Skip if scripting backend is disabled.
                if (!scriptingBackends[i].enabled)
                    continue;

                string key = keyChain + "/" + scriptingBackends[i].name;

                Configuration relConfig = new();

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
                {
                    relConfig.childKeys = RefreshDistributions(key, refreshedConfigSet, distributions, prevConfigSet);
                }

                // Save configuration.
                refreshedConfigSet.Add(key, relConfig);

                // Add key to list to send back to parent.
                childKeys.Add(key);
            }

            return childKeys.ToArray();
        }

        private string[] RefreshDistributions(string keyChain, ConfigDictionary refreshedConfigSet, BuildDistribution[] distributions, ConfigDictionary prevConfigSet)
        {
            List<string> childKeys = new();

            for (int i = 0; i < distributions.Length; i++)
            {
                if (!distributions[i].enabled)
                    continue;

                string key = keyChain + "/" + distributions[i].distributionName;
                Configuration relConfig = new();

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
