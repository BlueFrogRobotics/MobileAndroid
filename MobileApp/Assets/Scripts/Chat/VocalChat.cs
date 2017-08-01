using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using BuddyAPI;
//using BuddyOS;
//using SpeechToText = BuddyAPI.SpeechToText;
//using TextToSpeech = BuddyAPI.TextToSpeech;

using Buddy;

internal enum RequestType
{
    DEFINITION,
    NEWS,
    WEATHER
}

public delegate void QuestionAnalysed(string iType);

/// <summary>
/// Copy of VocalChat from SDK's FeatVocal
/// </summary>
public class VocalChat : MonoBehaviour
{
    [SerializeField]
    private TextToSpeech mTTS;

    [SerializeField]
    private SpeechToText mSTT;

    [SerializeField]
    private Face mFace;

    [SerializeField]
    private ChatManager mChat;

    private short mErrorCount;
    private string mQuestionsFile;
    private string mSynonymesFile;

    private Dictionary<RequestType, string> mWebsiteHash;

    private List<string> mAcceptSpeech;
    private List<string> mAnotherSpeech;
    private List<string> mAnswersSpeech;
    private List<string> mDateSpeech;
    private List<string> mDefinitionSpeech;
    private List<string> mDegreesCSpeech;
    private List<string> mDidntUnderstandSpeech;
    private List<string> mDontMoveSpeech;
    private List<string> mGameSpeech;
    private List<string> mGetSpeech;
    private List<string> mHourSpeech;
    private List<string> mICouldntSpeech;
    private List<string> mISpeech;
    private List<string> mLookForSpeech;
    private List<string> mMeteoSpeech;
    private List<string> mNewsSpeech;
    private List<string> mPhotoSpeech;
    private List<string> mPoseSpeech;
    private List<string> mQuestionSpeech;
    private List<string> mQuitSpeech;
    private List<string> mQuizzSpeech;
    private List<string> mSeveralResSpeech;
    private List<string> mSorrySpeech;
    private List<string> mStorySpeech;
    private List<string> mTemperatureSpeech;
    private List<string> mTempSpeech;
    private List<string> mThanksSpeech;
    private List<string> mURWelcomeSpeech;
    private List<string> mWanderSpeech;
    private List<string> mWantToKnowSpeech;

    public QuestionAnalysed OnQuestionTypeFound { get; set; }

    void Start()
    {
        mErrorCount = 0;

        StreamReader lstreamReader = new StreamReader(ResourceManager.StreamingAssetFilePath("questions.xml"));
        mQuestionsFile = lstreamReader.ReadToEnd();
        lstreamReader.Close();
        lstreamReader = new StreamReader(ResourceManager.StreamingAssetFilePath("synonymes.xml"));
        mSynonymesFile = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        mWebsiteHash = new Dictionary<RequestType, string>();
        mWebsiteHash.Add(RequestType.WEATHER, "http://www.infoclimat.fr/public-api/gfs/xml?_auth=ABpQRwB%2BUnAFKFViBnBXflM7BzJdK1RzA39VNl8xAn8CaANjVD9VNFc8VyoALwMzAi9TOwg3CDgDaQdiWigEeABgUDQAYFI1BWxVPwY3V3xTfwdlXWdUcwN%2FVTpfMAJ%2FAmMDY1QxVSlXP1c9AC4DNQIxUzoIKAgvA2EHY1ozBG8Aa1AyAGZSNAVpVTQGKVd8U2UHZ11kVGoDYFVmXzsCaAJoA2ZUMVU2V2lXPQAuAzQCM1MwCDQIMANoB2RaNQR4AHxQTQAQUi0FKlV1BmNXJVN9BzJdPFQ4&_c=f30d5acf5e18de4f029990712316a936&_ll=");
        mWebsiteHash.Add(RequestType.DEFINITION, "https://fr.wikipedia.org/w/api.php?format=xml&action=query&prop=extracts|categories|links&exintro=&explaintext=&titles=");
        mWebsiteHash.Add(RequestType.NEWS, "https://api.cognitive.microsoft.com/bing/v5.0/news/?Market=fr-FR&?count=1");

        // Init list of speech with same meaning
        InitSpeech();

        // starting STT with callback
        mSTT.OnBestRecognition.Add(OnSpeechRecognition);
    }

