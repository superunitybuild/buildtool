
using System;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildFilter
    {
        #region Constants and Enums

        public enum FilterCondition
        {
            Any,
            ExactlyOne,
            All,
            None
        }

        public enum FilterType
        {
            ReleaseType,
            Platform,
            Architecture,
            Distribution,
            FullConfigurationKey
        }

        public enum FilterComparison
        {
            Equals,
            Contains,
            NotEqual,
            DoesNotContain
        }

        #endregion

        public FilterCondition condition;
        public FilterClause[] clauses;

        public bool Evaluate(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, string configKeychain)
        {
            if (clauses == null || clauses.Length == 0)
                return true;

            // Set default state for success based on condition type.
            bool success = true;
            if (condition == FilterCondition.Any ||
                condition == FilterCondition.ExactlyOne)
            {
                success = false;
            }

            for (int i = 0; i < clauses.Length; i++)
            {
                if (condition == FilterCondition.Any)
                {
                    // Succeed as soon as any test evaluates true.
                    success |= clauses[i].Evaluate(releaseType, platform, architecture, distribution, configKeychain);
                    if (success)
                        break;
                }
                else if (condition == FilterCondition.All)
                {
                    // Succeed only if all tests evaluate true.
                    success &= clauses[i].Evaluate(releaseType, platform, architecture, distribution, configKeychain);
                    if (!success)
                        break;
                }
                else if (condition == FilterCondition.None)
                {
                    // Succeed only if all tests fail.
                    success &= !(clauses[i].Evaluate(releaseType, platform, architecture, distribution, configKeychain));
                    if (!success)
                        break;
                }
                else if (condition == FilterCondition.ExactlyOne)
                {
                    // Succeed only if exactly one test evaluates true.
                    if (clauses[i].Evaluate(releaseType, platform, architecture, distribution, configKeychain))
                    {
                        if (success)
                        {
                            // Another test already succeeded, so this is a failure.
                            success = false;
                            break;
                        }
                        else
                        {
                            success = true;
                        }
                    }
                }
            }

            return success;
        }

        [Serializable]
        public class FilterClause
        {
            public FilterType type;
            public FilterComparison comparison;
            public string test;

            public bool Evaluate(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, string configKeychain)
            {
                bool success = false;

                test = test.Trim().ToUpper();

                switch (type)
                {
                    case FilterType.ReleaseType:
                        success = PerformTest(releaseType.typeName);
                        break;
                    case FilterType.Platform:
                        success = PerformTest(platform.platformName);
                        break;
                    case FilterType.Architecture:
                        success = PerformTest(architecture.name);
                        break;
                    case FilterType.Distribution:
                        success = PerformTest(distribution.distributionName);
                        break;
                    case FilterType.FullConfigurationKey:
                        success = PerformTest(configKeychain);
                        break;
                }

                return success;
            }

            private bool PerformTest(string targetString)
            {
                switch (comparison)
                {
                    case FilterComparison.Equals:
                        return targetString.Equals(test, StringComparison.OrdinalIgnoreCase);
                    case FilterComparison.NotEqual:
                        return !(targetString.Equals(test, StringComparison.OrdinalIgnoreCase));
                    case FilterComparison.Contains:
                        return targetString.ToUpper().Contains(test);
                    case FilterComparison.DoesNotContain:
                        return !(targetString.ToUpper().Contains(test));
                    default:
                        return false;
                }
            }
        }
    }
}
