
namespace SuperSystems.UnityBuild
{

public class BuildFilter
{
    public enum FilterCondition
    {
        Any,
        One,
        All
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
        Contains
    }

    FilterCondition condition;
    FilterClause[] clauses;

    public bool Evaluate(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, string configKeychain)
    {
        return false;
    }

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
            if (comparison == FilterComparison.Equals)
            {
                return targetString.Equals(test, System.StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return targetString.ToUpper().Contains(test);
            }
        }
    }
}

}
