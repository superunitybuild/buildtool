using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class SceneList
    {
        public List<Scene> releaseScenes = new List<Scene>();

        public SceneList()
        {
        }

        public void Refresh()
        {
            // Verify that all scenes in list still exist.
            for (int i = 0; i < releaseScenes.Count; i++)
            {
                string sceneGUID = releaseScenes[i].fileGUID;
                string sceneFilepath = AssetDatabase.GUIDToAssetPath(sceneGUID);

                if (string.IsNullOrEmpty(sceneFilepath) || !File.Exists(sceneFilepath))
                {
                    releaseScenes.RemoveAt(i);
                    --i;
                }
            }
        }

        public string[] GetActiveSceneFileList()
        {
            List<string> scenes = new List<string>();
            for (int i = 0; i < releaseScenes.Count; i++)
            {
                var thisScene = releaseScenes[i];
                if(!thisScene.sceneActive)
                {
                    //Don't return inactive scenes
                    continue;
                }
                scenes.Add(SceneGUIDToPath(thisScene.fileGUID));
            }

            return scenes.ToArray();
        }

        public string SceneGUIDToPath(string guid)
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }

        [Serializable]
        public class Scene
        {
            /// <summary>
            /// File location of the scene starting from Project Root
            /// </summary>
            public string fileGUID = string.Empty;
            /// <summary>
            /// Whether the scene should be included in the final build.
            /// A user could leave this toggle in order to easily add/remove it later.
            /// </summary>
            public bool sceneActive = true;
        }
    }
}
