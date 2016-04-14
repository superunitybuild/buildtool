using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace UnityBuild
{

public abstract class BuildPlatform
{
    public abstract BuildTarget target { get; }
    public abstract string name { get; }
    public abstract string binaryNameFormat { get; }
    public abstract string dataDirNameFormat { get; }

    public abstract void Build();

    protected static void Toggle(string targetName)
    {
        EditorPrefs.SetBool("buildGame" + targetName, !EditorPrefs.GetBool("buildGame" + targetName, false));
    }

    protected static bool ToggleValidate(string targetName)
    {
        Menu.SetChecked("Build/" + targetName, EditorPrefs.GetBool("buildGame" + targetName, false));
        return true;
    }

    protected static void Build(BuildTarget targetType, string targetName, string binaryNameFormat, string dataDirNameFormat)
    {
        if (EditorPrefs.GetBool("buildGame" + targetName, false))
        {
            BuildProject.PerformBuild(targetType, binaryNameFormat, dataDirNameFormat, targetName);
        }
    }

    
}

}