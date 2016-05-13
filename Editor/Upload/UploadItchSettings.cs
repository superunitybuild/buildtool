using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

public class UploadItchSettings : BaseSettings
{
    #region Singleton

    private static UploadItchSettings instance = null;

    public static UploadItchSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateAsset<UploadItchSettings>("UploadItchSettings");
            }

            return instance;
        }
    }

    public UploadItchSettings()
    {
#if UNITY_EDITOR_WIN
        _butlerPath = @"C:\game-dev\butler.exe";
#elif UNITY_EDITOR_OSX
        _butlerPath = @"/Users/username/game-dev/butler";
#endif
    }

    #endregion

    #region MenuItems

    [MenuItem("Build/Upload/itch.io/Edit Settings", priority = 0)]
    public static void EditSettings()
    {
        Selection.activeObject = Instance;
        EditorApplication.ExecuteMenuItem("Window/Inspector");
    }

    #endregion

    #region Variables

    [Header("Itch.io Upload Settings (Field Info in Tooltips)")]

    [SerializeField]
    [Tooltip("Path to butler executable.")]
    private string _butlerPath = "";

    [SerializeField]
    [Tooltip("itch.io username.")]
    private string _itchUserName = "username";

    [SerializeField]
    [Tooltip("itch.io project name.")]
    private string _itchGameName = "project";

    [SerializeField]
    [Tooltip("Upload version number (optional).")]
    private string _versionNumber = "";

    #endregion
    
    #region Public Properties

    public static string versionNumber
    {
        get
        {
            return Instance._versionNumber;
        }
        set
        {
            Instance._versionNumber = value;
        }
    }

    public static string butlerPath
    {
        get
        {
            return Instance._butlerPath;
        }
    }

    public static string itchUserName
    {
        get
        {
            return Instance._itchUserName;
        }
    }

    public static string itchGameName
    {
        get
        {
            return Instance._itchGameName;
        }
    }

    #endregion
}

}