
namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildAction // This really should be an abstract class but needs to be concrete to work with Unity serialization.
{
    public string name = string.Empty;
    public string note = string.Empty;
    public BuildFilter filter = new BuildFilter();

    /// <summary>
    /// This will be exectued once before/after all players are built.
    /// </summary>
    public virtual void Execute()
    {
    }

    /// <summary>
    /// This will be executed before/after each individual player is built.
    /// </summary>
    public virtual void Execute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution)
    {
    }
}

}
