using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BasicSettings
{
    public string baseBuildFolder = "bin";
    public string buildPath = "$YEAR-$MONTH-$DAY/$BUILD/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/$DISTRIBUTION";
    public bool openFolderPostBuild = true;
}

}