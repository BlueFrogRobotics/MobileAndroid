using UnityEngine;
using System.Collections.Generic;
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

    [SerializeField]
    private DBManager dbManager;

    private AndroidJavaObject mJavaListener;

    // Use this for initialization
    void Start()
    {
        StartBackgroundListener(uri, realm);
    }

    private void StartBackgroundListener(string iUri, string iRealm)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

            mJavaListener = new AndroidJavaObject("my.maylab.unitywebrtc.BackgroundListener", iUri, iRealm,
                BuddyTools.Utils.GetStreamingAssetFilePath("client_cert.pem"),
                BuddyTools.Utils.GetStreamingAssetFilePath("server_key.pem"),
                jo);
        }
    }

    public void SubscribeConnectRequest()
    {
        mJavaListener.Call("SubscribeConnectRequest");
    }

    public void SubscribeChatChannel()
    {
        mJavaListener.Call("SubscribeChat", SelectBuddy.BuddyID);
    }

    public void SubscribeNotificationChannels(List<Buddy> iBuddies)
    {
        string lBuddyIDs = "";
        for (int i = 0; i < iBuddies.Count; i++)
        {
            lBuddyIDs += iBuddies[i].ID + "/";
        }
        mJavaListener.Call("StartNotificationService", lBuddyIDs);
    }

    public void UnsubscribeNotifications()
    {
        mJavaListener.Call("StopNotificationService");
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
        string[] lSplit = iMessage.Split('/');

        string lBuddyID = lSplit[0];
        Buddy lBuddy = new Buddy() { name = "NAN", ID = "00-00-00-00" };
        foreach (Buddy lFindingBuddy in dbManager.BuddiesList) {
            if (lFindingBuddy.ID == lBuddyID) {
                lBuddy = lFindingBuddy;
                break;
            }
        }

        string[] lMessageContent = lSplit[1].Split('@');
        string lTitle = lBuddy.name + " : " + lMessageContent[0];
        string lMessage = lMessageContent[1];

        notification.SendNotification(lMessage, lTitle);
    }

    public void OnAndroidLog(string iLogText)
    {
        Debug.Log(iLogText);
    }
}
