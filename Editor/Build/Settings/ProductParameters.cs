using System;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class ProductParameters
    {
        public int buildCounter = 0;
        public string lastGeneratedVersion = string.Empty;

        [Tooltip("Recognized tokens for the version: $ADJECTIVE, $NOUN, $YEAR, $MONTH, $DAY, $TIME, $DAYSSINCE(\"Date\"), $SECONDS, $BUILD")] public string version = "1.0.0.$BUILD";
        public bool autoGenerate = true;
        public bool syncWithPlayerSettings = false;
    }
}
