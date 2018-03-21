using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

namespace AndroidMobileApp.Editor
{
    public class OSDictionary : EditorWindow
    {
        private static DictionaryEditor sWindow;

        [MenuItem("BlueFrog/Edit dictionary", false, 0)]
        static void Init()
        {
            sWindow = (DictionaryEditor)GetWindowWithRect(typeof(DictionaryEditor),
                new Rect(100, 100, 1100, 650), true, "Edit dictionnary");
            sWindow.maxSize = new Vector2(1100, 900);
            sWindow.maxSize = new Vector2(1100, 650);
            sWindow.LoadDictionary(Application.streamingAssetsPath);
        }
    }
}