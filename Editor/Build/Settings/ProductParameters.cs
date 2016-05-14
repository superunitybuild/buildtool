using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class ProductParameters
{
    public int buildCounter = 0;

    // $NOUN, $ADJECTIVE, $DAYSSINCE("DATE"), $SECONDS, $BUILDS
    public string version = "$ADJECTIVE $NOUN 1.0.$DAYSSINCE(\"January 1, 2015\").$SECONDS";
}

}