
namespace SuperSystems.UnityBuild
{

public static class BuildCLI
{
    public static void PerformBuild()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        BuildProject.BuildAll();
    }

}

}