using System.Collections.Generic;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class SceneList
{
    public List<Scene> enabledScenes = new List<Scene>();

    public SceneList()
    {
    }

    public void Refresh()
    {
        string[] allScenes = GetListOfAllScenes();

        // Verify that all scenes in list still exist.
        for (int i = 0; i < enabledScenes.Count; i++)
        {
            string sceneName = enabledScenes[i].filePath;
            bool sceneExists = false;
            for (int j = 0; j < allScenes.Length; j++)
            {
                if (string.Equals(sceneName, allScenes[j]))
                {
                    sceneExists = true;
                    break;
                }
            }

            if (!sceneExists)
            {
                enabledScenes.RemoveAt(i);
                --i;
            }
        }
    }

    public string[] GetSceneList()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < enabledScenes.Count; i++)
        {
            scenes.Add(enabledScenes[i].filePath);
        }

        return scenes.ToArray();
    }

    public static string[] GetListOfAllScenes()
    {
        string[] allScenesGUID = AssetDatabase.FindAssets("t:scene");
        string[] allScenes = new string[allScenesGUID.Length];
        for (int i = 0; i < allScenesGUID.Length; i++)
        {
            allScenes[i] = AssetDatabase.GUIDToAssetPath(allScenesGUID[i]);
        }

        return allScenes;
    }

    [System.Serializable]
    public class Scene
    {
        public string filePath = string.Empty;
    }
}

}