using System;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace UnityBuild
{

[InitializeOnLoad]
public abstract class BuildPlatform
{
    #region Abstract

    public abstract BuildTarget target { get; }
    public abstract string name { get; }
    public abstract string binaryNameFormat { get; }
    public abstract string dataDirNameFormat { get; }

    #endregion

    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static BuildPlatform()
    {
        // Find all classes that inherit from BuildPlatform and register them with BuildProject.
        Type ti = typeof(BuildPlatform);

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    BuildProject.RegisterPlatform((BuildPlatform)Activator.CreateInstance(t));
                }
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Perform build for platform.
    /// </summary>
    public void Build()
    {
        BuildProject.PerformBuild(this);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Toggle if a target platform should be built.
    /// </summary>
    /// <param name="targetName">Platform name. Passed in from descendant class.</param>
    protected static void Toggle(string targetName)
    {
        EditorPrefs.SetBool("buildGame" + targetName, !EditorPrefs.GetBool("buildGame" + targetName, false));
    }

    /// <summary>
    /// UI Validation for platform build setting.
    /// </summary>
    /// <param name="targetName">Platform name. Passed in from descendant class.</param>
    /// <returns></returns>
    protected static bool ToggleValidate(string targetName)
    {
        Menu.SetChecked("Build/Platforms/" + targetName, EditorPrefs.GetBool("buildGame" + targetName, false));
        return true;
    }

    #endregion

    #region Public Properties

    public bool buildEnabled
    {
        get
        {
            return EditorPrefs.GetBool("buildGame" + name, false);
        }
    }

    public string buildPath
    {
        get
        {
            return BuildProject.settings.binPath + Path.DirectorySeparatorChar + name + Path.DirectorySeparatorChar;
        }
    }

    public string dataDirectory
    {
        get
        {
            return buildPath + string.Format(dataDirNameFormat, BuildProject.settings.binName) + Path.DirectorySeparatorChar;
        }
    }

    public string exeName
    {
        get
        {
            return string.Format(binaryNameFormat, BuildProject.settings.binName);
        }
    }

    #endregion
}

}