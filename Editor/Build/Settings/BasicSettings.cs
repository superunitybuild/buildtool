using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BasicSettings
{
    public string baseBuildFolder = "bin";
    public string buildPath = "$YEAR-$MONTH-$DAY-$TIME/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/";
    public bool openFolderPostBuild = true;
}

}