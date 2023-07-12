using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [CustomEditor(typeof(BuildSettings))]
    public class BuildSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open in SuperUnityBuild", GUILayout.ExpandWidth(true), GUILayout.MinHeight(30)))
            {
                BuildSettings targetObj = (BuildSettings)this.target;
                if(targetObj != null)
                {
                    //Open this asset in the UnityBuildWindow
                    targetObj.OpenInUnityBuildWindow();
                }
            }
        }
    }
}
