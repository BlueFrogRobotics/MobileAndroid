using UnityEngine;
using UnityEngine.UI;
using System;
using BuddyAPI;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject bubbleBlue;

    [SerializeField]
    private GameObject bubbleWhite;

    [SerializeField]
    private GameObject scrollParent;

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

    public void NewChatMessage()
    {
        Debug.Log("New chat message");
        string lMsg = chatInput.text;
        chatInput.text = "";
        Text[] lMsgText = bubbleWhite.GetComponentsInChildren<Text>();
        lMsgText[0].text = lMsg;
        lMsgText[1].text = DateTime.Now.ToString("g");

        GameObject lClone = Instantiate(bubbleWhite, transform.position, transform.rotation) as GameObject;
        lClone.transform.SetParent(scrollParent.GetComponent<Transform>());
        lClone.transform.localScale = Vector3.one;
        lClone.SetActive(true);

        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;

        //mTTS.Say(lMsg);

        mobileServer.SendChatMessage(lMsg);
    }

    public void NewBuddyMessage(string iMsg)
    {
        Text[] lMsgText = bubbleBlue.GetComponentsInChildren<Text>();
        lMsgText[0].text = iMsg;
        lMsgText[1].text = DateTime.Now.ToString("g");

        GameObject lClone = Instantiate(bubbleBlue, transform.position, transform.rotation) as GameObject;
        lClone.transform.SetParent(scrollParent.GetComponent<Transform>());
        lClone.transform.localScale = Vector3.one;
        lClone.SetActive(true);

        scrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0F;
    }
}
