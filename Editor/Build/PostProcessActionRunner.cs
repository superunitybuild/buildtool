using System.Reflection;
using UnityEditor;
using UnityEditor.Build;

namespace SuperSystems.UnityBuild
{
    public class PostProcessActionRunner : IPostprocessBuild
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            BuildNotificationList.instance.AddNotification(new BuildNotification(
                BuildNotification.Category.Notification, "PostProcessBuild", "Running post process build actions",
                true, null));

            BuildAction[] actions = BuildSettings.postProcessBuildActions.buildActions;
            if (actions != null)
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    BuildAction action = actions[i];

                    if (action.actionEnabled)
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Performing Post-Process-Build Action ({0}/{1}).", i + 1, actions.Length),
                            action.actionName,
                            true, null));

                        action.PostProcessExecute(target, path);
                    }
                    else
                    {
                        BuildNotificationList.instance.AddNotification(new BuildNotification(
                            BuildNotification.Category.Notification,
                            string.Format("Skipping Post-Build Action ({0}/{1}).", i + 1, actions.Length),
                            action.actionName,
                            true, null));
                    }
                }
            }
        }
    }
}