    private void InitSpeech()
    {
        mAcceptSpeech = new List<string>();
        mAnotherSpeech = new List<string>();
        mAnswersSpeech = new List<string>();
        mDateSpeech = new List<string>();
        mDefinitionSpeech = new List<string>();
        mDegreesCSpeech = new List<string>();
        mDidntUnderstandSpeech = new List<string>();
        mDontMoveSpeech = new List<string>();
        mGameSpeech = new List<string>();
        mGetSpeech = new List<string>();
        mHourSpeech = new List<string>();
        mICouldntSpeech = new List<string>();
        mISpeech = new List<string>();
        mLookForSpeech = new List<string>();
        mMeteoSpeech = new List<string>();
        mNewsSpeech = new List<string>();
        mPhotoSpeech = new List<string>();
        mPoseSpeech = new List<string>();
        mQuestionSpeech = new List<string>();
        mQuitSpeech = new List<string>();
        mQuizzSpeech = new List<string>();
        mSeveralResSpeech = new List<string>();
        mSorrySpeech = new List<string>();
        mStorySpeech = new List<string>();
        mTemperatureSpeech = new List<string>();
        mTempSpeech = new List<string>();
        mThanksSpeech = new List<string>();
        mURWelcomeSpeech = new List<string>();
        mWanderSpeech = new List<string>();
        mWantToKnowSpeech = new List<string>();

        FillListSyn("Accept", mAcceptSpeech);
        FillListSyn("Another", mAnotherSpeech);
        FillListSyn("Answers", mAnswersSpeech);
        FillListSyn("Date", mDateSpeech);
        FillListSyn("Definition", mDefinitionSpeech);
        FillListSyn("DegreesC", mDegreesCSpeech);
        FillListSyn("DidntUnderstand", mDidntUnderstandSpeech);
        FillListSyn("DontMove", mDontMoveSpeech);
        FillListSyn("Game", mGameSpeech);
        FillListSyn("Get", mGetSpeech);
        FillListSyn("Hour", mHourSpeech);
        FillListSyn("ICouldnt", mICouldntSpeech);
        FillListSyn("I", mISpeech);
        FillListSyn("LookFor", mLookForSpeech);
        FillListSyn("Meteo", mMeteoSpeech);
        FillListSyn("News", mNewsSpeech);
        FillListSyn("Photo", mPhotoSpeech);
        FillListSyn("Pose", mPoseSpeech);
        FillListSyn("Question", mQuestionSpeech);
        FillListSyn("Quit", mQuitSpeech);
        FillListSyn("Quizz", mQuizzSpeech);
        FillListSyn("SeveralRes", mSeveralResSpeech);
        FillListSyn("Sorry", mSorrySpeech);
        FillListSyn("Story", mStorySpeech);
        FillListSyn("Temperature", mTemperatureSpeech);
        FillListSyn("Temp", mTempSpeech);
        FillListSyn("Thanks", mThanksSpeech);
        FillListSyn("URWelcome", mURWelcomeSpeech);
        FillListSyn("Wander", mWanderSpeech);
        FillListSyn("WantToKnow", mWantToKnowSpeech);
    }

