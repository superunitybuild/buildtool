using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[InitializeOnLoad]
public abstract class PreBuildAction : BuildAction
{
    public static List<Type> preBuildActions = new List<Type>();

    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static PreBuildAction()
    {
        // Find all classes that inherit from PreBuildAction and register them.
        Type ti = typeof(PreBuildAction);

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    preBuildActions.Add(t);
                }
            }
        }
    }

    #endregion
}

}