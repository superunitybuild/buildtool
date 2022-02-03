using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildTool
{
    public class UnityBuildGUIUtility
    {
        private const string HELP_URL = @"https://github.com/superunitybuild/buildtool/wiki/{0}";

        #region Singleton

        private static UnityBuildGUIUtility _instance = null;

        public static UnityBuildGUIUtility instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnityBuildGUIUtility();
                }

                return _instance;
            }
        }

        #endregion

        private GUIStyle _dropdownHeaderStyle;
        private GUIStyle _dropdownContentStyle;
        private GUIStyle _helpButtonStyle;
        private GUIStyle _midHeaderStyle;
        private GUIStyle _popupStyle;
        private GUIStyle _mainTitleStyle;
        private GUIStyle _dragDropArea;

        private Color32 _defaultBackgroundColor = GUI.backgroundColor;
        private Color32 _mainHeaderColor = new Color32(180, 180, 255, 255);

        private GUIContent helpButtonContent;

        private UnityBuildGUIUtility()
        {
            _dropdownHeaderStyle = new GUIStyle(GUI.skin.button);
            _dropdownHeaderStyle.alignment = TextAnchor.MiddleLeft;
            _dropdownHeaderStyle.fontStyle = FontStyle.Bold;
            _dropdownHeaderStyle.margin = new RectOffset(5, 5, 0, 0);

            _popupStyle = new GUIStyle(EditorStyles.popup);
            _popupStyle.fontSize = 11;
            _popupStyle.alignment = TextAnchor.MiddleLeft;
            _popupStyle.margin = new RectOffset(0, 0, 4, 0);
            _popupStyle.fixedHeight = 18;

            _helpButtonStyle = new GUIStyle(_dropdownHeaderStyle);
            _helpButtonStyle.alignment = TextAnchor.MiddleCenter;
            _helpButtonStyle.fontStyle = FontStyle.Normal;
            _helpButtonStyle.margin = new RectOffset(0, 5, 0, 0);
            _helpButtonStyle.fixedWidth = 30;

            _midHeaderStyle = new GUIStyle(EditorStyles.helpBox);
            _midHeaderStyle.fontStyle = FontStyle.Bold;

            _dropdownContentStyle = new GUIStyle(GUI.skin.textArea);
            _dropdownContentStyle.padding = new RectOffset(5, 5, 5, 5);
            _dropdownContentStyle.margin = new RectOffset(5, 5, 0, 0);

            _mainTitleStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            _mainTitleStyle.fontSize = 18;
            _mainTitleStyle.fontStyle = FontStyle.Bold;
            _mainTitleStyle.alignment = TextAnchor.MiddleLeft;
            _mainTitleStyle.fixedHeight = 36;

            _dragDropArea = new GUIStyle(GUI.skin.box);
            _dragDropArea.stretchWidth = true;
            _dragDropArea.alignment = TextAnchor.MiddleCenter;
            _dragDropArea.normal.textColor = GUI.skin.textField.normal.textColor;

            helpButtonContent = new GUIContent("?", "Help");
        }

        public static void OpenHelp(string anchor = "")
        {
            Application.OpenURL(string.Format(HELP_URL, anchor));
        }

        public static void DropdownHeader(string content, ref bool showDropdown, bool noColor, params GUILayoutOption[] options)
        {
            DropdownHeader(new GUIContent { text = content }, ref showDropdown, noColor, options);
        }

        public static void DropdownHeader(GUIContent content, ref bool showDropdown, bool noColor, params GUILayoutOption[] options)
        {
            if (!noColor)
                GUI.backgroundColor = instance._mainHeaderColor;

            if (EditorGUILayout.DropdownButton(new GUIContent(content), FocusType.Keyboard, dropdownHeaderStyle, options))
            {
                showDropdown = !showDropdown;
            }

            if (!noColor)
                GUI.backgroundColor = instance._defaultBackgroundColor;
        }

        public static void HelpButton(string anchor = "")
        {
            if (GUILayout.Button(_instance.helpButtonContent, helpButtonStyle))
                OpenHelp(anchor);
        }

        public static string ToLabel(string input, int maxLength = 60)
        {
            string output = input;
            string suffix = "...";
            char[] trimChars = new char[] { ' ', ',', '.', '/' };

            Match match = Regex.Match(input, @"(?<primary>.*)\((?<secondary>.*)\)(?<tertiary>.*)");

            if (match.Success)
            {
                string primary = match.Groups["primary"].Value;
                string secondary = match.Groups["secondary"].Value;
                string tertiary = match.Groups["tertiary"].Value;

                if (!string.IsNullOrEmpty(secondary))
                {
                    int truncateLength = primary.Length + tertiary.Length;
                    output = primary + "(" + secondary.Truncate(maxLength - truncateLength - 2, suffix, trimChars) + ")" + tertiary;
                }
                else
                {
                    output = primary.Truncate(maxLength, suffix, trimChars);
                }
            }

            return output;
        }

        public static string ToWords(string input)
        {
            return Regex.Replace(input, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static GUIStyle helpButtonStyle
        {
            get
            {
                return instance._helpButtonStyle;
            }
        }

        public static GUIStyle midHeaderStyle
        {
            get
            {
                return instance._midHeaderStyle;
            }
        }

        public static GUIStyle dropdownHeaderStyle
        {
            get
            {
                return instance._dropdownHeaderStyle;
            }
        }

        public static GUIStyle dropdownContentStyle
        {
            get
            {
                return instance._dropdownContentStyle;
            }
        }

        public static GUIStyle popupStyle
        {
            get
            {
                return instance._popupStyle;
            }
        }

        public static GUIStyle mainTitleStyle
        {
            get
            {
                return instance._mainTitleStyle;
            }
        }

        public static Color defaultBackgroundColor
        {
            get
            {
                return instance._defaultBackgroundColor;
            }
        }

        public static Color mainHeaderColor
        {
            get
            {
                return instance._mainHeaderColor;
            }
        }

        public static GUIStyle dragDropStyle
        {
            get
            {
                return instance._dragDropArea;
            }
        }
    }
}
