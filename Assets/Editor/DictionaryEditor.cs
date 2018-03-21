using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AndroidMobileApp.Editor
{
    public class EditedDict
    {
        public LanguageThesaurus Thes { get; set; }
        public string Path { get; set; }
    }

    public class DictionaryEditor : AWindow
    {
        private Dictionary<Language, EditedDict> mEditedDicts;
        private Dictionary<int, Language> mIndexToDict;

        private Dictionary<string, List<DictionaryEntry>> mAllEntries;
        private List<DictionaryEntry> mEditingList;
        private List<string> mOrderedKeys;

        private string mDictionaryPath;
        private Dictionary<string, Language> mAvailableLangs;
        private string[] mDropdownLangsLabels;
        private int[] mDropdownLangsIndex;
        private int mSelectedLang;

        private string mEditingKey;
        private string mNewKey;
        private int mNbEntries;

        private string mAddKeyErrorMessage;
        private string mEditKeyErrorMessage;

        private Vector2 mScrollPosition;
        private Vector2 mSecondScrollPosition;

        private FooterElement lSaveButton;

        private Rect mHeaderPosition;

        private bool mLoaded = false;

        public void LoadDictionary(string iDictionnaryPath)
        {
            if (string.IsNullOrEmpty(iDictionnaryPath)) {
                Debug.LogError("Cannot edit dictionaries. Missing path to Config.");
                Close();
                return;
            }

            mDictionaryPath = iDictionnaryPath;

            LoadEditedDicts();
            RefreshDictionaryEntries();
            RefreshDropdownLangs();

            mLoaded = true;
        }

        void OnEnable()
        {
            lSaveButton = new FooterElement {
                Type = FooterElementType.VALIDATE_BUTTON,
                Label = "Save",
                OnClick = Save
            };

            mNewKey = string.Empty;

            mAvailableLangs = new Dictionary<string, Language>();
            foreach (Language lLang in Enum.GetValues(typeof(Language)))
                mAvailableLangs.Add(LangToFile(lLang), lLang);

            mEditedDicts = new Dictionary<Language, EditedDict>();

            mAllEntries = new Dictionary<string, List<DictionaryEntry>>();
            mOrderedKeys = new List<string>();
            mIndexToDict = new Dictionary<int, Language>();

            mHeaderPosition = new Rect();
            mHeaderPosition.x = 0;
            mHeaderPosition.y = 90;
            mHeaderPosition.width = position.width;
            mHeaderPosition.height = 1;
        }

        protected override void RenderImpl()
        {
            if (!mLoaded) {
                EditorComponents.RenderCenterText("Loading...");
                return;
            }

            mNbEntries = mAllEntries.Count;

            DisplayAddLanguageForm();

            GUILayout.Space(5);

            foreach (KeyValuePair<Language, EditedDict> lKV in mEditedDicts)
                EditorComponents.RenderText("Editing : " + lKV.Value.Path);

            if (mEditedDicts.Count == 0)
                return;

            mHeaderPosition.width = position.width;
            GUI.Label(mHeaderPosition, "", EditorComponents.LINE_STYLE);
            GUILayout.Space(35);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(380));

            DisplayAddKeyForm();

            if (!string.IsNullOrEmpty(mAddKeyErrorMessage))
                EditorComponents.RenderText(mAddKeyErrorMessage, EditorComponents.ERROR_STYLE);

            GUILayout.Space(10);

            if (mNbEntries > 0)
                DisplayKeysList();

            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.BeginVertical();

            if (mEditingList != null)
                DisplayActionArea();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            RenderFooter(lSaveButton);
        }

        private void LoadEditedDicts()
        {
            // Load all language availables
            foreach (string lFile in Directory.GetFiles(mDictionaryPath)) {
                string lFilename = Path.GetFileName(lFile);
                if (!mAvailableLangs.ContainsKey(lFilename))
                    continue;

                Language lLang = mAvailableLangs[lFilename];
                mAvailableLangs.Remove(lFilename);

                LanguageThesaurus lDict = null;
                lDict = (File.ReadAllText(lFile).Length < 10) ? new LanguageThesaurus() : EditorUtils.UnserializeXML<LanguageThesaurus>(lFile);

                mEditedDicts.Add(lLang, new EditedDict() { Path = lFile, Thes = lDict });
            }
        }

        private void RefreshDictionaryEntries()
        {
            mIndexToDict.Clear();
            mOrderedKeys.Clear();
            mAllEntries.Clear();

            // Populate mAllEntries dictionnary.
            // Key: dictionnary key
            // Value: list of dictionnary entries
            int lIndexDict = 0;
            foreach (KeyValuePair<Language, EditedDict> lThes in mEditedDicts) {
                mIndexToDict.Add(lIndexDict++, lThes.Key);
                foreach (DictionaryEntry lEntry in lThes.Value.Thes.Entries) {
                    if (lIndexDict == 1)
                        mOrderedKeys.Add(lEntry.Key);

                    if (mAllEntries.ContainsKey(lEntry.Key))
                        mAllEntries[lEntry.Key].Add(lEntry);
                    else
                        mAllEntries.Add(lEntry.Key, new List<DictionaryEntry>() { lEntry });
                }
            }

            mOrderedKeys.Sort();
        }

        private void RefreshDropdownLangs()
        {
            List<string> lDropdownLangsLabels = new List<string>() { "..." };
            List<int> lDropdownLangsIndex = new List<int>() { -1 };

            foreach (KeyValuePair<string, Language> lLang in mAvailableLangs) {
                lDropdownLangsLabels.Add(lLang.Value.ToString());
                lDropdownLangsIndex.Add((int)lLang.Value);
            }

            mDropdownLangsLabels = lDropdownLangsLabels.ToArray();
            mDropdownLangsIndex = lDropdownLangsIndex.ToArray();
            mSelectedLang = -1;
        }

        private void DisplayKeysList()
        {
            mScrollPosition = EditorComponents.BeginScrollView(mScrollPosition);

            string lKeyToDelete = null;

            foreach (string lKey in mOrderedKeys) {
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();

                EditorComponents.RenderText(lKey, GUILayout.Width(200));

                EditorComponents.RenderButton("Edit", () => {
                    EditEntry(lKey);
                }, 60);

                EditorComponents.RenderButton("Delete", () => {
                    lKeyToDelete = lKey;
                }, 60);

                GUILayout.EndHorizontal();

                GUILayout.Space(5);
            }

            if (!string.IsNullOrEmpty(lKeyToDelete))
                DeleteEntry(lKeyToDelete);

            EditorComponents.EndScrollView();
        }

        private void DisplayAddLanguageForm()
        {
            GUILayout.BeginHorizontal();

            mSelectedLang = EditorComponents.RenderIntDropdown(mSelectedLang, mDropdownLangsLabels, mDropdownLangsIndex,
                null, 100);

            if (!Enum.IsDefined(typeof(Language), mSelectedLang))
                GUI.enabled = false;

            EditorComponents.RenderButton("Add language", () => {
                AddLanguage((Language)Enum.GetValues(typeof(Language)).GetValue(mSelectedLang));
            }, 120);

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void DisplayAddKeyForm()
        {
            GUILayout.BeginHorizontal();

            mNewKey = EditorComponents.RenderTextField(mNewKey);

            if (!IsValidKeyName(mNewKey, ref mAddKeyErrorMessage))
                GUI.enabled = false;

            EditorComponents.RenderButton("ADD KEY", () => {
                AddEntry(mNewKey);
                mNewKey = string.Empty;
            }, 125);

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void DisplayActionArea()
        {
            EditorComponents.RenderText("KEY", EditorComponents.TEXT_LABEL_STYLE);

            GUILayout.BeginHorizontal();

            mEditingKey = EditorComponents.RenderTextField(mEditingKey);

            if (mEditingKey == mEditingList[0].Key) {
                mEditKeyErrorMessage = null;
                GUI.enabled = false;
            } else if (!IsValidKeyName(mEditingKey, ref mEditKeyErrorMessage))
                GUI.enabled = false;

            EditorComponents.RenderButton("CHANGE", () => {
                mAllEntries.Remove(mEditingList[0].Key);
                mAllEntries[mEditingKey] = mEditingList;

                for (int i = 0; i < mEditingList.Count; ++i)
                    mEditingList[i].Key = mEditingKey;

                GUI.FocusControl(string.Empty);

                RefreshDictionaryEntries();
                Save();
            }, 70);


            GUI.enabled = true;
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(mEditKeyErrorMessage))
                EditorComponents.RenderText(mEditKeyErrorMessage, EditorComponents.ERROR_STYLE);

            GUILayout.Space(10);

            mSecondScrollPosition = EditorComponents.BeginScrollView(mSecondScrollPosition);

            for (int i = 0; i < mEditingList.Count; ++i) {
                DictionaryEntry lEntry = mEditingList[i];

                GUILayout.Space(10);

                EditorComponents.RenderText(mIndexToDict[i].ToString(), EditorComponents.TEXT_LABEL_STYLE);
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                EditorComponents.RenderText("Base", GUILayout.Width(100));
                lEntry.BaseValue = EditorComponents.RenderTextField(lEntry.BaseValue, null, 500);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                EditorComponents.RenderText("Close phonetics", GUILayout.Width(100));
                string lClosePhoneticEntries = EditorUtils.CollectionToString(mEditingList[i].ClosePhoneticValues, "/");
                lEntry.ClosePhoneticValues = new List<string>(EditorComponents.RenderTextField(lClosePhoneticEntries, null, 500).Split('/'));

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();

                EditorComponents.RenderText("Random", GUILayout.Width(100));
                string lRandomEntries = EditorUtils.CollectionToString(mEditingList[i].RandomValues, "/");
                lEntry.RandomValues = new List<string>(EditorComponents.RenderTextField(lRandomEntries, null, 500).Split('/'));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            EditorComponents.EndScrollView();
        }

        private void AddEntry(string iNewKey)
        {
            // Add key to mAllEntries dictionary
            // Add key to mOrderedKeys to display keys sorted
            mAllEntries.Add(iNewKey, new List<DictionaryEntry>());
            mOrderedKeys.Add(iNewKey);
            mOrderedKeys.Sort();

            // Create a new DictionnaryEntry for each language of the new key
            foreach (KeyValuePair<Language, EditedDict> lDict in mEditedDicts) {
                DictionaryEntry lNewEntry = new DictionaryEntry() {
                    Key = iNewKey,
                    BaseValue = string.Empty,
                    RandomValues = new List<string>(),
                    ClosePhoneticValues = new List<string>()
                };

                lDict.Value.Thes.Entries.Add(lNewEntry);
                mAllEntries[iNewKey].Add(lNewEntry);
            }

            mEditingKey = iNewKey;
            mEditingList = mAllEntries[iNewKey];
            iNewKey = string.Empty;
            GUI.FocusControl(string.Empty);

            Save();
        }

        private void EditEntry(string iKey)
        {
            mEditingKey = iKey;
            mEditingList = mAllEntries[iKey];
            GUI.FocusControl(string.Empty);
        }

        private void DeleteEntry(string iKey)
        {
            if (iKey == mEditingKey) {
                mEditingKey = string.Empty;
                mEditingList = null;
            }

            // Remove entry from mAllEntries
            // Remove entry from mOrderedKeys
            mAllEntries.Remove(iKey);
            mOrderedKeys.Remove(iKey);
            mOrderedKeys.Sort();

            // Remove all dictionnary entries at the key
            foreach (KeyValuePair<Language, EditedDict> lDict in mEditedDicts) {
                for (int i = 0; i < lDict.Value.Thes.Entries.Count; ++i)
                    if (lDict.Value.Thes.Entries[i].Key == iKey)
                        lDict.Value.Thes.Entries.RemoveAt(i);
            }

            Save();
        }

        private void Save()
        {
            foreach (KeyValuePair<Language, EditedDict> lDict in mEditedDicts)
                EditorUtils.SerializeXML(lDict.Value.Thes, lDict.Value.Path);
        }

        private void AddLanguage(Language lLang)
        {
            string lLangFile = mDictionaryPath + "/" + LangToFile(lLang);
            if (File.Exists(lLangFile)) {
                Debug.LogError("Cannot add language. The language file already exists.");
                return;
            }
            mLoaded = false;

            FileStream lFileStream = File.Create(lLangFile);
            lFileStream.Close();

            LoadEditedDicts();

            EditedDict lNewLangDic = mEditedDicts[lLang];
            foreach (DictionaryEntry lEntry in mEditedDicts.First().Value.Thes.Entries) {
                lNewLangDic.Thes.Entries.Add(new DictionaryEntry() {
                    Key = lEntry.Key,
                    BaseValue = string.Empty,
                    RandomValues = new List<string>(),
                    ClosePhoneticValues = new List<string>()
                });
            }
            EditorUtils.SerializeXML(lNewLangDic.Thes, lNewLangDic.Path);

            RefreshDictionaryEntries();
            RefreshDropdownLangs();

            if (mEditingList != null && mEditingList.Count > 0)
                EditEntry(mEditingList[0].Key);

            GUI.FocusControl(string.Empty);
            mLoaded = true;
        }

        private bool IsValidKeyName(string iKeyName, ref string iErrorMessage)
        {
            if (string.IsNullOrEmpty(iKeyName)) {
                iErrorMessage = null;
                return false;
            }

            if (!Regex.IsMatch(iKeyName, @"^[a-z]+$")) {
                iErrorMessage = "The key must contains only minuscule alpha characters.";
                return false;
            }

            if (mAllEntries.ContainsKey(iKeyName)) {
                iErrorMessage = "This key already exists.";
                return false;
            }

            iErrorMessage = null;
            return true;
        }

        private string LangToFile(Language lLang)
        {
            return lLang.ToString().ToLower() + ".xml";
        }

    }
}