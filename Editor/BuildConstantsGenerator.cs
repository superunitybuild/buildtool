using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public static class BuildConstantsGenerator
    {
        public const string NONE = "None";

        public const string FileName = "BuildConstants.cs";

        public static string FindFile()
        {
            string[] fileSearchResults = Directory.GetFiles(Application.dataPath, FileName, SearchOption.AllDirectories);
            string filePath = null;
            char[] separatorChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            for (int i = 0; i < fileSearchResults.Length; i++)
            {
                string thisFilePath = fileSearchResults[i];
                string[] thisFilePathSplit = thisFilePath.Split(separatorChars);
                if (thisFilePathSplit.Length > 0)
                {
                    if (thisFilePathSplit[^1].Equals(FileName))
                    {
                        filePath = thisFilePath;
                        break;
                    }
                }
            }

            return filePath;
        }

        public static void Generate(
            DateTime buildTime,
            string filePath = "",
            string currentVersion = "",
            int currentBuildCounter = 0,
            BuildReleaseType currentReleaseType = null,
            BuildPlatform currentBuildPlatform = null,
            BuildScriptingBackend currentScriptingBackend = null,
            BuildTarget currentBuildTarget = null,
            BuildDistribution currentBuildDistribution = null)
        {
            // Find the BuildConstants file.
            string currentFilePath = FindFile();
            string finalFileLocation = string.IsNullOrEmpty(currentFilePath) ? Path.Combine(filePath, FileName) : currentFilePath;

            // Generate strings
            string versionString = currentVersion;
            string releaseTypeString = currentReleaseType == null ? NONE : SanitizeString(currentReleaseType.typeName);
            string platformString = currentBuildPlatform == null ? NONE : SanitizeString(currentBuildPlatform.platformName);
            string scriptingBackendString = currentScriptingBackend == null ? NONE : SanitizeString(currentScriptingBackend.name);
            string targetString = currentBuildTarget == null ? NONE : SanitizeString(currentBuildTarget.name);
            string distributionString = currentBuildDistribution == null ? NONE : SanitizeString(currentBuildDistribution.distributionName);

            if (File.Exists(finalFileLocation))
            {
                // Delete existing version.
                File.Delete(finalFileLocation);
            }
            else
            {
                // Ensure desired path exists if generating for the first time.
                FileInfo fileInfo = new(finalFileLocation);
                if (!fileInfo.Directory.Exists)
                {
                    _ = Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
            }

            // Create a buffer that we'll use to check for any duplicated names.
            List<string> enumBuffer = new();

            using (StreamWriter writer = new(finalFileLocation))
            {
                // Start of file
                writer.WriteLine("using System;");
                writer.WriteLine("");
                writer.WriteLine("// This file is auto-generated. Do not modify or move this file.");
                writer.WriteLine();
                writer.WriteLine("namespace SuperUnityBuild.Generated");
                writer.WriteLine("{");

                // Write ReleaseType enum.
                writer.WriteLine("    public enum ReleaseType");
                writer.WriteLine("    {");
                writer.WriteLine("        {0},", NONE);
                enumBuffer.Add(NONE);
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

                // Validate ReleaseType string.
                if (!enumBuffer.Contains(releaseTypeString))
                    releaseTypeString = NONE;

                // Write Platform enum.
                enumBuffer.Clear();
                writer.WriteLine("    public enum Platform");
                writer.WriteLine("    {");
                writer.WriteLine("        {0},", NONE);
                enumBuffer.Add(NONE);
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

                // Validate Platform string.
                if (!enumBuffer.Contains(platformString))
                    platformString = NONE;

                // Write Scripting Backend enum.
                enumBuffer.Clear();
                writer.WriteLine("    public enum ScriptingBackend");
                writer.WriteLine("    {");
                writer.WriteLine("        {0},", NONE);
                enumBuffer.Add(NONE);
                foreach (BuildPlatform platform in BuildSettings.platformList.platforms)
                {
                    if (platform.enabled)
                    {
                        foreach (BuildScriptingBackend scriptingBackend in platform.scriptingBackends)
                        {
                            string addedString = SanitizeString(scriptingBackend.name);

                            if (scriptingBackend.enabled && !enumBuffer.Contains(addedString))
                            {
                                enumBuffer.Add(addedString);
                                writer.WriteLine("        {0},", addedString);
                            }
                        }
                    }
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                // Validate Scripting Backend string.
                if (!enumBuffer.Contains(scriptingBackendString))
                    scriptingBackendString = NONE;

                // Write Target enum.
                enumBuffer.Clear();
                writer.WriteLine("    public enum Target");
                writer.WriteLine("    {");
                writer.WriteLine("        {0},", NONE);
                enumBuffer.Add(NONE);
                foreach (BuildPlatform platform in BuildSettings.platformList.platforms)
                {
                    if (platform.enabled)
                    {
                        foreach (BuildTarget target in platform.targets)
                        {
                            string addedString = SanitizeString(target.name);

                            if (target.enabled && !enumBuffer.Contains(addedString))
                            {
                                enumBuffer.Add(addedString);
                                writer.WriteLine("        {0},", addedString);
                            }
                        }
                    }
                }
                writer.WriteLine("    }");
                writer.WriteLine();

                // Validate Target string.
                if (!enumBuffer.Contains(targetString))
                    targetString = NONE;

                // Write Distribution enum.
                enumBuffer.Clear();
                writer.WriteLine("    public enum Distribution");
                writer.WriteLine("    {");
                writer.WriteLine("        {0},", NONE);
                enumBuffer.Add(NONE);
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

                // Validate Distribution string.
                if (!enumBuffer.Contains(distributionString))
                    distributionString = NONE;

                // Start of class.
                writer.WriteLine("    public static class BuildConstants");
                writer.WriteLine("    {");

                // Write current values.
                writer.WriteLine("        public static readonly DateTime buildDate = new DateTime({0});", buildTime.Ticks);
                writer.WriteLine("        public const string version = \"{0}\";", versionString);
                writer.WriteLine("        public const int buildCounter = {0};", currentBuildCounter);
                writer.WriteLine("        public const ReleaseType releaseType = ReleaseType.{0};", releaseTypeString);
                writer.WriteLine("        public const Platform platform = Platform.{0};", platformString);
                writer.WriteLine("        public const ScriptingBackend scriptingBackend = ScriptingBackend.{0};", scriptingBackendString);
                writer.WriteLine("        public const Target target = Target.{0};", targetString);
                writer.WriteLine("        public const Distribution distribution = Distribution.{0};", distributionString);

                // End of class.
                writer.WriteLine("    }");
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
