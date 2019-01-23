using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

public enum Language
{
    EN,
    FR
}

public enum EntryType
{
    /// <summary>
    /// Unique value for UI display purpose
    /// </summary>
    BASE,

    /// <summary>
    /// Dictionary entry for STT purpose mainly
    /// </summary>
    CLOSE_PHONETIC,

    /// <summary>
    /// Dictionary entry for TTS purpose mainly
    /// </summary>
    RANDOM
}

/// <summary>
/// Key-value dictionary base entry
/// </summary>
[Serializable]
public class DictionaryEntry
{
    /// <summary>
    /// Key of the value
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Display value
    /// </summary>
    public string BaseValue { get; set; }

    /// <summary>
    /// Listen list for STT input processing
    /// </summary>
    public List<string> ClosePhoneticValues { get; set; }

    /// <summary>
    /// Say list containing value that will be choosen randomly 
    /// </summary>
    public List<string> RandomValues { get; set; }
}

/// <summary>
/// Language dictionary storage. Contains key-value entries for one language
/// </summary>
[Serializable]
public class LanguageThesaurus
{
    /// <summary>
    /// All key-value entries of the thesaurus
    /// </summary>
    public List<DictionaryEntry> Entries { get; set; }

    /// <summary>
    /// Represents all the dictionary entries
    /// </summary>
    public LanguageThesaurus()
    {
        Entries = new List<DictionaryEntry>();
    }

    /// <summary>
    /// Check existing key in existing entries
    /// </summary>
    /// <param name="iKey">Key to check</param>
    /// <returns>True if the key already exists in Entries</returns>
    public bool ContainsKey(string iKey)
    {
        int lLength = Entries.Count;
        for (int i = 0; i < lLength; ++i)
            if (Entries[i].Key == iKey)
                return true;
        return false;
    }
}
/// <summary>
/// Hosts all global_keyword -> currentlanguage_word / word list matchings
/// </summary>
public sealed class Dictionary
{
    private Trie<Dictionary<EntryType, string[]>> mCurrentNativeTrie;

    internal Dictionary(Language iLang, PoolManager lPoolManager)
    {
        lPoolManager.StartCoroutine(SetLanguage(iLang));
    }

    /// <summary>
    /// Retrieve the corresponding word from the input key word. Checks first in app dict if exists, if not checks in native dict.
    /// Lookup speed in Th(iKey.Length)
    /// </summary>
    /// <param name="iKey">The key word corresponding to the wished word</param>
    /// <param name="iContext">The context corresponding to the dictionary to use</param>
    /// <returns>The word in the current language, default value if not found</returns>
    public string GetString(string iKey)
    {
        string oVal = string.Empty;
        if (string.IsNullOrEmpty(iKey))
            return oVal;
        Dictionary<EntryType, string[]> lSearchResult = null;

        if (mCurrentNativeTrie != null)
            lSearchResult = mCurrentNativeTrie.Find(iKey);
        if (lSearchResult == null)
            return oVal;
        oVal = lSearchResult[EntryType.BASE][0];
        return oVal;
    }

    /// <summary>
    /// Retrieve a random word from the input key word. Checks first in app dict if exists, if not checks in native dict.
    /// Lookup speed in Th(iKey.Length)
    /// </summary>
    /// <param name="iKey">The key word corresponding to the wished random word</param>
    /// <returns>The word in the current language, default value if not found</returns>
    public string GetRandomString(string iKey)
    {
        string oVal = string.Empty;

        if (string.IsNullOrEmpty(iKey))
            return oVal;

        Dictionary<EntryType, string[]> lSearchResult = null;

        if (mCurrentNativeTrie != null)
            lSearchResult = mCurrentNativeTrie.Find(iKey);

        if (lSearchResult == null)
            return oVal;

        string[] lSayEntries = lSearchResult[EntryType.RANDOM];
        if (lSayEntries.Length > 0)
            oVal = lSayEntries[UnityEngine.Random.Range(0, (int)Mathf.Round(lSayEntries.Length))];

        return oVal;
    }

    /// <summary>
    /// Retrieve the corresponding list to the input iKey. Useful for multiple response handling with the SpeechToText
    /// </summary>
    /// <param name="iKey">The key corresponding to the wished list</param>
    /// <returns>List of strings that must sound similar</returns>
    public string[] GetPhoneticStrings(string iKey)
    {
        if (string.IsNullOrEmpty(iKey))
            return new string[0];

        Dictionary<EntryType, string[]> oSearchResult = null;

        if (mCurrentNativeTrie != null)
            oSearchResult = mCurrentNativeTrie.Find(iKey);

        if (oSearchResult == null)
            return new string[0];

        return oSearchResult[EntryType.CLOSE_PHONETIC];
    }

