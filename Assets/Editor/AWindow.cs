using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

namespace AndroidMobileApp.Editor
{
    public abstract class AWindow : EditorWindow
    {
        private Rect mFooterPosition = new Rect();
        private Rect mLeftElementPosition = new Rect();
        private Rect mRightElementPosition = new Rect();

        protected abstract void RenderImpl();

        void OnGUI()
        {
            // Draw the window background
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), EditorComponents.DARK_COLOR);

            GUILayout.BeginVertical(EditorComponents.BODY_STYLE);
            // Refresh footer elements base on window position and dimension
            InitPosition(position);
            RenderImpl();
            GUILayout.EndVertical();
        }

        private void InitPosition(Rect iPosition)
        {
            mFooterPosition.height = 25;
            mFooterPosition.y = position.height - (20 + 20 + 25);

            mLeftElementPosition.width = 90;
            mLeftElementPosition.height = 25;
            mLeftElementPosition.y = position.height - (25 + 20);

            mRightElementPosition.width = 90;
            mRightElementPosition.height = 29;
            mRightElementPosition.y = position.height - (25 + 20);
        }

        public void RenderFooter(params FooterElement[] iElements)
        {
            RenderFooter(0, position.width, iElements);
        }

        public void RenderFooter(float iStart, float iEnd, params FooterElement[] iElements)
        {
            GUILayout.Space(60);

            Rect lFooterPosition = new Rect();
            lFooterPosition.x = iStart;
            lFooterPosition.y = position.height - (20 + 25 + 20);
            lFooterPosition.height = 20 + 25 + 20;
            lFooterPosition.width = iEnd - iStart;

            float lHalfSize = lFooterPosition.width / 2;

            // Draw thin line to separate footer
            GUI.Label(lFooterPosition, "", EditorComponents.LINE_STYLE);

            // Create two fixed area in which elements will apear
            GUILayout.BeginArea(new Rect(lFooterPosition.x + 20, lFooterPosition.y + 20, lHalfSize - 20, 25));
            GUILayout.BeginHorizontal();
            for (int i = 0; i < iElements.Length; ++i)
                RenderLeftElement(iElements[i]);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(lFooterPosition.x + lHalfSize, lFooterPosition.y + 20, lHalfSize - 20, 25));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int i = 0; i < iElements.Length; ++i)
                RenderRightElement(iElements[i]);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void RenderRightElement(FooterElement iElement)
        {
            if (iElement == null)
                return;

            switch (iElement.Type) {

                case FooterElementType.VALIDATE_BUTTON:
                    EditorComponents.RenderButton(iElement.Label, iElement.OnClick,
                        EditorComponents.BUTTON_VALIDATE_STYLE, 90, true, iElement.Active);
                    break;

                case FooterElementType.BUTTON:
                    EditorComponents.RenderButton(iElement.Label, iElement.OnClick,
                        EditorComponents.BUTTON_STYLE, 90, true, iElement.Active);
                    break;
            }
        }

        private void RenderLeftElement(FooterElement iElement)
        {
            if (iElement == null)
                return;

            switch (iElement.Type) {

                case FooterElementType.CANCEL_BUTTON:
                    EditorComponents.RenderButton(iElement.Label, iElement.OnClick,
                        EditorComponents.BUTTON_CANCEL_STYLE, 90, true, iElement.Active);
                    break;

                case FooterElementType.TEXT:
                    EditorComponents.RenderText(iElement.Label);
                    break;

                case FooterElementType.ERROR:
                    EditorComponents.RenderText(iElement.Label, EditorComponents.ERROR_STYLE);
                    break;

                case FooterElementType.SUCCESS:
                    EditorComponents.RenderText(iElement.Label, EditorComponents.SUCCESS_STYLE);
                    break;

                case FooterElementType.LINK:
                    EditorComponents.RenderLink(iElement.Label, iElement.OnClick);
                    break;
            }
        }
    }
}