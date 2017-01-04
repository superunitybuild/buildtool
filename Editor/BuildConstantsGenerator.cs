using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

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
        string[] fileSearchResults = Directory.GetFiles(Application.dataPath, "BuildConstants.cs", SearchOption.AllDirectories);
        string filePath = null;
        string desiredFilePath = string.Format("UnityBuild{0}BuildConstants.cs", Path.DirectorySeparatorChar);
        for (int i = 0; i < fileSearchResults.Length; i++)
        {
            if (fileSearchResults[i].EndsWith(desiredFilePath))
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

        List<string> enumBuffer = new List<string>();

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
            foreach (BuildReleaseType releaseType in BuildSettings.releaseTypeList.releaseTypes)
            {
                string addedString = SanitizeString(releaseType.typeName);

                if (!enumBuffer.Contains(addedString))
                {
                    enumBuffer.Add(addedString);
                    writer.WriteLine("        {0},", addedString);
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Platform enum.
            enumBuffer.Clear();
            writer.WriteLine("    public enum Platform");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.platformList.platforms)
            {
                string addedString = SanitizeString(platform.platformName);

                if (platform.enabled && !enumBuffer.Contains(addedString))
                {
                    enumBuffer.Add(addedString);
                    writer.WriteLine("        {0},", addedString);
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Architecture enum.
            enumBuffer.Clear();
            writer.WriteLine("    public enum Architecture");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.platformList.platforms)
            {
                if (platform.enabled)
                {
                    foreach (BuildArchitecture arch in platform.architectures)
                    {
                        string addedString = SanitizeString(arch.name);

                        if (arch.enabled && !enumBuffer.Contains(addedString))
                        {
                            enumBuffer.Add(addedString);
                            writer.WriteLine("        {0},", addedString);
                        }
                    }
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write Distribution enum.
            enumBuffer.Clear();
            writer.WriteLine("    public enum Distribution");
            writer.WriteLine("    {");
            writer.WriteLine("        None,");
            foreach (BuildPlatform platform in BuildSettings.platformList.platforms)
            {
                if (platform.enabled)
                {
                    foreach (BuildDistribution dist in platform.distributionList.distributions)
                    {
                        string addedString = SanitizeString(dist.distributionName);

                        if (dist.enabled && !enumBuffer.Contains(addedString))
                        {
                            enumBuffer.Add(addedString);
                            writer.WriteLine("        {0},", addedString);
                        }
                    }
                }
            }
            writer.WriteLine("    }");
            writer.WriteLine();

            // Write current values.
            writer.WriteLine("    public const string version = \"{0}\";", currentVersion);
            writer.WriteLine("    public const ReleaseType releaseType = ReleaseType.{0};", currentReleaseType == null ? "None" : SanitizeString(currentReleaseType.typeName));
            writer.WriteLine("    public const Platform platform = Platform.{0};", currentBuildPlatform == null ? "None" : SanitizeString(currentBuildPlatform.platformName));
            writer.WriteLine("    public const Architecture architecture = Architecture.{0};", currentBuildArchitecture == null ? "None" : SanitizeString(currentBuildArchitecture.name));
            writer.WriteLine("    public const Distribution distribution = Distribution.{0};", currentBuildDistribution == null ? "None" : SanitizeString(currentBuildDistribution.distributionName));

            // End of class.
            writer.WriteLine("}");
            writer.WriteLine();
        }

        // Refresh AssetDatabse so that changes take effect.
        AssetDatabase.Refresh();
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