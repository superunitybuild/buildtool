using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityBuild
{

public static class Generator
{
    public static void Generate(string outputFilename, string className, string templateFilename)
    {
        string directory = EditorUtility.OpenFolderPanel("Choose location for " + outputFilename + " (MUST BE AN EDITOR DIRECTORY)", Application.dataPath, "");

        // If user cancelled, we can't do anything.
        if (string.IsNullOrEmpty(directory))
        {
            return;
        }
        else if (!directory.Contains(Path.AltDirectorySeparatorChar + "Editor") && !directory.Contains(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar))
        {
            Debug.LogError(className + " must be in an Editor directory/sub-directory");
            return;
        }

        string filePath = Path.Combine(directory, outputFilename);

        string templateFilePath = string.Empty;
        foreach (var file in Directory.GetFiles(Application.dataPath, "*.txt", SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(file) == templateFilename)
            {
                templateFilePath = file;
                break;
            }
        }

        File.WriteAllText(filePath, File.ReadAllText(templateFilePath));

        AssetDatabase.Refresh();
    }
}

}