    /// <summary>
    /// Proceed to a GetPhoneticsStrings(iKey, iContext = APP) and checks if one element on the closephonetic array is contained in iRef
    /// </summary>
    /// <param name="iRef">The sentence to compare to each word into the list</param>
    /// <param name="iKey">The key corresponding to the wished list</param>
    /// <returns>True if the one element of the closephonetic array in contained in iRef sentence (not null and not empty), false otherwise</returns>
    public bool ContainsPhonetic(string iRef, string iKey)
    {
        if (string.IsNullOrEmpty(iKey) || string.IsNullOrEmpty(iRef))
            return false;

        string[] lClosePhonetics = GetPhoneticStrings(iKey);

        if (lClosePhonetics == null || lClosePhonetics.Length == 0)
            return false;

        iRef = iRef.ToLower();
        for (int i = 0; i < lClosePhonetics.Length; ++i)
            if (iRef.Contains(lClosePhonetics[i].ToLower()))
                return true;

        return false;
    }

    internal IEnumerator SetLanguage(Language iLang)
    {
        string lFilePath = Application.streamingAssetsPath + "/" + LangToFile(iLang);
        Debug.Log("dict file path: " + lFilePath);
        //string lFilePath = "jar:file://"+Application.dataPath + "!/assets/" + LangToFile(iLang);
        WWW lwww = new WWW(lFilePath);
        Debug.Log("set language: " + lFilePath);
        yield return lwww;
        if (lwww.isDone/*File.Exists(lFilePath)*/) {
            
            //LanguageThesaurus lNativeThes = UnserializeXML<LanguageThesaurus>(lFilePath);
            LanguageThesaurus lNativeThes = UnserializeXMLFromString<LanguageThesaurus>(lwww.text);
            Debug.Log("file exists"+ lwww.text);
            if (lNativeThes != null)
            {
                Debug.Log("lnative pas null");
                mCurrentNativeTrie = BuildTrie(lNativeThes.Entries);
            }
            lNativeThes = null;
        }
    }

//    IEnumerator testLoad()
//    {
//        string testPath = Path.Combine(Application.streamingAssetsPath, "testText.txt");
//        Debug.Log(testPath);
////#if UNITY_ANDROID
//        UnityWebRequest www = UnityWebRequest.Get(testPath);
//        yield return www.SendWebRequest();
//        if (www.isNetworkError || www.isHttpError)
//            Debug.Log(www.error);
//        else
//            Debug.Log(www.downloadHandler.text);
////#endif
//    }

    private Trie<Dictionary<EntryType, string[]>> BuildTrie(List<DictionaryEntry> iEntries)
    {
        Trie<Dictionary<EntryType, string[]>> oTrie = new Trie<Dictionary<EntryType, string[]>>();

        int lCount = iEntries.Count;

        for (int i = 0; i < lCount; ++i) {
            DictionaryEntry lEntry = iEntries[i];
            Dictionary<EntryType, string[]> lDicEntry = new Dictionary<EntryType, string[]>();
            lDicEntry.Add(EntryType.BASE, new string[] { lEntry.BaseValue });
            lDicEntry.Add(EntryType.CLOSE_PHONETIC, lEntry.ClosePhoneticValues.ToArray());
            lDicEntry.Add(EntryType.RANDOM, lEntry.RandomValues.ToArray());
            oTrie.Add(lEntry.Key, lDicEntry);
        }

        return oTrie;
    }

    private string LangToFile(Language iLang)
    {
        return iLang.ToString().ToLower() + ".xml";
    }

    /// <summary>
    /// Deserializes an xml file into an object list
    /// </summary>
    /// <typeparam name="T">Type of the objet to deserialize</typeparam>
    /// <param name="iPath">Path to the file</param>
    /// <returns>The created object from the XML. Null if unexisting/bad file or bad filename</returns>
    private static T UnserializeXML<T>(string iPath)
    {
        if (string.IsNullOrEmpty(iPath) || !File.Exists(iPath))
            return default(T);
        T oObject = default(T);
        try {
            //WWW lWWW = new WWW(iPath);

            using (StreamReader lReader = new StreamReader(iPath, Encoding.UTF8, true)) {
                XmlSerializer lSerializer = new XmlSerializer(typeof(T));
                oObject = (T)lSerializer.Deserialize(lReader);
            }
        } catch (Exception lEx) {
            UnityEngine.Debug.LogError(lEx.StackTrace);
        }

        return oObject;
    }

    private static T UnserializeXMLFromString<T>(string iText)
    {
        
        T oObject = default(T);
        try
        {
            //WWW lWWW = new WWW(iPath);

            using (TextReader lReader = new StringReader(iText))
            {
                lReader.Read();
                XmlSerializer lSerializer = new XmlSerializer(typeof(T));
                oObject = (T)lSerializer.Deserialize(lReader);
            }
        }
        catch (Exception lEx)
        {
            UnityEngine.Debug.LogError(lEx.StackTrace);
        }

        return oObject;
    }
}
