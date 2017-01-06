using System;
using System.Reflection;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[InitializeOnLoad]
public abstract class PreBuildAction : BuildAction
{
    #region Contructor

    /// <summary>
    /// Constructor
    /// </summary>
    static PreBuildAction()
    {
        //// Find all classes that inherit from PreBuildAction and register them with BuildProject.
        //Type ti = typeof(PreBuildAction);

        //foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        //{
        //    foreach (Type t in asm.GetTypes())
        //    {
        //        if (ti.IsAssignableFrom(t) && ti != t)
        //        {
        //            BuildProject.RegisterPreBuildAction((BuildAction)Activator.CreateInstance(t));
        //        }
        //    }
        //}
    }

    #endregion
}

}