using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperSystems.UnityBuild
{

public class BaseSettings : ScriptableObject
{
    protected const string SettingsPath = "Assets/Editor Default Resources/UnityBuildSettings/";

    protected static T CreateAsset<T>(string assetName) where T : BaseSettings
    {
        T instance = EditorGUIUtility.Load("UnityBuildSettings/" + assetName + ".asset") as T;
        if (instance == null)
        {    
            Debug.Log("UnityBuild: Creating settings file - " + SettingsPath + assetName + ".asset");
            instance = CreateInstance<T>();
            instance.name = assetName;

            if (!Directory.Exists("Assets/Editor Default Resources"))
                AssetDatabase.CreateFolder("Assets", "Editor Default Resources");

            if (!Directory.Exists("Assets/Editor Default Resources/UnityBuildSettings"))
                AssetDatabase.CreateFolder("Assets/Editor Default Resources", "UnityBuildSettings");

            AssetDatabase.CreateAsset(instance, SettingsPath + assetName + ".asset");
        }

        return instance;
    }
}

}