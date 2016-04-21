using System;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace UnityBuild
{

[InitializeOnLoad]
public abstract class BuildSettings
{
    #region Abstract

    /// <summary>
    /// The name of executable file (e.g. mygame.exe, mygame.app)
    /// </summary>
    public abstract string binName { get; }

    /// <summary>
    /// The base path where builds are output. Relative to the Unity project's base folder.
    /// </summary>
    public abstract string binPath { get; }

    /// <summary>
    /// A list of scenes to include in the build. The first listed scene will be loaded first.
    /// </summary>
    public abstract string[] scenesInBuild { get; }

    /// <summary>
    /// A list of files/directories to include with the build. Relative to the Unity project's base folder.
    /// </summary>
    public abstract string[] copyToBuild { get; }

    #endregion

    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static BuildSettings()
    {
        // Find all classes that inherit from BuildPlatform and register them with BuildProject.
        Type ti = typeof(BuildSettings);

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    BuildProject.RegisterSettings((BuildSettings)Activator.CreateInstance(t));
                }
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Method executed before doing any build actions.
    /// </summary>
    public virtual void PreBuild()
    {
    }

    /// <summary>
    /// Method executed after performing all standard build actions.
    /// </summary>
    public virtual void PostBuild()
    {
    }

    /// <summary>
    /// Method executed prior to doing the build for a particular platform.
    /// </summary>
    /// <param name="platform"></param>
    public virtual void PreBuild(BuildPlatform platform)
    {
    }

    /// <summary>
    /// Method executed after doing the build for a particular platform.
    /// </summary>
    /// <param name="platform"></param>
    public virtual void PostBuild(BuildPlatform platform)
    {
    }

    #endregion
}

}