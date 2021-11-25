
namespace SuperUnityBuild.BuildTool
{
    public static class ExtensionMethods
    {
        public static string Truncate(this string value, int maxLength, string suffix = "", char[] trimChars = null)
        {
            trimChars = trimChars ?? new char[] { ' ' };

            return (string.IsNullOrEmpty(value) || value.Length <= maxLength) ?
                value :
                value.Substring(0, maxLength).Trim(trimChars) + suffix;
        }
    }
}
