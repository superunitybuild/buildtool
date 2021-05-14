using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    [InitializeOnLoad]
    public sealed class BuildActionListUtility
    {
        public static List<Type> preBuildActions = new List<Type>();
        public static List<Type> postBuildActions = new List<Type>();

        #region Contructor

        /// <summary>
        /// Constructor
        /// </summary>
        static BuildActionListUtility()
        {
            // Find all classes that inherit from BuildAction and register them.
            Type ti = typeof(BuildAction);

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (ti.IsAssignableFrom(t) && ti != t)
                    {
                        // We've found a BuildAction, now check what interfaces it implements.
                        if (typeof(IPreBuildAction).IsAssignableFrom(t) ||
                            typeof(IPreBuildPerPlatformAction).IsAssignableFrom(t))
                        {
                            preBuildActions.Add(t);
                        }

                        if (typeof(IPostBuildAction).IsAssignableFrom(t) ||
                            typeof(IPostBuildPerPlatformAction).IsAssignableFrom(t))
                        {
                            postBuildActions.Add(t);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
