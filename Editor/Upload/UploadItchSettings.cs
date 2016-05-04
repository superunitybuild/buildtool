using UnityEngine;
using UnityEditor;

public class UploadItchSettings
{
    #region Constants

    private const string _butlerPath = @"D:\User\Chase\Downloads\game-dev\butler\butler.exe";
    private const string _itchUserName = "supersystems";
    private const string _itchGameName = "butler-test";

    #endregion

    #region Public Properties

    public static string versionNumber
    {
        get
        {
            return string.Empty;
        }
    }

    public static string butlerPath
    {
        get
        {
            return _butlerPath;
        }
    }

    public static string itchUserName
    {
        get
        {
            return _itchUserName;
        }
    }

    public static string itchGameName
    {
        get
        {
            return _itchGameName;
        }
    }

    #endregion
}