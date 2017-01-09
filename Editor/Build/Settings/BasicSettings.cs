using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BasicSettings
{
    [FilePath(true, true, "Choose location for build output")]
    public string baseBuildFolder = "bin";
    public string buildPath = "$YEAR-$MONTH-$DAY/$BUILD/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/$DISTRIBUTION";
    public bool openFolderPostBuild = true;
}

}