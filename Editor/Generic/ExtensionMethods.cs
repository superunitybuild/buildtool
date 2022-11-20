using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    public static class ExtensionMethods
    {
        public static string Truncate(this string value, int maxLength, string suffix = "", char[] trimChars = null)
        {
            trimChars = trimChars ?? new char[] { ' ' };

            if(string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }
            if(maxLength <= 0)
            {
                return suffix;
            }

            return value.Substring(0, maxLength).Trim(trimChars) + suffix;
        }

        public static void SafeDeleteArrayElementAtIndex(this SerializedProperty value, int i)
        {
            if (!value.isArray || i < 0 || i >= value.arraySize)
                return;

            int oldLength = value.arraySize;

            value.DeleteArrayElementAtIndex(i);

            // Unity 2021.2+ changes the behaviour of DeleteArrayElementAtIndex to no longer just null the element reference
            // and instead deletes the element immediately.
            // Older Unity versions require you to call this method *twice* to actually delete an element.
            // See <https://forum.unity.com/threads/array-element-deletion-change.1060004/>

            if (value.arraySize == oldLength)
                value.DeleteArrayElementAtIndex(i);
        }
    }
}
