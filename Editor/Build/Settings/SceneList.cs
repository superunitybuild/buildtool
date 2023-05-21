using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class SceneList
    {
        public List<Scene> enabledScenes = new List<Scene>();

        public SceneList()
        {
        }

        public void Refresh()
        {
            // Verify that all scenes in list still exist.
            for (int i = 0; i < enabledScenes.Count; i++)
            {
                string sceneGUID = enabledScenes[i].fileGUID;
                string sceneFilepath = AssetDatabase.GUIDToAssetPath(sceneGUID);

                if (string.IsNullOrEmpty(sceneFilepath) || !File.Exists(sceneFilepath))
                {
                    enabledScenes.RemoveAt(i);
                    --i;
                }
            }
        }

        public string[] GetSceneFileList()
        {
            List<string> scenes = new List<string>();
            for (int i = 0; i < enabledScenes.Count; i++)
            {
                scenes.Add(AssetDatabase.GUIDToAssetPath(enabledScenes[i].fileGUID));
            }

            return scenes.ToArray();
        }

        [Serializable]
        public class Scene
        {
            public string fileGUID = string.Empty;
        }
    }
}
