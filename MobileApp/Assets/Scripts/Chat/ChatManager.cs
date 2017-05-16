using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using BuddyAPI;

[Serializable]
public class UserMessages
{
    public BuddyMessages[] Contact;
}

[Serializable]
public class BuddyMessages
{
    public string Buddy;
    public ChatMessage[] Messages;
}

[Serializable]
public class ChatMessage
{
    public string Content;
    public string Color;
    public string Date;
}

/// <summary>
/// Class to manage the chat window
/// </summary>
public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private PoolManager poolManager;

    [SerializeField]
    private GameObject scrollParent;

    [SerializeField]
    private SelectBuddy selectBuddy;

    [SerializeField]
    private BackgroundListener backgroundListener;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private InputField chatInput;

    [SerializeField]
    private TextToSpeech mTTS;

    [SerializeField]
    private RegularFace mFace;

    private string mCurrentFilePath;
    private string mCurrentBuddy;
    private List<ChatMessage> mMessages;
    private BuddyMessages mBuddyMessages;
    private UserMessages mHistory;

    
    void Start()
    {
        mMessages = new List<ChatMessage>();
    }

    //Called when user has entered a new message
    public void NewChatMessage(string iMsg)
    {
        if (string.IsNullOrEmpty(iMsg))
            return;

        ChatMessage lMessage = new ChatMessage() { Content = iMsg, Color = "White", Date = DateTime.Now.ToString("g") };
        mMessages.Add(lMessage);

        poolManager.fBubble(lMessage.Color, lMessage.Content, lMessage.Date);

        //Scroll the chat list to the bottom where the new message has appeared
        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;

        GameObject.Find("Bottom_UI").GetComponentInChildren<InputField>().text = "";

        //mTTS.Say(lMsg);
        if (selectBuddy.Remote == SelectBuddy.RemoteType.LOCAL)
            mobileServer.SendChatMessage(iMsg);
        else if (selectBuddy.Remote == SelectBuddy.RemoteType.WEBRTC)
            backgroundListener.SendChatMessage(iMsg);


    }

    public void NewBuddyMessage(string iMsg)
    {
        ChatMessage lMessage = new ChatMessage() { Content = iMsg, Color = "Blue", Date = DateTime.Now.ToString("g") };
        mMessages.Add(lMessage);

        poolManager.fBubble(lMessage.Color, lMessage.Content, lMessage.Date);

        //Scroll the chat list to the bottom where the new message has appeared
        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;
    }

    public void LoadMessageHistory()
    {
        //Pas opti, on risque de recharger tous les messages de tous les Buddy pour un user, si on change pas de user entre temps.

        //mBuddyMessages = new BuddyMessages();
        mCurrentBuddy = SelectBuddy.BuddyName;
        //Load all the messages to all Buddy from a specific user
        mHistory = new UserMessages()
        {
            Contact = new List<BuddyMessages>().ToArray()
        };

        DBManager lDB= GameObject.Find("DBManager").GetComponent<DBManager>();
        mCurrentFilePath = Application.persistentDataPath + "/" + lDB.CurrentUser.Email + "Msg.txt";

        if (File.Exists(mCurrentFilePath)) {
            StreamReader lstreamReader = new StreamReader(mCurrentFilePath);
            string lTemp = lstreamReader.ReadToEnd();
            lstreamReader.Close();

            mHistory = JsonUtility.FromJson<UserMessages>(lTemp);
            List<BuddyMessages> lMessages = new List<BuddyMessages>(mHistory.Contact);

            bool lFound = false;

            //Look for the Buddy that was selected
            foreach (BuddyMessages BuddyMessage in lMessages) {
                if (BuddyMessage.Buddy == SelectBuddy.BuddyName) {
                    mBuddyMessages = BuddyMessage;
                    mMessages = new List<ChatMessage>(mBuddyMessages.Messages);
                    lFound = true;
                    break;
                }
            }

            //If we didn't find it, then we start a new conversation history
            if (!lFound) {
                mMessages = new List<ChatMessage>();
                mBuddyMessages = new BuddyMessages() { Buddy = SelectBuddy.BuddyName, Messages = mMessages.ToArray() };
            } else {
                //else, we just recreate the message history
                foreach (ChatMessage lMessage in mBuddyMessages.Messages) {
                    poolManager.fBubble(lMessage.Color, lMessage.Content, lMessage.Date);
                }
            }
        }        
    }

    public void SaveMessageHistory()
    {
        //We have the chat history, so we have to go through the whole list to get the current Buddy
        Debug.Log("Saving history");
        bool lFound = false;
        if(!File.Exists(mCurrentFilePath)) {
            List<BuddyMessages> lTempMEssages = new List<BuddyMessages>();
            lTempMEssages.Add(new BuddyMessages()
            {
                Buddy = mCurrentBuddy,
                Messages = mMessages.ToArray()
            });
            mHistory.Contact = lTempMEssages.ToArray();
        } else {
            foreach (BuddyMessages lContact in mHistory.Contact) {
                if (lContact.Buddy == mCurrentBuddy) {
                    lContact.Messages = mMessages.ToArray();
                    lFound = true;
                }
            }

            //If the Buddy we are talking to doesn't exist in the history, we add it
            if (!lFound) {
                List<BuddyMessages> lTempMEssages = new List<BuddyMessages>(mHistory.Contact);
                lTempMEssages.Add(new BuddyMessages() {
                    Buddy = mCurrentBuddy,
                    Messages = mMessages.ToArray()
                });
                mHistory.Contact = lTempMEssages.ToArray();
            }
        }        

        string lJSON = JsonUtility.ToJson(mHistory, true);
        StreamWriter lStrWriter = new StreamWriter(mCurrentFilePath);
        lStrWriter.Write(lJSON);
        lStrWriter.Close();

        mMessages = new List<ChatMessage>();

        Transform lContent =  GameObject.Find("Message_UI/ScrollView/Viewport/Content").transform;
        if (lContent.childCount == 0)
            return;

        foreach (Transform lChild in lContent)
            GameObject.Destroy(lChild.gameObject);
    }
}
