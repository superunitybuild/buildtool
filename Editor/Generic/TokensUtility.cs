using System;
using System.Text;

namespace SuperUnityBuild.BuildTool
{
    public static class TokensUtility
    {
        public static string ResolveBuildConfigurationTokens(string prototype, BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime? buildTime)
        {
            prototype = ResolveBuildVersionTokens(prototype);

            if (buildTime != null)
                prototype = ResolveBuildTimeTokens(prototype, (DateTime)buildTime);

            StringBuilder sb = new StringBuilder(prototype);

            string variants = "";
            if (platform.variants != null && platform.variants.Length > 0)
                variants = platform.variantKey.Replace(",", ", ");

            sb.Replace("$RELEASE_TYPE", releaseType?.typeName.SanitizeFolderName());
            sb.Replace("$PLATFORM", platform?.platformName.SanitizeFolderName());
            sb.Replace("$ARCHITECTURE", architecture?.name.SanitizeFolderName());
            sb.Replace("$VARIANTS", variants.SanitizeFolderName());
            sb.Replace("$DISTRIBUTION", distribution?.distributionName.SanitizeFolderName());
            sb.Replace("$PRODUCT_NAME", releaseType?.productName.SanitizeFolderName());
            sb.Replace("$SCRIPTING_BACKEND", scriptingBackend?.name.SanitizeFolderName());

            return sb.ToString();
        }

        public static string ResolveBuildNumberToken(string prototype)
        {
            return (prototype ?? "")
                .Replace("$BUILD", BuildSettings.productParameters.buildCounter.ToString());
        }

        public static string ResolveBuildOutputTokens(string prototype, string buildPath)
        {
            return (prototype ?? "")
                .Replace("$BUILDPATH", buildPath)
                .Replace("$BASEPATH", BuildSettings.basicSettings.baseBuildFolder);
        }

        public static string ResolveBuildTimeTokens(string prototype, DateTime buildTime)
        {
            return (prototype ?? "")
                .Replace("$YEAR", buildTime.ToString("yyyy"))
                .Replace("$MONTH", buildTime.ToString("MM"))
                .Replace("$DAY", buildTime.ToString("dd"))
                .Replace("$TIME", buildTime.ToString("hhmmss"));
        }

        public static string ResolveBuildVersionToken(string prototype)
        {
            return (prototype ?? "")
                .Replace("$VERSION", BuildSettings.productParameters.buildVersion.SanitizeFolderName());
        }

        public static string ResolveBuildVersionTokens(string prototype)
        {
            prototype = prototype ?? "";

            prototype = ResolveBuildVersionToken(prototype);
            prototype = ResolveBuildNumberToken(prototype);

            return prototype;
        }
    }
}
