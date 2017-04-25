using UnityEngine;
using UnityEngine.UI;
using System;
using BuddyAPI;

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

    
    void Start()
    {

    }
    
    void Update()
    {

    }

    //Called when user has entered a new message
    public void NewChatMessage(string iMsg)
    {
        Debug.Log("New chat message");
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

        poolManager.fBubble("White", iMsg, DateTime.Now.ToString("g"));

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

        poolManager.fBubble("Blue", iMsg, DateTime.Now.ToString("g"));

        //Scroll the chat list to the bottom where the new message has appeared
        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;
    }
}
