using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BasicSettings
{
    /// <summary>
    /// The base path where builds are output.
    /// Path is relative to the Unity project's base folder unless an absolute path is given.
    /// </summary>
    [Tooltip("The base path where all builds are created.")]
    public string baseBuildFolder = "bin";

    [Tooltip("The path for each new build. If not unique, previous build will be deleted.")]
    public string buildPath = "$YEAR-$MONTH-$DAY-$TIME/$RELEASE_TYPE/$PLATFORM/$ARCHITECTURE/";

    public bool openFolderPostBuild = true;
}

}