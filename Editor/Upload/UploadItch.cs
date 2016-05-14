using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperSystems.UnityBuild
{

public class UploadItch : PostBuildAction
{
    private const string WINDOWS = "windows";
    private const string OSX = "osx";
    private const string LINUX = "linux";

    #region MenuItems

    [MenuItem("Build/Upload/itch.io/Execute", false, 50)]
    private static void UploadAll()
    {
        for (int i = 0; i < BuildProject.platforms.Count; i++)
        {
            BuildPlatform platform = BuildProject.platforms[i];
            PerformUpload(platform);
        }
    }

    [MenuItem("Build/Upload/itch.io/Auto Upload")]
    private static void ToggleAutoUpload()
    {
        EditorPrefs.SetBool("buildUploadItchAuto", !EditorPrefs.GetBool("buildUploadItchAuto", false));
    }

    [MenuItem("Build/Upload/itch.io/Auto Upload", true)]
    private static bool ToggleAutoUploadValidate()
    {
        Menu.SetChecked("Build/Upload/itch.io/Auto Upload", EditorPrefs.GetBool("buildUploadItchAuto", false));
        return true;
    }

    #endregion

    #region Public Methods

    public override void Execute(BuildPlatform platform)
    {
        if (EditorPrefs.GetBool("buildUploadItchAuto", false))
            PerformUpload(platform);
    }

    #endregion

    #region Private Methods

    private static void PerformUpload(BuildPlatform platform)
    {
        //if (!platform.buildEnabled)
        //    return;

        //string absolutePath = Path.GetFullPath(platform.buildPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        //if (File.Exists(absolutePath))
        //{
        //    Debug.Log("UploadItch: Upload Failed - Build does not exist for platform " + platform.name + " - " + absolutePath);
        //    return;
        //}

        //string channel = GetChannelName(platform.target);
        //if (string.IsNullOrEmpty(channel))
        //{
        //    Debug.Log("UploadItch: Upload Failed - Unknown platform " + platform.name);
        //    return;
        //}

        //string arguments = "push \"" + absolutePath + "\" " + UploadItchSettings.itchUserName + "/" + UploadItchSettings.itchGameName + ":" + channel;

        //if (!string.IsNullOrEmpty(UploadItchSettings.versionNumber))
        //{
        //    arguments += "--userversion " + UploadItchSettings.versionNumber;
        //}

        //System.Diagnostics.Process uploadProc = new System.Diagnostics.Process();
        //uploadProc.StartInfo.FileName = UploadItchSettings.butlerPath;
        //uploadProc.StartInfo.Arguments =
        //    arguments;
        //uploadProc.StartInfo.CreateNoWindow = false;
        //uploadProc.StartInfo.UseShellExecute = false;
        //uploadProc.Start();
    }

    private static string GetChannelName(BuildTarget target)
    {
        switch (target)
        {
            // Windows
            case BuildTarget.StandaloneWindows:
                return WINDOWS + "-x86";
            case BuildTarget.StandaloneWindows64:
                return WINDOWS + "-x64";

            // Linux
            case BuildTarget.StandaloneLinux:
                return LINUX + "-x86";
            case BuildTarget.StandaloneLinux64:
                return LINUX + "-x64";
            case BuildTarget.StandaloneLinuxUniversal:
                return LINUX + "-universal";

            // OSX
            case BuildTarget.StandaloneOSXIntel:
                return OSX + "-intel";
            case BuildTarget.StandaloneOSXIntel64:
                return OSX + "-intel64";
            case BuildTarget.StandaloneOSXUniversal:
                return OSX + "-universal";
            
            default:
                return null;
        }
    }

    #endregion
}

}