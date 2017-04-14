using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildAction : ScriptableObject // This really should be an abstract class but needs to be concrete to work with Unity serialization.
{
    public enum ActionType
    {
        SingleRun,
        PerPlatform
    }

    public ActionType actionType = ActionType.PerPlatform;
    public string actionName = string.Empty;
    public string note = string.Empty;
    public bool actionEnabled = true;
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
    public virtual void PerBuildExecute(
        BuildReleaseType releaseType,
        BuildPlatform platform,
        BuildArchitecture architecture,
        BuildDistribution distribution,
        System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
    }

    public void Draw(SerializedObject obj)
    {
        DrawProperties(obj);

        System.Type myType = this.GetType();
        bool actionTypeSelectable = false;
        if (typeof(IPreBuildAction).IsAssignableFrom(myType) &&
            typeof(IPreBuildPerPlatformAction).IsAssignableFrom(myType))
        {
            actionTypeSelectable = true;
        }
        else if (typeof(IPostBuildAction).IsAssignableFrom(myType) &&
            typeof(IPostBuildPerPlatformAction).IsAssignableFrom(myType))
        {
            actionTypeSelectable = true;
        }
        else if (typeof(IPreBuildAction).IsAssignableFrom(myType) ||
            typeof(IPostBuildAction).IsAssignableFrom(myType))
        {
            actionType = ActionType.SingleRun;
        }
        else if (typeof(IPreBuildPerPlatformAction).IsAssignableFrom(myType) ||
            typeof(IPostBuildPerPlatformAction).IsAssignableFrom(myType))
        {
            actionType = ActionType.PerPlatform;
        }

        if (actionTypeSelectable)
        {
            actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type", actionType);
        }
        EditorGUILayout.PropertyField(obj.FindProperty("note"));
        EditorGUILayout.PropertyField(obj.FindProperty("filter"), GUILayout.Height(0));
        obj.ApplyModifiedProperties();
    }

    protected virtual void DrawProperties(SerializedObject obj)
    {
        SerializedProperty prop = obj.GetIterator();
        bool done = false;

        while (!done && prop != null)
        {
            if (prop.name == "actionName" ||
                prop.name == "actionType" ||
                prop.name == "note" ||
                prop.name == "actionEnabled" ||
                prop.name == "filter")
            {
                // Already drawn these. Go to next, don't enter into object.
                done = !prop.NextVisible(false);
            }
            else if (prop.name == "Base")
            {
                // Valid property we want to enter, but don't want to draw the parent node.
                done = !prop.NextVisible(true);
            }
            else
            {
                EditorGUILayout.PropertyField(prop);
                done = !prop.NextVisible(true);
            }
        }
    }
}

}
