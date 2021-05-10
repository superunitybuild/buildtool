using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class ProductParameters
    {
        [Tooltip("Counter that is incremented whenever a build version is generated. Available for use in the version template via the $BUILD token")] public int buildCounter = 0;
        [Tooltip("The version string that will be returned by Application.version in builds. It will be updated using the version template whenever a new version is generated"), FormerlySerializedAs("lastGeneratedVersion")] public string buildVersion = string.Empty;
        [Tooltip("Recognized tokens for the version template: $ADJECTIVE, $NOUN, $YEAR, $MONTH, $DAY, $TIME, $DAYSSINCE(\"Date\"), $SECONDS, $BUILD"), FormerlySerializedAs("version")] public string versionTemplate = "1.0.0.$BUILD";
        [Tooltip("If enabled, a new version will be generated each time a build is performed")] public bool autoGenerate = true;
        [Tooltip("If enabled, the build version will be kept in sync with the version string from Player Settings")] public bool syncWithPlayerSettings = false;

        [Obsolete("Use buildVersion instead")]
        public string lastGeneratedVersion => buildVersion;
        [Obsolete("Use versionTemplate instead")]
        public string version => versionTemplate;
    }
}
