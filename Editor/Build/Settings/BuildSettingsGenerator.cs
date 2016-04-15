using UnityEditor;

namespace UnityBuild
{

public static class BuildSettingsGenerator
{
    [MenuItem("Build/Generate/BuildSettings", false, 100)]
    private static void GenerateBuildSettings()
    {
        Generator.Generate("ProjectBuildSettings.cs", "BuildSettings", "BuildSettingsTemplate");
    }
}

}