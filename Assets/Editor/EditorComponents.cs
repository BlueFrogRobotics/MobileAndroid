using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

namespace AndroidMobileApp.Editor
{
    public enum FieldSize : int
    {
        MEDIUM,
        LARGE
    }

    public enum FieldType : int
    {
        TEXT,
        PASSWORD
    }

    public enum FooterElementType : int
    {
        TEXT,
        BUTTON,
        VALIDATE_BUTTON,
        CANCEL_BUTTON,
        LINK,
        SUCCESS,
        ERROR
    }

    public class FooterElement
    {
        public FooterElementType Type { get; set; }

        public Action OnClick { get; set; }

        public string Label { get; set; }

        public bool Active { get; set; }

        public FooterElement()
        {
            Active = true;
            Label = string.Empty;
        }
    }

    public static class EditorComponents
    {
        public readonly static Color32 BUDDY_COLOR;
        public readonly static Color32 DARK_COLOR;
        public readonly static Color32 LIGHT_COLOR;
        public readonly static Color32 BLUE_COLOR;
        public readonly static Color32 GREEN_COLOR;
        public readonly static Color32 RED_COLOR;

        public static readonly GUIStyle BODY_STYLE;

        public static readonly GUIStyle BUTTON_STYLE;
        public static readonly GUIStyle BUTTON_VALIDATE_STYLE;
        public static readonly GUIStyle BUTTON_CANCEL_STYLE;
        public static readonly GUIStyle ROW_BUTTON_STYLE;

        public static readonly GUIStyle BLUE_STYLE;
        public static readonly GUIStyle GREEN_STYLE;
        public static readonly GUIStyle RED_STYLE;

        public static readonly GUIStyle TEXT_LABEL_STYLE;
        public static readonly GUIStyle TEXT_STYLE;
        public static readonly GUIStyle LARGE_TEXT_STYLE;
        public static readonly GUIStyle LINK_STYLE;
        public static readonly GUIStyle SUCCESS_STYLE;
        public static readonly GUIStyle ERROR_STYLE;

        public static readonly GUIStyle FIELD_WRAPPER_STYLE;
        public static readonly GUIStyle FIELD_LABEL_STYLE;
        public static readonly GUIStyle FIELD_MEDIUM_STYLE;
        public static readonly GUIStyle FIELD_LARGE_STYLE;

        public static readonly GUIStyle DROPDOWN_FIELD_STYLE;
        public static readonly GUIStyle CHECKBOX_FIELD_STYLE;

        public static readonly GUIStyle LINE_STYLE;

        public static readonly GUIStyle VERTICAL_SCROLLBAR_STYLE;
        public static readonly GUIStyle DEFAULT_VERTICAL_SCROLLBAR_THUMB_STYLE;
        public static readonly GUIStyle DEFAULT_VERTICAL_SCROLLBAR_UP_STYLE;
        public static readonly GUIStyle DEFAULT_VERTICAL_SCROLLBAR_DOWN_STYLE;
        public static readonly GUIStyle VERTICAL_SCROLLBAR_THUMB_STYLE;
        public static readonly GUIStyle VERTICAL_SCROLLBAR_UP_STYLE;
        public static readonly GUIStyle VERTICAL_SCROLLBAR_DOWN_STYLE;

