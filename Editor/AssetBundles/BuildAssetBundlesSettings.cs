using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityBuild
{

[InitializeOnLoad]
public class BuildAssetBundlesSettings : BaseSettings
{
    #region Singleton

    private static BuildAssetBundlesSettings instance = null;

    public static BuildAssetBundlesSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateAsset<BuildAssetBundlesSettings>("BuildAssetBundlesSettings");
            }

            return instance;
        }
    }

    #endregion

    #region MenuItems

    [MenuItem("Build/AssetBundles/Edit Settings", priority = 0)]
    public static void EditSettings()
    {
        Selection.activeObject = Instance;
        EditorApplication.ExecuteMenuItem("Window/Inspector");
    }

    #endregion
    
    #region Variables

    [Header("AssetBundle Build Settings (Field Info in Tooltips)")]

    /// <summary>
    /// The path where AssetBundles are built. {0} = binPath
    /// </summary>
    [SerializeField]
    [Tooltip("The path where AssetBundles are built. {0} = binPath")]
    private string _buildPath = "{0}/Bundles";

    /// <summary>
    /// Flag indicating if the AssetBundles should be copied into the game's data directory.
    /// </summary>
    [SerializeField]
    [Tooltip("Flag indicating if the AssetBundles should be copied into the game's data directory.")]
    private bool _copyToBuild = true;

    #endregion

    #region Public Properties

    public static string buildPath
    {
        get
        {
            return string.Format(Instance._buildPath, BuildSettings.binPath);
        }
    }

    public static bool copyToBuild
    {
        get
        {
            return Instance._copyToBuild;
        }
    }

    #endregion
}

}