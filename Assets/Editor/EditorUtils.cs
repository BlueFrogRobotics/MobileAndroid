using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using OpenCVUnity;

namespace AndroidMobileApp.Editor
{
    public static class EditorUtils
    {
        public static string Trim(string iStringToTrim)
        {
            if (string.IsNullOrEmpty(iStringToTrim))
                return string.Empty;

            string lNormalizedString = iStringToTrim.Normalize(NormalizationForm.FormD);
            StringBuilder lStringBuilder = new StringBuilder();

            foreach (char lChar in lNormalizedString) {
                if (lChar == ' ')
                    continue;
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(lChar);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    lStringBuilder.Append(lChar);
            }

            return lStringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static void GetAssetFiles(string iDirectory, List<string> ioFiles)
        {
            if (iDirectory.Contains("Editor"))
                return;

            foreach (string lFile in Directory.GetFiles(iDirectory))
                if (!lFile.EndsWith(".meta") && !lFile.EndsWith(".cs"))
                    ioFiles.Add(lFile);

            foreach (string lSubDir in Directory.GetDirectories(iDirectory))
                GetAssetFiles(lSubDir, ioFiles);
        }

        public static void DisplaySeparator(string iLabel = null)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(iLabel))
                GUILayout.Label(iLabel.ToUpper(), EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Space(15);
            GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }

        public static Texture2D LoadImage(string iRessource)
        {
            return AssetDatabase.LoadAssetAtPath("Assets/" + iRessource,
                typeof(Texture2D)) as Texture2D;
        }

        public static Texture2D MakeBackground(int iWidth, int iHeight, Color iColor)
        {
            Color[] lColors = new Color[iWidth * iHeight];
            for (int i = 0; i < lColors.Length; ++i)
                lColors[i] = iColor;

            Texture2D lTexture = new Texture2D(iWidth, iHeight);
            lTexture.SetPixels(lColors);
            lTexture.Apply();

            return lTexture;
        }

        /// <summary>
        /// Converts an T element array to a string. Elements are separate with a coma (without space).
        /// </summary>
        /// <typeparam name="T">Type of element inside the array</typeparam>
        /// <param name="iArray">The array</param>
        /// <param name="iSeparator">Between element separator in the final string. Space by default</param>
        /// <returns>the string value of the array</returns>
        public static string CollectionToString<T>(ICollection<T> iArray, string iSeparator = " ")
        {
            if (iArray == null)
                return string.Empty;

            string oString = string.Empty;

            int lCount = iArray.Count;
            for (int i = 0; i < lCount; ++i)
                oString += iArray.ElementAt(i) + (i < lCount - 1 ? iSeparator : string.Empty);
            return oString;
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="iSerializableObject">Object to serialize</param>
        /// <param name="iPath">Path to the file</param>
        public static void SerializeXML<T>(T iSerializableObject, string iPath)
        {
            if (iSerializableObject == null)
                return;

            try {
                Directory.CreateDirectory(Path.GetDirectoryName(iPath));

                XmlSerializer lSerializer = new XmlSerializer(iSerializableObject.GetType());

                using (FileStream lFileStream = new FileStream(iPath, FileMode.Create))
                using (XmlTextWriter lXMLTextWriter = new XmlTextWriter(lFileStream, Encoding.UTF8)) {
                    lXMLTextWriter.Formatting = Formatting.Indented;
                    lXMLTextWriter.Indentation = 4;
                    XmlWriter lXMLWriter = lXMLTextWriter;
                    lSerializer.Serialize(lXMLWriter, iSerializableObject);
                }
            } catch (Exception lEx) {
                UnityEngine.Debug.LogError(lEx.StackTrace);
            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T">Type of the objet to deserialize</typeparam>
        /// <param name="iPath">Path to the file</param>
        /// <returns>The created object from the XML. Null if unexisting/bad file or bad filename</returns>
        public static T UnserializeXML<T>(string iPath)
        {
            if (string.IsNullOrEmpty(iPath) || !File.Exists(iPath))
                return default(T);
            T oObject = default(T);
            try {
                using (StreamReader lReader = new StreamReader(iPath, Encoding.UTF8, true)) {
                    XmlSerializer lSerializer = new XmlSerializer(typeof(T));
                    oObject = (T)lSerializer.Deserialize(lReader);
                }
            } catch (Exception lEx) {
                UnityEngine.Debug.LogError(lEx.StackTrace);
            }

            return oObject;
        }
    }
}