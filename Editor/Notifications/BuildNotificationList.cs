using System;
using System.Collections.Generic;

namespace SuperUnityBuild.BuildTool
{
    [Serializable]
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
        }

        public void AddNotification(BuildNotification notification)
        {
            BuildNotification entry = null;
            switch (notification.cat)
            {
                case BuildNotification.Category.Error:
                    entry = FindDuplicate(notification, errors);
                    if (entry == null)
                        errors.Add(notification);
                    else if (entry.valid == null && notification.valid != null)
                        entry.valid = notification.valid;
                    break;

                case BuildNotification.Category.Warning:
                    entry = FindDuplicate(notification, warnings);
                    if (entry == null)
                        warnings.Add(notification);
                    else if (entry.valid == null && notification.valid != null)
                        entry.valid = notification.valid;
                    break;

                case BuildNotification.Category.Notification:
                    entry = FindDuplicate(notification, notifications);
                    if (entry == null)
                        notifications.Add(notification);
                    else if (entry.valid == null && notification.valid != null)
                        entry.valid = notification.valid;
                    break;
            }
        }

        public void RefreshAll()
        {
            RefreshList(notifications);
            RefreshList(warnings);
            RefreshList(errors);
        }

        public void RefreshErrors()
        {
            RefreshList(errors);
        }

        public void RefreshWarnings()
        {
            RefreshList(warnings);
        }

        public void RefreshNotifications()
        {
            RefreshList(notifications);
        }

        public void Remove(BuildNotification notification)
        {
            BuildNotification entry = null;
            switch (notification.cat)
            {
                case BuildNotification.Category.Error:
                    entry = FindDuplicate(notification, errors);
                    if (entry != null)
                        errors.Remove(entry);
                    break;

                case BuildNotification.Category.Warning:
                    entry = FindDuplicate(notification, warnings);
                    if (entry != null)
                        warnings.Remove(entry);
                    break;

                case BuildNotification.Category.Notification:
                    entry = FindDuplicate(notification, notifications);
                    if (entry != null)
                        notifications.Remove(entry);
                    break;
            }
        }

        public void InitializeErrors()
        {
            AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "No ReleaseType Found",
                "At least one ReleaseType is required to perform a build.",
                false,
                () => BuildSettings.releaseTypeList.releaseTypes.Length == 0));

            AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "No Build Platform Found",
                "At least one Build Platform with one enabled Architecture is required to perform a build.",
                false,
                () =>
                {
                    bool validError = true;

                    int platformCount = BuildSettings.platformList.platforms.Count;
                    if (platformCount > 0)
                    {
                        for (int i = 0; i < platformCount; i++)
                        {
                            BuildPlatform platform = BuildSettings.platformList.platforms[i];
                            if (platform.enabled && platform.atLeastOneArch && platform.atLeastOneBackend)
                            {
                                validError = false;
                                break;
                            }
                        }
                    }

                    return validError;
                }));

            AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "Invalid ReleaseType Name",
                $"One or more ReleaseType names is invalid. They may not be empty or '{BuildConstantsGenerator.NONE}'.",
                false,
                () =>
                {
                    bool validError = false;

                    int count = BuildSettings.releaseTypeList.releaseTypes.Length;
                    for (int i = 0; i < count; i++)
                    {
                        string typeName = BuildSettings.releaseTypeList.releaseTypes[i].typeName.Trim();
                        if (string.IsNullOrEmpty(typeName) ||
                            typeName.Equals(BuildConstantsGenerator.NONE, StringComparison.OrdinalIgnoreCase))
                        {
                            validError = true;
                            break;
                        }
                    }

                    return validError;
                }));

            AddNotification(new BuildNotification(
                BuildNotification.Category.Error,
                "Invalid Distribution Name",
                $"One or more Distribution names is invalid. They may not be empty or '{BuildConstantsGenerator.NONE}'.",
                false,
                () =>
                {
                    bool validError = false;

                    int platformCount = BuildSettings.platformList.platforms.Count;
                    for (int i = 0; i < platformCount; i++)
                    {
                        BuildPlatform platform = BuildSettings.platformList.platforms[i];
                        int distroCount = platform.distributionList.distributions.Length;
                        for (int j = 0; j < distroCount; j++)
                        {
                            string distributionName = platform.distributionList.distributions[j].distributionName.Trim();
                            if (string.IsNullOrEmpty(distributionName) ||
                                distributionName.Equals(BuildConstantsGenerator.NONE, StringComparison.OrdinalIgnoreCase))
                            {
                                validError = true;
                                break;
                            }
                        }
                    }

                    return validError;
                }));
        }

        private BuildNotification FindDuplicate(BuildNotification notification, List<BuildNotification> list)
        {
            BuildNotification duplicate = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].title.Equals(notification.title) && list[i].details.Equals(notification.details))
                {
                    duplicate = list[i];
                    break;
                }
            }

            return duplicate;
        }

        private void RefreshList(List<BuildNotification> buildNotifications)
        {
            for (int i = 0; i < buildNotifications.Count; i++)
            {
                BuildNotification note = buildNotifications[i];
                if (note.clearable)
                {
                    buildNotifications.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}
