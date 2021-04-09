
namespace SuperUnityBuild.BuildTool
{
    [System.Serializable]
    public class BuildNotification
    {
        public enum Category
        {
            Notification,
            Warning,
            Error
        }

        public delegate bool ValidityCheck();

        public Category cat;
        public string title;
        public string details;
        public bool clearable;
        public ValidityCheck valid;

        public BuildNotification(Category cat = Category.Notification, string title = null, string details = null, bool clearable = true, ValidityCheck valid = null)
        {
            this.cat = cat;
            this.title = title;
            this.details = details;
            this.clearable = clearable;
            this.valid = valid;
        }
    }
}