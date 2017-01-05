using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public class UnityBuildGUIUtility
{
    private const string HELP_URL = @"https://github.com/Chaser324/unity-build/wiki/{0}";

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

        _dropdownContentStyle = new GUIStyle(GUI.skin.textField);
        _dropdownContentStyle.padding = new RectOffset(5, 5, 5, 5);
        _dropdownContentStyle.margin = new RectOffset(5, 5, 0, 0);

        helpButtonContent = new GUIContent("?", "Help");
    }

    public static void OpenHelp(string anchor = "")
    {
        Application.OpenURL(string.Format(HELP_URL, anchor));
    }

    public static void DropdownHeader(string content, ref bool showDropdown, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, UnityBuildGUIUtility.dropdownHeaderStyle, options))
        {
            showDropdown = !showDropdown;
            GUIUtility.keyboardControl = 0;
        }
    }

    public static void HelpButton(string anchor = "")
    {
        if (GUILayout.Button(_instance.helpButtonContent, UnityBuildGUIUtility.helpButtonStyle))
            OpenHelp(anchor);
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
}

}