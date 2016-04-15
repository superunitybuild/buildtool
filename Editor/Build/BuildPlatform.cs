using System;
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

    public abstract void Build();

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

    /// <summary>
    /// Perform build for platform.
    /// </summary>
    /// <param name="targetType">Platform type. Passed in from descendant class.</param>
    /// <param name="targetName">Platform name. Passed in from descendant class.</param>
    /// <param name="binaryNameFormat">Binary name format. Passed in from descendant class.</param>
    /// <param name="dataDirNameFormat">Data directory name format. Passed in from descendant class.</param>
    protected static void Build(BuildTarget targetType, string targetName, string binaryNameFormat, string dataDirNameFormat)
    {
        BuildProject.PerformBuild(targetType, binaryNameFormat, dataDirNameFormat, targetName);
    }

    #endregion
}

}