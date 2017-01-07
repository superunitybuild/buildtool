using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[InitializeOnLoad]
public abstract class PostBuildAction : BuildAction
{
    public static List<Type> postBuildActions = new List<Type>();

    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static PostBuildAction()
    {
        // Find all classes that inherit from PostBuildAction and register them.
        Type ti = typeof(PostBuildAction);

        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in asm.GetTypes())
            {
                if (ti.IsAssignableFrom(t) && ti != t)
                {
                    postBuildActions.Add(t);
                }
            }
        }
    }

    #endregion
}

}
