using UnityEngine;
using UnityEditor;

namespace UnityBuild
{

public abstract class BuildSettings
{
    public abstract string binName { get; }
    public abstract string binPath { get; }
    public abstract string[] scenesInBuild { get; }
    public abstract string[] copyToBuild { get; }

    public virtual void PreBuild()
    {
    }

    public virtual void PostBuild()
    {
    }
}

}