using System;
using System.IO;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BasicSettings
    {
        [FilePath(true, true, "Choose location for build output")]
        public string baseBuildFolder = "Builds";
        [Tooltip("Recognized tokens for the build path: $YEAR, $MONTH, $DAY, $TIME, $RELEASE_TYPE, $PLATFORM, $TARGET, $VARIANTS, $DISTRIBUTION, $VERSION, $BUILD, $PRODUCT_NAME, $SCRIPTING_BACKEND")]
        public string buildPath = "$VERSION/$RELEASE_TYPE/$PLATFORM/$TARGET/$SCRIPTING_BACKEND";
        public bool openFolderPostBuild = true;
        [Tooltip("The folder path for the " + BuildConstantsGenerator.FileName + " file which will be generated on build. Use the Configure Editor Environment button on a selected configuration to generate it now.")]
        [FilePath(true, true, "Choose folder location for the " + BuildConstantsGenerator.FileName + " file")]
        public string constantsFileLocation = Path.Combine(Constants.AssetsDirectoryName, Constants.RootDirectoryName);
    }
}
