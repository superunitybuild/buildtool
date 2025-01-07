using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public static class TokensUtility
    {
        public static string ResolveBuildConfigurationTokens(string prototype, BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime? buildTime)
        {
            prototype = ResolveBuildVersionTokens(prototype);

            if (buildTime != null)
                prototype = ResolveBuildTimeTokens(prototype, (DateTime)buildTime);

            StringBuilder sb = new(prototype);

            string variants = "";
            if (platform.variants != null && platform.variants.Length > 0)
                variants = platform.variantKey.Replace(",", ", ");

            _ = sb.Replace("$RELEASE_TYPE", releaseType?.typeName.SanitizeFolderName());
            _ = sb.Replace("$PLATFORM", platform?.platformName.SanitizeFolderName());
            _ = sb.Replace("$TARGET", target?.name.SanitizeFolderName());
            _ = sb.Replace("$VARIANTS", variants.SanitizeFolderName());
            _ = sb.Replace("$DISTRIBUTION", distribution?.distributionName.SanitizeFolderName());
            _ = sb.Replace("$PRODUCT_NAME", releaseType?.productName.SanitizeFolderName());
            _ = sb.Replace("$SCRIPTING_BACKEND", scriptingBackend?.name.SanitizeFolderName());

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

        public static string ResolveBuildTimeUtilityTokens(string prototype, DateTime buildTime)
        {
            StringBuilder sb = new(prototype ?? "");

            // Regex = (?:\$DAYSSINCE\(")([^"]*)(?:"\))
            Match match = Regex.Match(prototype, "(?:\\$DAYSSINCE\\(\")([^\"]*)(?:\"\\))");
            while (match.Success)
            {
                int daysSince = DateTime.TryParse(match.Groups[1].Value, out DateTime parsedTime) ?
                    buildTime.Subtract(parsedTime).Days :
                    0;

                _ = sb.Replace(match.Captures[0].Value, daysSince.ToString());
                match = match.NextMatch();
            }

            _ = sb.Replace("$SECONDS", (buildTime.TimeOfDay.TotalSeconds / 15f).ToString("F0"));

            return sb.ToString();
        }

        public static string ResolveBuildVersionToken(string prototype)
        {
            return (prototype ?? "")
                .Replace("$VERSION", BuildSettings.productParameters.buildVersion.SanitizeFolderName());
        }

        public static string ResolveBuildVersionTokens(string prototype)
        {
            prototype ??= "";

            prototype = ResolveBuildVersionToken(prototype);
            prototype = ResolveBuildNumberToken(prototype);

            return prototype;
        }

        public static string ResolveBuildWordTokens(string prototype)
        {
            prototype ??= "";

            prototype = ReplaceTokenFromFile(prototype, "$NOUN", "nouns.txt");
            prototype = ReplaceTokenFromFile(prototype, "$ADJECTIVE", "adjectives.txt");

            return prototype;
        }

        private static string ReplaceTokenFromFile(string prototype, string token, string filename)
        {
            prototype ??= "";

            if (prototype.IndexOf(token) > -1)
            {
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Packages/{Constants.PackageName}/Editor/{filename}");

                if (textAsset != null)
                {
                    string[] lines = textAsset.text.Split("\n");
                    int index = prototype.IndexOf(token, 0);
                    StringBuilder sb = new(prototype);

                    while (index > -1)
                    {
                        string noun = lines[UnityEngine.Random.Range(0, lines.Length - 1)].ToUpperInvariant();

                        _ = sb.Replace(token, noun, index, token.Length);

                        prototype = sb.ToString();
                        index = prototype.IndexOf(token, index + 1);
                    }
                }
            }

            return prototype;
        }
    }
}