        static EditorComponents()
        {
            BUDDY_COLOR = new Color32(0, 212, 209, 255);
            DARK_COLOR = new Color32(51, 51, 51, 255);
            LIGHT_COLOR = new Color32(255, 255, 255, 255);
            BLUE_COLOR = new Color32(0, 120, 200, 255);
            GREEN_COLOR = new Color32(255, 0, 0, 125);
            RED_COLOR = new Color32(255, 0, 0, 125);

            BLUE_STYLE = new GUIStyle(GUI.skin.label);
            BLUE_STYLE.normal.textColor = BLUE_COLOR;

            GREEN_STYLE = new GUIStyle(GUI.skin.label);
            GREEN_STYLE.normal.textColor = GREEN_COLOR;

            RED_STYLE = new GUIStyle(GUI.skin.label);
            RED_STYLE.normal.textColor = RED_COLOR;

            SUCCESS_STYLE = new GUIStyle(GUI.skin.label);
            SUCCESS_STYLE.normal.textColor = GREEN_COLOR;
            SUCCESS_STYLE.margin = new RectOffset(0, 0, 5, 5);

            ERROR_STYLE = new GUIStyle(GUI.skin.label);
            ERROR_STYLE.normal.textColor = RED_COLOR;
            ERROR_STYLE.margin = new RectOffset(0, 0, 5, 5);

            LINK_STYLE = new GUIStyle(GUI.skin.label);
            LINK_STYLE.normal.textColor = BUDDY_COLOR;
            LINK_STYLE.padding = new RectOffset(0, 0, 0, 0);

            TEXT_LABEL_STYLE = new GUIStyle(GUI.skin.label);
            TEXT_LABEL_STYLE.normal.textColor = Color.white;
            TEXT_LABEL_STYLE.wordWrap = true;
            TEXT_LABEL_STYLE.padding = new RectOffset(0, 0, 0, 0);
            TEXT_LABEL_STYLE.margin = new RectOffset(0, 0, 2, 2);
            TEXT_LABEL_STYLE.fontStyle = FontStyle.Bold;

            TEXT_STYLE = new GUIStyle(GUI.skin.label);
            TEXT_STYLE.normal.textColor = Color.white;
            TEXT_STYLE.wordWrap = true;
            TEXT_STYLE.padding = new RectOffset(0, 0, 0, 0);
            TEXT_STYLE.margin = new RectOffset(0, 0, 2, 2);

            LARGE_TEXT_STYLE = new GUIStyle(TEXT_STYLE);
            LARGE_TEXT_STYLE.fontSize = 15;

            BODY_STYLE = new GUIStyle();
            BODY_STYLE.padding = new RectOffset(20, 20, 20, 20);

            Texture2D lButtonTexture = EditorUtils.LoadImage("Editor/Sprites/square_grey.png");
            BUTTON_STYLE = new GUIStyle(GUI.skin.button);
            BUTTON_STYLE.fixedHeight = 25;
            BUTTON_STYLE.margin = new RectOffset(4, 4, -3, 0);
            BUTTON_STYLE.normal.textColor = LIGHT_COLOR;
            BUTTON_STYLE.active.textColor = LIGHT_COLOR;
            BUTTON_STYLE.normal.background = lButtonTexture;
            BUTTON_STYLE.active.background = lButtonTexture;

            Texture2D lValidateButtonTexture = EditorUtils.LoadImage("Editor/Sprites/square_blue.png");
            BUTTON_VALIDATE_STYLE = new GUIStyle(BUTTON_STYLE);
            BUTTON_VALIDATE_STYLE.normal.textColor = DARK_COLOR;
            BUTTON_VALIDATE_STYLE.active.textColor = DARK_COLOR;
            BUTTON_VALIDATE_STYLE.normal.background = lValidateButtonTexture;
            BUTTON_VALIDATE_STYLE.active.background = lValidateButtonTexture;

            Texture2D lCancelButtonTexture = EditorUtils.LoadImage("Editor/Sprites/square_black.png");
            BUTTON_CANCEL_STYLE = new GUIStyle(BUTTON_STYLE);
            BUTTON_CANCEL_STYLE.normal.background = lCancelButtonTexture;
            BUTTON_CANCEL_STYLE.active.background = lCancelButtonTexture;

            ROW_BUTTON_STYLE = new GUIStyle(GUI.skin.button);
            ROW_BUTTON_STYLE.padding = new RectOffset(14, 14, 14, 14);
            ROW_BUTTON_STYLE.alignment = TextAnchor.MiddleLeft;
            ROW_BUTTON_STYLE.fontSize = 14;
            ROW_BUTTON_STYLE.normal.textColor = LIGHT_COLOR;
            ROW_BUTTON_STYLE.active.textColor = LIGHT_COLOR;
            ROW_BUTTON_STYLE.richText = true;

            FIELD_WRAPPER_STYLE = new GUIStyle();
            FIELD_WRAPPER_STYLE.margin = new RectOffset(0, 0, 10, 0);

            FIELD_LABEL_STYLE = new GUIStyle(GUI.skin.label);
            FIELD_LABEL_STYLE.normal.textColor = LIGHT_COLOR;
            FIELD_LABEL_STYLE.active.textColor = LIGHT_COLOR;
            FIELD_LABEL_STYLE.fontStyle = FontStyle.Bold;
            FIELD_LABEL_STYLE.margin = new RectOffset(0, 0, 2, 2);

            Texture2D lFieldBackground = EditorUtils.MakeBackground(1, 1, new Color32(255, 255, 255, 255));
            GUIStyle lFieldStyle = new GUIStyle(GUI.skin.textField);
            lFieldStyle.wordWrap = false;
            lFieldStyle.normal.background = lFieldBackground;
            lFieldStyle.active.background = lFieldBackground;
            lFieldStyle.focused.background = lFieldBackground;
            lFieldStyle.active.textColor = DARK_COLOR;
            lFieldStyle.normal.textColor = DARK_COLOR;
            lFieldStyle.focused.textColor = DARK_COLOR;
            lFieldStyle.margin = new RectOffset(0, 0, 0, 2);

            FIELD_LARGE_STYLE = new GUIStyle(lFieldStyle);
            //FIELD_LARGE_STYLE.font = (Font)Resources.Load("Fonts/os_hangeul");
            FIELD_LARGE_STYLE.fontSize = 15;
            FIELD_LARGE_STYLE.padding = new RectOffset(5, 5, 8, 8);

            FIELD_MEDIUM_STYLE = new GUIStyle(lFieldStyle);
            //FIELD_MEDIUM_STYLE.font = (Font)Resources.Load("Fonts/os_hangeul");
            FIELD_MEDIUM_STYLE.fontSize = 11;
            FIELD_MEDIUM_STYLE.padding = new RectOffset(5, 5, 3, 3);

            DROPDOWN_FIELD_STYLE = new GUIStyle(FIELD_MEDIUM_STYLE);
            DROPDOWN_FIELD_STYLE.fixedHeight = 19;

            Texture2D lActiveCheckboxTexture = EditorUtils.LoadImage("Editor/Sprites/checkbox_active.png");
            Texture2D lInactiveCheckboxTexture = EditorUtils.LoadImage("Editor/Sprites/checkbox_inactive.png");
            CHECKBOX_FIELD_STYLE = new GUIStyle(GUI.skin.toggle);
            CHECKBOX_FIELD_STYLE.margin = new RectOffset(0, 0, 2, 2);
            CHECKBOX_FIELD_STYLE.normal.textColor = LIGHT_COLOR;
            CHECKBOX_FIELD_STYLE.active.textColor = LIGHT_COLOR;
            CHECKBOX_FIELD_STYLE.focused.textColor = LIGHT_COLOR;

            LINE_STYLE = new GUIStyle();
            LINE_STYLE.fixedHeight = 1;
            LINE_STYLE.normal.background = EditorUtils.MakeBackground(1, 1, new Color32(255, 255, 255, 50));

            DEFAULT_VERTICAL_SCROLLBAR_THUMB_STYLE = new GUIStyle(GUI.skin.verticalScrollbarThumb);
            DEFAULT_VERTICAL_SCROLLBAR_UP_STYLE = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
            DEFAULT_VERTICAL_SCROLLBAR_DOWN_STYLE = new GUIStyle(GUI.skin.verticalScrollbarDownButton);

            VERTICAL_SCROLLBAR_STYLE = new GUIStyle(GUI.skin.verticalScrollbar);
            VERTICAL_SCROLLBAR_STYLE.fixedWidth = 25;
            VERTICAL_SCROLLBAR_STYLE.normal.background =
                EditorUtils.LoadImage("Editor/Sprites/vertical_scrollbar_grey.png");

            VERTICAL_SCROLLBAR_UP_STYLE = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
            VERTICAL_SCROLLBAR_UP_STYLE.normal.background = null;
            VERTICAL_SCROLLBAR_UP_STYLE.hover.background = null;
            VERTICAL_SCROLLBAR_UP_STYLE.active.background = null;

            VERTICAL_SCROLLBAR_DOWN_STYLE = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
            VERTICAL_SCROLLBAR_DOWN_STYLE.normal.background = null;
            VERTICAL_SCROLLBAR_DOWN_STYLE.hover.background = null;
            VERTICAL_SCROLLBAR_DOWN_STYLE.active.background = null;

            VERTICAL_SCROLLBAR_THUMB_STYLE = new GUIStyle(GUI.skin.verticalScrollbarThumb);
            VERTICAL_SCROLLBAR_THUMB_STYLE.fixedWidth = 25;
            VERTICAL_SCROLLBAR_THUMB_STYLE.normal.background =
                EditorUtils.LoadImage("Editor/Sprites/vertical_scrollbar_blue.png");
        }

