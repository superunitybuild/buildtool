
namespace SuperUnityBuild.BuildTool
{
    public static class ExtensionMethods
    {
        public static string Truncate(this string value, int maxLength, string truncateSuffix = "")
        {
            return (string.IsNullOrEmpty(value) || value.Length <= maxLength) ?
                value :
                value.Substring(0, maxLength) + truncateSuffix;
        }
    }
}
