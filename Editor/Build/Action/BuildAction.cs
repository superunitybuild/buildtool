using UnityEngine;
using UnityEditor;

namespace UnityBuild
{

public abstract class BuildAction
{
    public virtual void Execute()
    {
    }

    public virtual void Execute(BuildPlatform platform)
    {
    }
}

}