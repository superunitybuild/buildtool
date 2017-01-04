using System.Collections.Generic;

namespace SuperSystems.UnityBuild
{

[System.Serializable]
public class BuildNotificationList
{
    #region Singleton

    private static BuildNotificationList _instance;

    public static BuildNotificationList instance
    {
        get
        {
            if (_instance == null)
                _instance = new BuildNotificationList();

            return _instance;
        }
    }

    #endregion

    public List<BuildNotification> notifications = new List<BuildNotification>();
    public List<BuildNotification> warnings = new List<BuildNotification>();
    public List<BuildNotification> errors = new List<BuildNotification>();

    public BuildNotificationList()
    {
        InitializeErrors();
    }

    public void AddNotification(BuildNotification notification)
    {
        switch (notification.cat)
        {
            case BuildNotification.Category.Error:
                if (!CheckForDuplicate(notification, errors))
                    errors.Add(notification);
                break;
            case BuildNotification.Category.Warning:
                if (!CheckForDuplicate(notification, warnings))
                    warnings.Add(notification);
                break;
            case BuildNotification.Category.Notification:
                if (!CheckForDuplicate(notification, notifications))
                    notifications.Add(notification);
                break;
        }
    }

    public void Refresh()
    {
        RefreshList(notifications);
        RefreshList(warnings);
        RefreshList(errors);
    }

    private bool CheckForDuplicate(BuildNotification notification, List<BuildNotification> list)
    {
        bool duplicate = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].title.Equals(notification.title) && list[i].details.Equals(notification.details))
            {
                duplicate = true;
                break;
            }
        }

        return duplicate;
    }

    private void RefreshList(List<BuildNotification> buildNotifications)
    {
        for (int i = 0; i < buildNotifications.Count; i++)
        {
            if (buildNotifications[i].valid != null && !buildNotifications[i].valid())
            {
                buildNotifications.RemoveAt(i);
                --i;
            }
        }
    }

    public void InitializeErrors()
    {
        AddNotification(new BuildNotification(
            BuildNotification.Category.Error,
            "No ReleaseType Found",
            "At least one ReleaseType is required to perform a build.",
            () => BuildSettings.releaseTypeList.releaseTypes.Length == 0));

        AddNotification(new BuildNotification(
            BuildNotification.Category.Error,
            "No Build Platform Found",
            "At least one Build Platform with one enabled Architecture is required to perform a build.",
            () => {
                bool validError = true;

                int platformCount = BuildSettings.platformList.platforms.Length;
                if (platformCount > 0)
                {
                    for (int i = 0; i < platformCount; i++)
                    {
                        BuildPlatform platform = BuildSettings.platformList.platforms[i];
                        if (platform.enabled && platform.atLeastOneArch)
                        {
                            validError = false;
                            break;
                        }
                    }
                }

                return validError;
            }));
    }
}

}