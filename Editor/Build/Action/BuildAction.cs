using System;
using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public abstract class BuildAction : IComparable<BuildAction>
{
    /// <summary>
    /// Priority of this build action. Lower values execute earlier.
    /// </summary>
    public int priority = 100;
    public BuildFilter filter = null;

    /// <summary>
    /// This will be exectued once before/after all players are built.
    /// </summary>
    public virtual void Execute()
    {
    }

    /// <summary>
    /// This will be executed before/after each individual player is built.
    /// </summary>
    /// <param name="platform"></param>
    public virtual void Execute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution)
    {
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