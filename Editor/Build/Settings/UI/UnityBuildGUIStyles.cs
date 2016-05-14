using UnityEngine;
using UnityEditor;

public class UnityBuildGUIStyles
{
    #region Singleton

    private static UnityBuildGUIStyles instance = null;

    public static UnityBuildGUIStyles Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UnityBuildGUIStyles();
            }

            return instance;
        }
    }

    #endregion

    private GUIStyle _dropdownHeaderStyle;
    private GUIStyle _dropdownHeaderCollapsedStyle;
    private GUIStyle _dropdownContentStyle;
    private GUIStyle _popupStyle;

    private UnityBuildGUIStyles()
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

        _dropdownHeaderCollapsedStyle = new GUIStyle(_dropdownHeaderStyle);

        _dropdownContentStyle = new GUIStyle(GUI.skin.textField);
        _dropdownContentStyle.padding = new RectOffset(5, 5, 5, 5);
        _dropdownContentStyle.margin = new RectOffset(5, 5, 0, 0);
    }

    public static void DropdownHeader(string content, ref bool showDropdown)
    {
        if (GUILayout.Button(content, UnityBuildGUIStyles.dropdownHeaderStyle))
        {
            showDropdown = !showDropdown;
        }
    }

    public static GUIStyle dropdownHeaderStyle
    {
        get
        {
            return Instance._dropdownHeaderStyle;
        }
    }

    public static GUIStyle dropdownContentStyle
    {
        get
        {
            return Instance._dropdownContentStyle;
        }
    }

    public static GUIStyle popupStyle
    {
        get
        {
            return Instance._popupStyle;
        }
    }
}