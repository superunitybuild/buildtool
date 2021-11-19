using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BasicSettings
    {
        [FilePath(true, true, "Choose location for build output")]
        public string baseBuildFolder = "Builds";
        [Tooltip("Recognized tokens for the build path: $YEAR, $MONTH, $DAY, $TIME, $RELEASE_TYPE, $PLATFORM, $ARCHITECTURE, $VARIANTS, $DISTRIBUTION, $VERSION, $BUILD, $PRODUCT_NAME")] public string buildPath = "$VERSION/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE";
        public bool openFolderPostBuild = true;
    }
}
