using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace SuperSystems.UnityBuild
{

public static class BuildConstantsGenerator
{
    public static void Generate(
        string currentVersion = "",
        BuildReleaseType currentReleaseType = null,
        BuildPlatform currentBuildPlatform = null,
        BuildArchitecture currentBuildArchitecture = null,
        BuildDistribution currentBuildDistribution = null)
    {
        // Find the BuildConstants file.
        string[] fileSearchResults = AssetDatabase.FindAssets("BuildConstants.cs");
        string filePath = null;
        for (int i = 0; i < fileSearchResults.Length; i++)
        {
            if (fileSearchResults[i].EndsWith("UnityBuild/BuildConstants.cs"))
            {
                filePath = fileSearchResults[i];
                break;
            }
        }

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        // Delete any existing version.
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Start of file and class.
            writer.WriteLine("// This file is auto-generated. Do not modify or move this file.");
            writer.WriteLine();
            writer.WriteLine("public static class BuildConstants");
            writer.WriteLine("{");

            // Write ReleaseType enum.
            writer.WriteLine("    public enum ReleaseType");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildReleaseType releaseType in BuildSettings.instance._releaseTypeList.releaseTypes)
            {
                writer.WriteLine("        {0},", SanitizeString(releaseType.typeName));
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Platform enum.
            writer.WriteLine("    public enum Platform");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.instance._platformList.platforms)
            {
                writer.WriteLine("        {0},", SanitizeString(platform.platformName));
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Architecture enum.
            writer.WriteLine("    public enum Architecture");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.instance._platformList.platforms)
            {
                foreach (BuildArchitecture arch in platform.architectures)
                {
                    writer.WriteLine("        {0},", SanitizeString(arch.name));
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Distribution enum.
            writer.WriteLine("    public enum Distribution");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.instance._platformList.platforms)
            {
                foreach (BuildDistribution dist in platform.distributionList.distributions)
                {
                    writer.WriteLine("        {0},", SanitizeString(dist.distributionName));
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write current values.
            writer.WriteLine("    public const string version = \"{0}\";", currentVersion);
            writer.WriteLine("    public const ReleaseType releaseType = \"{0}\";", currentReleaseType == null ? "None" : currentReleaseType.typeName);
            writer.WriteLine("    public const Platform platform = \"{0}\";", currentBuildPlatform == null ? "None" : currentBuildPlatform.platformName);
            writer.WriteLine("    public const Architecture architecture = \"{0}\";", currentBuildArchitecture == null ? "None" : currentBuildArchitecture.name);
            writer.WriteLine("    public const Distribution distribution = \"{0}\";", currentBuildDistribution == null ? "None" : currentBuildDistribution.distributionName);

            // End of class.
            writer.WriteLine("}");
            writer.WriteLine();

            // Refresh AssetDatabse so that changes take effect.
            AssetDatabase.Refresh();
        }
    }

    private static string SanitizeString(string str)
    {
        str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }
        return str;
    }
}

}