using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace SuperUnityBuild.BuildTool
{
    public static class ExtensionMethods
    {
        public static string SanitizeCodeString(this string str)
        {
            str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);

            if (char.IsDigit(str[0]))
                str = "_" + str;

            return str;
        }

        public static string SanitizeDefine(this string input)
        {
            return input.ToUpperInvariant().Replace(" ", "").SanitizeCodeString();
        }

        public static string SanitizeFolderName(this string folderName)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"[{0}]", invalidChars);

            return Regex.Replace(folderName, invalidRegStr, "");
        }

        public static string SanitizeFileName(this string fileName)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(fileName, invalidRegStr, "_");
        }

        public static string Truncate(this string value, int maxLength, string suffix = "", char[] trimChars = null)
        {
            trimChars = trimChars ?? new char[] { ' ' };

            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;
            else if (maxLength <= 0)
                return suffix;
            else
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

        // Provided by Jon Skeet in https://stackoverflow.com/questions/969091/c-skiplast-implementation
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            T previous = default(T);
            bool first = true;

            foreach (T element in source)
            {
                if (!first)
                    yield return previous;

                previous = element;
                first = false;
            }
        }
    }
}
