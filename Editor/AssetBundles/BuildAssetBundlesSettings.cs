using System.IO;

namespace UnityBuild
{

public class BuildAssetBundlesSettings
{
    #region Constants

    /// <summary>
    /// The path where AssetBundles are built. {0} = binPath
    /// </summary>
    private const string _buildPath = "{0}/Bundles";

    /// <summary>
    /// Flag indicating if the AssetBundles should be copied into the game's data directory.
    /// </summary>
    private const bool _copyToBuild = true;

    #endregion

    #region Public Properties

    public static string buildPath
    {
        get
        {
            return string.Format(_buildPath, BuildProject.settings.binPath);
        }
    }

    public static bool copyToBuild
    {
        get
        {
            return _copyToBuild;
        }
    }

    #endregion
}

}