    private void FillListSyn(string iXmlCode, List<string> iSynList)
    {
        using (XmlReader lReader = XmlReader.Create(new StringReader(mSynonymesFile)))
        {

            if (lReader.ReadToFollowing(iXmlCode))
            {
                string lContent = lReader.ReadElementContentAsString();
                string[] lSynonymes = lContent.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lSynonymes.Length; ++i)
                    iSynList.Add(lSynonymes[i]);
            }
        }
    }

    public void StartDialogueWithPhrase()
    {
        TTSProcessAndSay("Que puis-je faire pour vous?");
        mFace.SetExpression(MoodType.LISTENING);
        mSTT.Request();
    }

    public void StartReco()
    {
        mSTT.Request();
    }

    private void TTSProcessAndSay(string iSpeech, bool iStack = false)
    {
        //Debug.Log("Saying: " + iSpeech);
        mChat.NewBuddyMessage(iSpeech.Replace("[silence]", ""));
        string lCorrectedSpeech = iSpeech.Replace("vous", "vou");
        string[] lSentences = lCorrectedSpeech.Split(new string[] { "[silence]" }, StringSplitOptions.None);

        foreach (string lSentence in lSentences)
        {
            mTTS.Say(lSentence, iStack);
            mTTS.Silence(500, true);
        }
    }

    private void OnSpeechRecognition(string iVoiceInput)
    {
        //Debug.Log("OnSpeechReco");
        mFace.SetExpression(MoodType.NEUTRAL);
        //mLED.SetBodyLight(LEDColor.BLUE_NEUTRAL);          
        mErrorCount = 0;
        string lLowVoiceInput = iVoiceInput.ToLower();
        Debug.Log("On Speech Recognition input : " + lLowVoiceInput);

        if (!SpecialRequest(lLowVoiceInput))
            BuildGeneralAnswer(lLowVoiceInput);
    }

    private void OnPartialRecognition(string iVoiceInput)
    {
        //Debug.Log("OnPartialReco");
        //Debug.Log("[chatbot Partial Reco] : " + iVoiceInput);
        mFace.SetExpression(MoodType.NEUTRAL);
        //mLED.SetBodyLight(LEDColor.BLUE_NEUTRAL);
        mErrorCount = 0;
        string lLowVoiceInput = iVoiceInput.ToLower();
        Debug.Log("On Partial Recognition input : " + lLowVoiceInput);

        if (!SpecialRequest(lLowVoiceInput))
            BuildGeneralAnswer(lLowVoiceInput);
    }

    private void ErrorSTT(STTError iError)
    {
        //mFace.SetExpression(MoodType.SAD);
        //Debug.Log("[chatbot error] : " + iError);
        ++mErrorCount;
        //Debug.Log("[chatbot error] : count " + mErrorCount);

        // If too much error (or no answer), ask for answer. If still no answer, get back to IDLE
        if (mErrorCount == 4)
        {
            TTSProcessAndSay("Je ne vous entends plus ! [silence] Etes-vous toujours là?");
        }
        else if (mErrorCount > 6)
        {
            TTSProcessAndSay("Désolé je ne vous entends plu");
        }
        else
        {
            string lSentence = "";

            switch (iError)
            {
                case STTError.ERROR_AUDIO: lSentence = "Il y a un problème avec le micro !"; break;
                case STTError.ERROR_NETWORK: lSentence = "Il y a un problème de connexion !"; break;
                case STTError.ERROR_RECOGNIZER_BUSY: lSentence = "La reconaissance vocale est déjà occupée !"; break;
                case STTError.ERROR_SPEECH_TIMEOUT: lSentence = "Je n'ai rien entendu. Pouvez vous répéter ?"; break;
                default: lSentence = RandomString(mDidntUnderstandSpeech); break;
            }

            TTSProcessAndSay(lSentence);
        }
    }

    private bool SpecialRequest(string iSpeech)
    {
        string lType = "";

        if (ContainsOneOf(iSpeech, mQuitSpeech))
            lType = "Quit";
        else if (ContainsOneOf(iSpeech, mMeteoSpeech))
        {
            lType = "Weather";
            //We search for the location of the weather request
            int lKeywordIndex = WordIndexOfOneOf(iSpeech, mMeteoSpeech);
            string[] lWords = iSpeech.Split(' ');
            string lWeatherPlace = "";

            if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
            {
                for (int j = lKeywordIndex + 2; j < lWords.Length; j++)
                    lWeatherPlace += lWords[j] + " ";
            }
            StartCoroutine(BuildWeatherAnswer(lWeatherPlace));
        }
        else if (ContainsOneOf(iSpeech, mDefinitionSpeech))
        {
            lType = "Definition";
            //We search for the location of the weather request
            int lKeywordIndex = WordIndexOfOneOf(iSpeech, mDefinitionSpeech);
            string[] lWords = iSpeech.Split(' ');
            string lDefinitionWord = "";

            if (lKeywordIndex != -1 && lKeywordIndex != lWords.Length)
            {
                for (int j = lKeywordIndex + 1; j < lWords.Length; j++)
                    lDefinitionWord += lWords[j] + " ";
            }
            StartCoroutine(BuildDefinitionAnswer(lDefinitionWord));
        }
        else if (ContainsOneOf(iSpeech, mQuizzSpeech))
            lType = "Quizz";
        else if (iSpeech.Contains("calcul"))
            lType = "Calcul";
        else if (iSpeech.Contains("mémoire"))
            lType = "Memory";
        else if (ContainsOneOf(iSpeech, mPoseSpeech))
            lType = "Pose";
        else if (ContainsOneOf(iSpeech, mPhotoSpeech))
            lType = "Photo";
        else if (ContainsOneOf(iSpeech, mStorySpeech))
            lType = "Story";
        else if (ContainsOneOf(iSpeech, mGameSpeech))
        {
            if (iSpeech.Contains("mémoire"))
                lType = "Memory";
            else if (iSpeech.Contains("couleur"))
                lType = "Colors";
            else
                lType = "Games";
        }
        else if (ContainsOneOf(iSpeech, mThanksSpeech))
            TTSProcessAndSay(RandomString(mURWelcomeSpeech));
        else if (ContainsOneOf(iSpeech, mDateSpeech))
            TTSProcessAndSay("Nous sommes le  " + DateTime.Now.Day +
                " " + DateTime.Now.Month +
                " " + DateTime.Now.Year, true);
        else if (ContainsOneOf(iSpeech, mHourSpeech))
        {
            TTSProcessAndSay("Au troisième bip il sera exactement " +
                DateTime.Now.Hour + " heure " +
                DateTime.Now.Minute + " minutes et " +
                DateTime.Now.Second + " secondes ", true);
            mTTS.Silence(1000, true);
            TTSProcessAndSay("bip", true);
            mTTS.Silence(1000, true);
            TTSProcessAndSay("bip", true);
            mTTS.Silence(1000, true);
            TTSProcessAndSay("et bip", true);
        }
        else if (iSpeech.Contains("propose"))
            lType = Suggest();
        else if (ContainsOneOf(iSpeech, mWanderSpeech))
            lType = "Wander";
        else if (ContainsOneOf(iSpeech, mDontMoveSpeech))
            lType = "DontMove";
        else
            return false;
        OnQuestionTypeFound(lType);
        return true;
    }

    private void BuildGeneralAnswer(string iData)
    {
        string lFormatedData = Regex.Replace(iData, @"[^\w\s]", " ");
        //Debug.Log("BuildGeneralAnswer - ponctu " + lFormatedData);
        string lAnswer = "";

        if (ContainsOneOf(lFormatedData, mAcceptSpeech))
        {
            //TTSProcessAndSay(RandomString(mAcceptSpeech) + " J'écoute votre " + RandomString(mQuestionSpeech) + " ?", true);
        }
        else
        {
            string[] lWords = lFormatedData.Split(' ');

            using (XmlReader lReader = XmlReader.Create(new StringReader(mQuestionsFile)))
            {
                while (lReader.ReadToFollowing("QA"))
                {
                    lReader.ReadToFollowing("question");
                    //Remove ponctuation
                    string lContentQ = Regex.Replace(lReader.ReadElementContentAsString().ToLower(), @"[^\w\s]", " ");
                    //Debug.Log("Question series : " + lContentQ);

                    if (lContentQ.Contains(lFormatedData))
                    {
                        Debug.Log("Found Content Question : " + lContentQ);
                        bool lFoundInput = true;
                        if (lFoundInput)
                        {
                            lReader.ReadToFollowing("answer");
                            string lContentA = lReader.ReadElementContentAsString();
                            string[] lAnswers = lContentA.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                            if (lAnswers.Length == 1)
                                lAnswer = lAnswers[0];
                            else
                            {
                                System.Random lRnd = new System.Random();
                                lAnswer = lAnswers[lRnd.Next(0, lAnswers.Length)];
                            }
                            //Debug.Log("Found Content Answer : " + lAnswer);
                        }
                        else
                        {
                            lAnswer = RandomString(mSorrySpeech) + " " +
                                RandomString(mICouldntSpeech) + " " +
                                RandomString(mGetSpeech) + " " +
                                RandomString(mAnswersSpeech);
                            OnQuestionTypeFound("");
                        }
                        break;
                    }
                }
                TTSProcessAndSay(lAnswer, true);
                //Debug.Log("BuildGeneralAnswer " + data + " " + lAnswer);
                //TTSProcessAndSay(RandomString(mAnOtherSpeech) + " " + RandomString(mQuestionSpeech) + " ?", true);
            }
        }
    }

    private IEnumerator BuildWeatherAnswer(string iData)
    {
        string lXmlData = "";
        string lKeyword = "48.853,2.35";

        if (!string.IsNullOrEmpty(iData))
        {
            WWW lWww = new WWW("http://maps.googleapis.com/maps/api/geocode/xml?address=" + iData);

            float lElapsedTime = 0.0f;
            while (!lWww.isDone)
            {
                lElapsedTime += Time.deltaTime;

                if (lElapsedTime >= 5f)
                    break;

                yield return new WaitForSeconds(0.5f);
            }

            if (lWww.isDone && string.IsNullOrEmpty(lWww.error))
            {
                string lXml = lWww.text;
                using (XmlReader lReader = XmlReader.Create(new StringReader(lXml)))
                {
                    if (lReader.ReadToFollowing("location"))
                    {
                        lReader.ReadToFollowing("lat");
                        lReader.Read();
                        lKeyword = lReader.ReadContentAsString();
                        lReader.ReadToFollowing("lng");
                        lReader.Read();
                        lKeyword += "," + lReader.ReadContentAsString();
                    }
                    lReader.Close();
                }
            }
        }
        else if (Input.location.isEnabledByUser)
        {
            Input.location.Start();

            int lmaxWait = 7;
            while (Input.location.status == LocationServiceStatus.Initializing && lmaxWait > 0)
            {
                yield return new WaitForSeconds(1f);
                lmaxWait--;
                Debug.Log("Waiting for geoloc");
            }
            if (Input.location.status != LocationServiceStatus.Failed && lmaxWait > 0)
            {
                lKeyword = Input.location.lastData.latitude + "," + Input.location.lastData.longitude;
            }

            Input.location.Stop();
        }

        yield return StartCoroutine(MakeRequest(RequestType.WEATHER, lKeyword, null, value => lXmlData = value));

        //We take the answer from the weather website and extract the temperature as a string
        string lTemperature = "";
        WeatherInfo[] lInfos = new WeatherInfo[6];

        using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData)))
        {
            while (lReader.ReadToFollowing("echeance"))
            {
                lReader.MoveToAttribute("timestamp");
                string lReadTime = lReader.Value.Substring(0, lReader.Value.Length - 4);
                if (DateTime.Now.CompareTo(Convert.ToDateTime(lReadTime)) < 0)
                {
                    lReader.ReadToFollowing("level");
                    lTemperature = lReader.ReadElementContentAsString();
                    break;
                }
            }

            int lCount = 0;
            while (lCount < 6 && lReader.ReadToFollowing("echeance"))
            {
                lReader.MoveToAttribute("timestamp");
                string lReadTime = lReader.Value;
                string[] lReadTimeSplit = lReadTime.Split(' ');
                int lHour = 0;
                Int32.TryParse(lReadTimeSplit[1].Split(':')[0], out lHour);

                lReader.ReadToFollowing("level");
                string lTemp = lReader.ReadElementContentAsString();
                int lTempInt = 0;
                if (Int32.TryParse(lTemp.Split('.')[0], out lTempInt))
                    lTempInt -= 274;

                lReader.ReadToFollowing("pluie");
                int lRainLevel = 0;
                Int32.TryParse(lReader.ReadElementContentAsString(), out lRainLevel);

                lInfos[lCount] = new WeatherInfo
                {
                    Hour = lHour,
                    Temperature = lTempInt,
                    Type = lRainLevel > 3 ? WeatherType.RAIN : WeatherType.SUNNY
                };

                lCount++;
            }
            lReader.Close();
        }

        string[] lsubstrings = lTemperature.Split('.');
        int loutValue = 0;
        string lFinalSentence = RandomString(mICouldntSpeech) +
            " " + RandomString(mGetSpeech) +
            " " + RandomString(mTemperatureSpeech);
        if (Int32.TryParse(lsubstrings[0], out loutValue))
        {
            loutValue = loutValue - 274;
            lFinalSentence = RandomString(mTempSpeech) + " " + loutValue.ToString() + " " + RandomString(mDegreesCSpeech);
        }
        //BYOS.Instance.NotManager.Display<MeteoNot>(10F).With(loutValue, "",
        //                                                DateTime.Now.ToString(),
        //                                                string.IsNullOrEmpty(iData) ? "" : iData,
        //                                                "",
        //                                                lInfos);
        TTSProcessAndSay(lFinalSentence);
    }

    private IEnumerator BuildDefinitionAnswer(string iData)
    {
        string lXmlData = "";
        string lKeyword = "48.853,2.35";

        if (!string.IsNullOrEmpty(iData))
        {
            WWW lWww = new WWW("http://maps.googleapis.com/maps/api/geocode/xml?address=" + iData);

            float lElapsedTime = 0.0f;
            while (!lWww.isDone)
            {
                lElapsedTime += Time.deltaTime;

                if (lElapsedTime >= 5f)
                    break;

                yield return new WaitForSeconds(0.5f);
            }

            if (lWww.isDone && string.IsNullOrEmpty(lWww.error))
            {
                string lXml = lWww.text;
                using (XmlReader lReader = XmlReader.Create(new StringReader(lXml)))
                {
                    if (lReader.ReadToFollowing("location"))
                    {
                        lReader.ReadToFollowing("lat");
                        lReader.Read();
                        lKeyword = lReader.ReadContentAsString();
                        lReader.ReadToFollowing("lng");
                        lReader.Read();
                        lKeyword += "," + lReader.ReadContentAsString();
                    }
                    lReader.Close();
                }
            }
        }
        else if (Input.location.isEnabledByUser)
        {
            Input.location.Start();

            int lmaxWait = 7;
            while (Input.location.status == LocationServiceStatus.Initializing && lmaxWait > 0)
            {
                yield return new WaitForSeconds(1f);
                lmaxWait--;
                Debug.Log("Waiting for geoloc");
            }
            if (Input.location.status != LocationServiceStatus.Failed && lmaxWait > 0)
            {
                lKeyword = Input.location.lastData.latitude + "," + Input.location.lastData.longitude;
            }

            Input.location.Stop();
        }

        yield return StartCoroutine(MakeRequest(RequestType.WEATHER, lKeyword, null, value => lXmlData = value));

        //We take the answer from the weather website and extract the temperature as a string
        string lTemperature = "";
        WeatherInfo[] lInfos = new WeatherInfo[6];

        using (XmlReader lReader = XmlReader.Create(new StringReader(lXmlData)))
        {
            while (lReader.ReadToFollowing("echeance"))
            {
                lReader.MoveToAttribute("timestamp");
                string lReadTime = lReader.Value.Substring(0, lReader.Value.Length - 4);
                if (DateTime.Now.CompareTo(Convert.ToDateTime(lReadTime)) < 0)
                {
                    lReader.ReadToFollowing("level");
                    lTemperature = lReader.ReadElementContentAsString();
                    break;
                }
            }

            int lCount = 0;
            while (lCount < 6 && lReader.ReadToFollowing("echeance"))
            {
                lReader.MoveToAttribute("timestamp");
                string lReadTime = lReader.Value;
                string[] lReadTimeSplit = lReadTime.Split(' ');
                int lHour = 0;
                Int32.TryParse(lReadTimeSplit[1].Split(':')[0], out lHour);

                lReader.ReadToFollowing("level");
                string lTemp = lReader.ReadElementContentAsString();
                int lTempInt = 0;
                if (Int32.TryParse(lTemp.Split('.')[0], out lTempInt))
                    lTempInt -= 274;

                lReader.ReadToFollowing("pluie");
                int lRainLevel = 0;
                Int32.TryParse(lReader.ReadElementContentAsString(), out lRainLevel);

                lInfos[lCount] = new WeatherInfo
                {
                    Hour = lHour,
                    Temperature = lTempInt,
                    Type = lRainLevel > 3 ? WeatherType.RAIN : WeatherType.SUNNY
                };

                lCount++;
            }
            lReader.Close();
        }

        string[] lsubstrings = lTemperature.Split('.');
        int loutValue = 0;
        string lFinalSentence = RandomString(mICouldntSpeech) +
            " " + RandomString(mGetSpeech) +
            " " + RandomString(mTemperatureSpeech);
        if (Int32.TryParse(lsubstrings[0], out loutValue))
        {
            loutValue = loutValue - 274;
            lFinalSentence = RandomString(mTempSpeech) + " " + loutValue.ToString() + " " + RandomString(mDegreesCSpeech);
        }
        TTSProcessAndSay(lFinalSentence);
    }

    private IEnumerator MakeRequest(RequestType iType, string iKeyword, Dictionary<string, string> iHeader, Action<string> ioResult)
    {
        WWW lWww = new WWW(mWebsiteHash[iType] + Uri.EscapeUriString(iKeyword), null, iHeader);

        float lElapsedTime = 0.0f;
        while (!lWww.isDone)
        {
            lElapsedTime += Time.deltaTime;
            if (lElapsedTime >= 5f) break;
            yield return null;
        }
        if (!lWww.isDone || !string.IsNullOrEmpty(lWww.error))
        {
            Debug.Log("Request error: " + lWww.error);
            ioResult(null);
            yield break;
        }
        ioResult(lWww.text);
    }

    private string Suggest()
    {
        if (UnityEngine.Random.value > 0.4)
            return SuggestGame();
        else if (UnityEngine.Random.value > 0.2)
        {
            //TTSProcessAndSay("J'ai envie de te prendre en photo!", true);
            return "Photo";
            //link.animator.SetTrigger("Photo");
        }
        else
        {
            //TTSProcessAndSay("J'adore faire la star, prends moi en photo!", true);
            return "Pose";
            //link.animator.SetTrigger("Pose");
        }
    }

    private string SuggestGame()
    {
        if (UnityEngine.Random.value > 0.8)
        {
            //TTSProcessAndSay("J'ai envie de poser des questions! Allez, faisons un quizz!", true);
            return "Quizz";
            //link.animator.SetTrigger("Quizz");
        }
        else if (UnityEngine.Random.value > 0.6)
        {
            //TTSProcessAndSay("J'ai envie de jouer! Faisons le test de mémoire!", true);
            return "Memory";
            //link.animator.SetTrigger("Memory");
        }
        else if (UnityEngine.Random.value > 0.4)
        {
            //TTSProcessAndSay("J'ai envie de tester tes facultés cognitives, faisons des calculs!", true);
            return "Calcul";
            //link.animator.SetTrigger("Calcul");
        }
        else if (UnityEngine.Random.value > 0.2)
        {
            //TTSProcessAndSay("J'adore le jeu des couleurs, allez, faisons une partie!", true);
            return "Colors";
            //link.animator.SetTrigger("Colors");
        }
        else
        {
            //TTSProcessAndSay("Allez, faisons un jeu ensemble !", true);
            return "Games";
            //link.animator.SetTrigger("Games");
        }
    }

    private bool ContainsOneOf(string iSpeech, List<string> iListSpeech)
    {
        for (int i = 0; i < iListSpeech.Count; ++i)
        {
            string[] words = iListSpeech[i].Split(' ');
            if (words.Length < 2)
            {
                words = iSpeech.Split(' ');
                foreach (string word in words)
                {
                    if (word == iListSpeech[i].ToLower())
                    {
                        return true;
                    }
                }
            }
            else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                return true;
        }
        return false;
    }

    private int WordIndexOfOneOf(string iSpeech, List<string> iListSpeech)
    {
        for (int i = 0; i < iListSpeech.Count; ++i)
        {
            string[] lWords = iListSpeech[i].Split(' ');
            string lKeyword = lWords[lWords.Length - 1];
            string[] lSpeechWords = iSpeech.Split(' ');
            for (int j = 0; j < lSpeechWords.Length; j++)
            {
                if (lSpeechWords[j] == lKeyword)
                    return j;
            }
        }
        return -1;
    }

    private string RandomString(List<string> iListStr)
    {
        if (iListStr.Count == 0)
        {
            Debug.Log("the following list is empty!!! " + iListStr.ToString());
        }
        System.Random lRnd = new System.Random();
        return iListStr[lRnd.Next(0, iListStr.Count)];
    }
}