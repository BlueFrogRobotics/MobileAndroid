using UnityEngine;
using System.Collections;
using System.IO;

public class BackgroundListener : MonoBehaviour
{
    [SerializeField]
    private string uri;

    [SerializeField]
    private string realm;

    [SerializeField]
    private NotificationSender notification;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private ChatManager chatManager;

    private AndroidJavaObject mJavaListener;

    // Use this for initialization
    void Start()
    {
        StartBackgroundListener(uri, realm);
    }

    private void StartBackgroundListener(string iUri, string iRealm)
    {
        mJavaListener = new AndroidJavaObject("my.maylab.unitywebrtc.BackgroundListener", iUri, iRealm,
            BuddyTools.Utils.GetStreamingAssetFilePath("client_cert.pem"),
            BuddyTools.Utils.GetStreamingAssetFilePath("server_key.pem"));
    }

    public void SubscribeConnectRequest()
    {
        mJavaListener.Call("SubscribeConnectRequest");
    }

    public void SubscribeChatChannel()
    {
        Debug.Log("Connecting to chan Chat" + SelectBuddy.BuddyID);
        mJavaListener.Call("SubscribeChat", SelectBuddy.BuddyID);
    }

    public void SendChatMessage(string iMessage)
    {
        mJavaListener.Call("SendChatMessage", iMessage);
    }

    public void PublishConnectionRequest(string iRemoteID)
    {
        mJavaListener.Call("Publish", "ConnectRequest", webRTC.ID + "/" + iRemoteID);
    }

    public void PublishNotification()
    {
        mJavaListener.Call("Publish", "NotificationChannel", "R2D2/A New Hope@Please Obi Wan Kenobi, you're my only hope!");
    }

    public void OnConnectionRequest(string iMessage)
    {
        Debug.Log("Received message : " + iMessage);
    }

    public void OnMessageReceived(string iMessage)
    {
        chatManager.NewBuddyMessage(iMessage);
    }

    public void OnNotificationMessage(string iMessage)
    {
        //Format of the notification is the following BuddyID / Title @ Message
        Debug.Log("Received notification : " + iMessage);
        string[] lSplit = iMessage.Split('/');
        string lBuddyID = lSplit[0];
        string[] lMessageContent = lSplit[1].Split('@');
        string lTitle = lMessageContent[0] + "@Buddy " + lBuddyID;
        string lMessage = lMessageContent[1];

        notification.SendNotification(lMessage, lTitle);
    }

    public void OnAndroidLog(string iLogText)
    {
        Debug.Log(iLogText);
    }
}
