using System;
using System.Reflection;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[InitializeOnLoad]
public abstract class PostBuildAction : BuildAction
{
    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static PostBuildAction()
    {
        //// Find all classes that inherit from PostBuildAction and register them with BuildProject.
        //Type ti = typeof(PostBuildAction);

        //foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        //{
        //    foreach (Type t in asm.GetTypes())
        //    {
        //        if (ti.IsAssignableFrom(t) && ti != t)
        //        {
        //            BuildProject.RegisterPostBuildAction((BuildAction)Activator.CreateInstance(t));
        //        }
        //    }
        //}
    }

    #endregion
}

}