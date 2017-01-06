using System;

namespace SuperSystems.UnityBuild
{

[Serializable]
public class ProductParameters
{
    public int buildCounter = 0;
    public string lastGeneratedVersion = string.Empty;

    // $NOUN, $ADJECTIVE, $DAYSSINCE("DATE"), $SECONDS, $BUILDS
    public string version = "1.0.$DAYSSINCE(\"January 1, 2015\").$SECONDS";
    public bool autoGenerate = true;
}

}