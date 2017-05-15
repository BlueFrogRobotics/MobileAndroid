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

    private string mCurrentBuddy;
    private List<ChatMessage> mMessages;
    private BuddyMessages mBuddyMessages;
    private UserMessages mHistory;

    
    void Start()
    {
        mMessages = new List<ChatMessage>();
    }
    
    void Update()
    {

    }

    //Called when user has entered a new message
    public void NewChatMessage(string iMsg)
    {
        //Debug.Log("New chat message");
        //Get message
        //string lMsg = chatInput.text;
        //chatInput.text = "";
        //Assign it to the prefab and set a timestamp
        //Text[] lMsgText = bubbleWhite.GetComponentsInChildren<Text>();
        //lMsgText[0].text = lMsg;
        //lMsgText[1].text = DateTime.Now.ToString("g");

        ////Instiante the white bubble corresponding to a user message in the chat list
        //GameObject lClone = Instantiate(bubbleWhite, transform.position, transform.rotation) as GameObject;
        //lClone.transform.SetParent(scrollParent.GetComponent<Transform>());
        //lClone.transform.localScale = Vector3.one;
        //lClone.SetActive(true);
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
        //Assign Buddy's answer to the prefab and set a timestamp
        //Text[] lMsgText = bubbleBlue.GetComponentsInChildren<Text>();
        //lMsgText[0].text = iMsg;
        //lMsgText[1].text = DateTime.Now.ToString("g");

        ////Instantiate the blue bubble corresponding to a Buddy message
        //GameObject lClone = Instantiate(bubbleBlue, transform.position, transform.rotation) as GameObject;
        //lClone.transform.SetParent(scrollParent.GetComponent<Transform>());
        //lClone.transform.localScale = Vector3.one;
        //lClone.SetActive(true);

        ChatMessage lMessage = new ChatMessage() { Content = iMsg, Color = "Blue", Date = DateTime.Now.ToString("g") };
        mMessages.Add(lMessage);

        poolManager.fBubble(lMessage.Color, lMessage.Content, lMessage.Date);

        //Scroll the chat list to the bottom where the new message has appeared
        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;
    }

    public void LoadMessageHistory(string iBuddy)
    {
        //Pas opti, on risque de recharger tous les messages de tous les Buddy pour un user, si on change pas de user entre temps.

        //mBuddyMessages = new BuddyMessages();
        mCurrentBuddy = SelectBuddy.BuddyName;
        //Load all the messages to all Buddy from a specific user
        mHistory = new UserMessages();

        StreamReader lstreamReader = new StreamReader(BuddyTools.Utils.GetStreamingAssetFilePath("currentUserMessages.txt"));
        string lTemp = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        mHistory = JsonUtility.FromJson<UserMessages>(lTemp);
        List<BuddyMessages> lMessages = new List<BuddyMessages>(mHistory.Contact);

        bool lFound = false;

        //Look for the Buddy that was selected
        foreach(BuddyMessages BuddyMessage in lMessages) {
            if(BuddyMessage.Buddy == SelectBuddy.BuddyName) {
                mBuddyMessages = BuddyMessage;
                lFound = true;
                break;
            }
        }

        //If we didn't find it, then we start a new conversation history
        if(!lFound) {
            mMessages = new List<ChatMessage>();
            mBuddyMessages = new BuddyMessages() { Buddy = SelectBuddy.BuddyName, Messages = mMessages.ToArray() };
        } else {
            //else, we just recreate the message history
            foreach(ChatMessage lMessage in mBuddyMessages.Messages) {
                poolManager.fBubble(lMessage.Color, lMessage.Content, lMessage.Date);
            }
        }
    }

    public void SaveMessageHistory()
    {
        //We have the chat history, so we have to go through the whole list to get the current Buddy
        bool lFound = false;
        foreach(BuddyMessages lMessages in mHistory.Contact)
        {
            if(lMessages.Buddy == mCurrentBuddy)
            {
                lMessages.Messages = mMessages.ToArray();
                lFound = true;
            }
        }

        //If the Buddy we are talking to doesn't exist in the history, we add it
        if(!lFound)
        {
            List<BuddyMessages> lTempMEssages = new List<BuddyMessages>(mHistory.Contact);
            lTempMEssages.Add(new BuddyMessages()
            {
                Buddy = mCurrentBuddy,
                Messages = mMessages.ToArray()
            });
            mHistory.Contact = lTempMEssages.ToArray();
        }

        string lJSON = JsonUtility.ToJson(mHistory, true);
        StreamWriter lStrWriter = new StreamWriter(BuddyTools.Utils.GetStreamingAssetFilePath("currentUserMessages.txt"));
        lStrWriter.Write(lJSON);
        lStrWriter.Close();
    }
}