        public static void RenderText(string iText, params GUILayoutOption[] iOptions)
        {
            RenderText(iText, TEXT_STYLE, iOptions);
        }

        public static void RenderText(string iText, GUIStyle iStyle, params GUILayoutOption[] iOptions)
        {
            GUILayout.Label(iText, iStyle, iOptions);
        }

        public static void RenderTextLabel(string iLabel, string iValue, int iLabelSize, int iValueSize)
        {
            GUILayout.BeginHorizontal();
            RenderText(iLabel, GUILayout.Width(iLabelSize));
            RenderText(iValue, GUILayout.Width(iValueSize));
            GUILayout.EndHorizontal();
        }

        public static void RenderCenterText(string iText, params GUILayoutOption[] iOptions)
        {
            RenderCenterText(iText, TEXT_STYLE, iOptions);
        }

        public static void RenderCenterText(string iText, GUIStyle iStyle, params GUILayoutOption[] iOptions)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(iText, iStyle, iOptions);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void RenderLink(string iText, string iLink)
        {
            RenderLink(iText, () => { Application.OpenURL(iLink); });
        }

        public static void RenderLink(string iText, Action iOnClick)
        {
            if (GUILayout.Button(iText, LINK_STYLE))
                iOnClick();

            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
        }

        public static void RenderTitle(string iTitle, float iWidth)
        {
            RenderText(iTitle.ToUpper(), LARGE_TEXT_STYLE, GUILayout.ExpandWidth(false));
            Rect lTitleRect = GUILayoutUtility.GetLastRect();
            Rect lLineRect = new Rect(lTitleRect.x + lTitleRect.width + 10, lTitleRect.y + lTitleRect.height / 2, iWidth - lTitleRect.width, 1);
            EditorGUI.DrawRect(lLineRect, new Color32(255, 255, 255, 50));
            GUILayout.Space(15);
        }

