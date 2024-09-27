using System;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
    public class BuildAction : ScriptableObject // This really should be an abstract class but needs to be concrete to work with Unity serialization.
    {
        public enum ActionType
        {
            SingleRun,
            PerBuild
        }

        public ActionType actionType = ActionType.PerBuild;
        public string actionName = string.Empty;
        public string note = string.Empty;
        public bool actionEnabled = true;
        [Tooltip("BuildAction should run when 'Configure Editor Environment' button is clicked")] public bool configureEditor = false;
        public BuildFilter filter = new();

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
            BuildTarget target,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
        }

        public static string ResolveExecuteTokens(string prototype)
        {
            DateTime runTime = DateTime.Now;

            prototype = TokensUtility.ResolveBuildTimeTokens(prototype, runTime);
            prototype = TokensUtility.ResolveBuildVersionTokens(prototype);

            return prototype;
        }

        public static string ResolvePerBuildExecuteTokens(string prototype, BuildReleaseType releaseType, BuildPlatform platform, BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, string buildPath)
        {
            prototype = TokensUtility.ResolveBuildOutputTokens(prototype, buildPath);
            prototype = TokensUtility.ResolveBuildConfigurationTokens(prototype, releaseType, platform, target, scriptingBackend, distribution, buildTime);

            return prototype;
        }

        public void Draw(SerializedObject obj)
        {
            DrawProperties(obj);

            Type myType = GetType();
            bool isPreBuildAction = typeof(IPreBuildAction).IsAssignableFrom(myType);
            bool isPostBuildAction = typeof(IPostBuildAction).IsAssignableFrom(myType);
            bool isPreBuildPerPlatformAction = typeof(IPreBuildPerPlatformAction).IsAssignableFrom(myType);
            bool isPostBuildPerPlatformAction = typeof(IPostBuildPerPlatformAction).IsAssignableFrom(myType);
            bool isPreBuildActionCanConfigureEditor = typeof(IPreBuildPerPlatformActionCanConfigureEditor).IsAssignableFrom(myType);
            bool actionTypeSelectable = false;

            if ((isPreBuildAction && isPreBuildPerPlatformAction) || (isPostBuildAction && isPostBuildPerPlatformAction))
                actionTypeSelectable = true;
            else if (isPreBuildAction || isPostBuildAction)
                actionType = ActionType.SingleRun;
            else if (isPreBuildPerPlatformAction || isPostBuildPerPlatformAction)
                actionType = ActionType.PerBuild;

            if (actionTypeSelectable)
                actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type", actionType);

            if (isPreBuildActionCanConfigureEditor)
                _ = EditorGUILayout.PropertyField(obj.FindProperty("configureEditor"));

            _ = EditorGUILayout.PropertyField(obj.FindProperty("note"));

            // Only Per-Build actions can be filtered
            if (actionType == ActionType.PerBuild)
                _ = EditorGUILayout.PropertyField(obj.FindProperty("filter"), GUILayout.Height(0));

            _ = obj.ApplyModifiedProperties();
        }

        public override string ToString()
        {
            string name = actionName;
            name += !string.IsNullOrEmpty(note) ?
                $" ({note})" :
                "";

            return name;
        }

        protected virtual void DrawProperties(SerializedObject obj)
        {
            SerializedProperty prop = obj.GetIterator();
            bool done = false;

            while (!done && prop != null)
            {
                if (prop.name is "actionName" or
                    "actionType" or
                    "note" or
                    "actionEnabled" or
                    "filter" or
                    "configureEditor")
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
                    DrawProperty(prop);
                    done = !prop.NextVisible(true);
                }
            }
        }

        protected virtual void DrawProperty(SerializedProperty prop)
        {
            _ = EditorGUILayout.PropertyField(prop);
        }
    }
}
