using System;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BasicSettings
    {
        [FilePath(true, true, "Choose location for build output")]
        public string baseBuildFolder = "Builds";
        [Tooltip("Recognized tokens for the build path: $YEAR, $MONTH, $DAY, $TIME, $RELEASE_TYPE, $PLATFORM, $ARCHITECTURE, $VARIANTS, $DISTRIBUTION, $VERSION, $BUILD, $PRODUCT_NAME, $SCRIPTING_BACKEND")] public string buildPath = "$VERSION/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/$SCRIPTING_BACKEND";
        public bool openFolderPostBuild = true;
    }
}