        public static void RenderCenterImage(Texture2D iImage, GUIStyle iStyle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(iImage, iStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static string RenderTextField(string iValue, Action iOnChange = null, int iWidth = 200)
        {
            return RenderNormalField(null, iValue, iOnChange, FieldType.TEXT, FieldSize.MEDIUM, iWidth);
        }

        public static string RenderTextField(string iLabel, string iValue, Action iOnChange = null, int iWidth = 200)
        {
            return RenderNormalField(iLabel, iValue, iOnChange, FieldType.TEXT, FieldSize.MEDIUM, iWidth);
        }

        public static string RenderPasswordField(string iValue, Action iOnChange = null, int iWidth = 200)
        {
            return RenderNormalField(null, iValue, iOnChange, FieldType.PASSWORD, FieldSize.MEDIUM, iWidth);
        }

        public static string RenderPasswordField(string iLabel, string iValue, Action iOnChange = null, int iWidth = 200)
        {
            return RenderNormalField(iLabel, iValue, iOnChange, FieldType.PASSWORD, FieldSize.MEDIUM, iWidth);
        }

        public static string RenderNormalField(string iLabel, string iValue,
            Action iOnChange, FieldType iType, FieldSize iSize, int iWidth = 200)
        {
            FIELD_WRAPPER_STYLE.margin = string.IsNullOrEmpty(iLabel)
                ? new RectOffset(0, 0, 0, 0) : new RectOffset(0, 0, 10, 0);

            GUILayout.BeginVertical(FIELD_WRAPPER_STYLE, GUILayout.Width(iWidth));

            if (!string.IsNullOrEmpty(iLabel))
                GUILayout.Label(iLabel, FIELD_LABEL_STYLE);

            if (iOnChange != null)
                HandleKeyEvents(iOnChange);

            GUI.skin.settings.cursorColor = DARK_COLOR;

            GUIContent lFieldContent = new GUIContent();
            iValue = iValue == null ? "" : iValue;
            lFieldContent.text = iType == FieldType.PASSWORD ? string.Empty.PadRight(iValue.Length, '*') : iValue;

            Rect lFieldRect = GUILayoutUtility.GetRect(lFieldContent,
                iSize == FieldSize.LARGE ? FIELD_LARGE_STYLE : FIELD_MEDIUM_STYLE, GUILayout.MaxWidth(iWidth));

            string oResult;
            if (iType == FieldType.PASSWORD)
                oResult = EditorGUI.PasswordField(lFieldRect, iValue,
                    iSize == FieldSize.LARGE ? FIELD_LARGE_STYLE : FIELD_MEDIUM_STYLE);
            else
                oResult = EditorGUI.TextField(lFieldRect, iValue,
                    iSize == FieldSize.LARGE ? FIELD_LARGE_STYLE : FIELD_MEDIUM_STYLE);

            GUILayout.EndVertical();

            return oResult;
        }

        public static Enum RenderEnumDropdown(Enum iSelected, Action iOnChange = null, int iWidth = 200)
        {
            return RenderEnumDropdown(null, iSelected, iOnChange, iWidth);
        }

        public static Enum RenderEnumDropdown(string iLabel, Enum iSelected, Action iOnChange = null, int iWidth = 200)
        {
            FIELD_WRAPPER_STYLE.margin = string.IsNullOrEmpty(iLabel)
                ? new RectOffset(0, 0, 0, 0) : new RectOffset(0, 0, 10, 0);

            GUILayout.BeginVertical(FIELD_WRAPPER_STYLE, GUILayout.Width(iWidth));

            if (!string.IsNullOrEmpty(iLabel))
                GUILayout.Label(iLabel, FIELD_LABEL_STYLE);

            Enum oSelected = EditorGUILayout.EnumPopup(iSelected, DROPDOWN_FIELD_STYLE, GUILayout.MaxWidth(iWidth));

            if (iOnChange != null && oSelected.CompareTo(iSelected) != 0)
                iOnChange();

            GUILayout.EndVertical();

            return oSelected;
        }

        public static int RenderDropdown(int iSelected, string[] iOptions, Action iOnChange = null, int iWidth = 200)
        {
            return RenderDropdown(null, iSelected, iOptions, iOnChange, iWidth);
        }

        public static int RenderDropdown(string iLabel, int iSelected, string[] iOptions,
            Action iOnChange = null, int iWidth = 200)
        {
            FIELD_WRAPPER_STYLE.margin = string.IsNullOrEmpty(iLabel)
                ? new RectOffset(0, 0, 0, 0) : new RectOffset(0, 0, 10, 0);

            GUILayout.BeginVertical(FIELD_WRAPPER_STYLE, GUILayout.Width(iWidth));

            if (!string.IsNullOrEmpty(iLabel))
                GUILayout.Label(iLabel, FIELD_LABEL_STYLE);

            int oSelected = EditorGUILayout.Popup(iSelected, iOptions, DROPDOWN_FIELD_STYLE, GUILayout.MaxWidth(iWidth));

            if (iOnChange != null && oSelected != iSelected)
                iOnChange();

            GUILayout.EndVertical();

            return oSelected;
        }

        public static int RenderIntDropdown(int iSelected, string[] iLabels, int[] iOptions,
            Action iOnChange = null, int iWidth = 200)
        {
            return RenderIntDropdown(null, iSelected, iLabels, iOptions, iOnChange, iWidth);
        }

        public static int RenderIntDropdown(string iLabel, int iSelected, string[] iLabels, int[] iOptions,
            Action iOnChange = null, int iWidth = 200)
        {
            FIELD_WRAPPER_STYLE.margin = string.IsNullOrEmpty(iLabel)
                ? new RectOffset(0, 0, 0, 0) : new RectOffset(0, 0, 10, 0);

            GUILayout.BeginVertical(FIELD_WRAPPER_STYLE, GUILayout.Width(iWidth));

            if (!string.IsNullOrEmpty(iLabel))
                GUILayout.Label(iLabel, FIELD_LABEL_STYLE);

            int oSelected = EditorGUILayout.IntPopup(iSelected, iLabels, iOptions,
                DROPDOWN_FIELD_STYLE, GUILayout.MaxWidth(iWidth));

            if (iOnChange != null && oSelected != iSelected)
                iOnChange();

            GUILayout.EndVertical();

            return oSelected;
        }

        public static bool RenderCheckboxField(string iLabel, bool iVal, int iLabelSize = 200, bool iLeftLabel = false)
        {
            if (iLeftLabel) {
                GUILayout.BeginHorizontal();
                RenderText(iLabel, GUILayout.Width(iLabelSize));
            }

            bool oChecked = GUILayout.Toggle(iVal, iLeftLabel ? "" : iLabel,
                CHECKBOX_FIELD_STYLE, GUILayout.Width(iLeftLabel ? 10 : iLabelSize));

            if (iLeftLabel)
                GUILayout.EndHorizontal();

            return oChecked;
        }

        public static void RenderButton(string iLabel, Action iOnClick, int? iWidth = null, bool iUpperCase = true, bool iEnable = true)
        {
            RenderButton(iLabel, iOnClick, BUTTON_STYLE, iWidth, iUpperCase, iEnable);
        }

        public static void RenderButton(string iLabel, Action iOnClick, GUIStyle iStyle, int? iWidth = null, bool iUpperCase = true, bool iEnable = true)
        {
            List<GUILayoutOption> lLayoutOptions = new List<GUILayoutOption>();

            if (iWidth.HasValue)
                lLayoutOptions.Add(GUILayout.Width(iWidth.Value));

            if (!iEnable)
                GUI.enabled = false;

            if (GUILayout.Button(iUpperCase ? iLabel.ToUpper() : iLabel, iStyle, lLayoutOptions.ToArray()))
                iOnClick();

            GUI.enabled = true;
        }

        public static Vector2 BeginScrollView(Vector2 iScrollPosition)
        {
            // Change the default unity skin for scrollbar elements
            GUI.skin.verticalScrollbarThumb = VERTICAL_SCROLLBAR_THUMB_STYLE;
            GUI.skin.verticalScrollbarDownButton = VERTICAL_SCROLLBAR_DOWN_STYLE;
            GUI.skin.verticalScrollbarUpButton = VERTICAL_SCROLLBAR_UP_STYLE;

            Vector2 oScrollPosition = GUILayout.BeginScrollView(iScrollPosition, GUI.skin.horizontalScrollbar,
                VERTICAL_SCROLLBAR_STYLE);

            // Reset default unity skin for scrollbar elements
            GUI.skin.verticalScrollbarThumb = DEFAULT_VERTICAL_SCROLLBAR_THUMB_STYLE;
            GUI.skin.verticalScrollbarDownButton = DEFAULT_VERTICAL_SCROLLBAR_DOWN_STYLE;
            GUI.skin.verticalScrollbarUpButton = DEFAULT_VERTICAL_SCROLLBAR_UP_STYLE;

            return oScrollPosition;
        }

        public static void EndScrollView()
        {
            GUILayout.EndScrollView();
        }

        private static void HandleKeyEvents(Action iOnChange)
        {
            UnityEngine.Event lCurrent = UnityEngine.Event.current;

            if (lCurrent.type == EventType.KeyDown)
                iOnChange();
        }
    }
}