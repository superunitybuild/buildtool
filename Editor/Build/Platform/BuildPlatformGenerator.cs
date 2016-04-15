using UnityEditor;

namespace UnityBuild
{

public static class BuildPlatformGenerator
{
    [MenuItem("Build/Generate/BuildPlatform", false, 100)]
    private static void GenerateBuildSettings()
    {
        Generator.Generate("BuildCustomPlatform.cs", "BuildPlatform", "BuildPlatformTemplate");
    }
}

}