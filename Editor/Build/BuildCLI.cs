using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    public static class BuildCLI
    {
        public static void PerformBuild()
        {
            //string[] args = System.Environment.GetCommandLineArgs();
            BuildProject.BuildAll();

            // Exit w/ 0 to indicate success.
            EditorApplication.Exit(0);
        }
    }
}
