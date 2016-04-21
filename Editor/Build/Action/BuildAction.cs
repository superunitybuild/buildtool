using System;
using UnityEngine;
using UnityEditor;

namespace UnityBuild
{

public abstract class BuildAction : IComparable<BuildAction>
{
    /// <summary>
    /// Build action.
    /// </summary>
    public virtual void Execute()
    {
    }

    /// <summary>
    /// Platform-specific build action.
    /// </summary>
    /// <param name="platform"></param>
    public virtual void Execute(BuildPlatform platform)
    {
    }

    /// <summary>
    /// Priority of this build action. Lower values execute earlier.
    /// </summary>
    public virtual int priority
    {
        get
        {
            return 100;
        }
    }

    #region IComparable

    public int CompareTo(BuildAction other)
    {
        if (other == null)
            return 1;
        else
            return priority.CompareTo(other.priority);
    }

    #endregion
}

}