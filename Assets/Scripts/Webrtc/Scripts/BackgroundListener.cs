using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

using Buddy;

public class BackgroundListener : MonoBehaviour
{
    [SerializeField]
    private string uri;

    [SerializeField]
    private string realm;

    [SerializeField]
    private NotificationSender notification;

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
                ResourceManager.StreamingAssetFilePath("client_cert.pem"),
                ResourceManager.StreamingAssetFilePath("server_key.pem"),
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

	public void SubscribeStatusChannels(List<BuddyDB> iBuddies)
	{
		string lBuddyIDs = "";
		for(int i = 0; i < iBuddies.Count; i++)
		{
			lBuddyIDs += iBuddies[i].ID + "/";
		}
        Debug.Log("lBuddyIDs = " + lBuddyIDs);
		mJavaListener.Call("SubscribeStatus", lBuddyIDs);
    }

    public void SubscribeNotificationChannels(List<BuddyDB> iBuddies)
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

    public void PublishConnectionRequest(string iWebrtcID, string iRemoteID)
    {
        mJavaListener.Call("Publish", "ConnectRequest", iWebrtcID + "/" + iRemoteID);
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
        BuddyDB lBuddy = new BuddyDB() { name = "NAN", ID = "00-00-00-00" };
        foreach (BuddyDB lFindingBuddy in dbManager.BuddiesList) {
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

	public void OnStatusMessage(string iMessage)
	{
        //Status format is buddyID | appName
		string[] lSplit = iMessage.Split('|');
		string lBuddyID = lSplit[0];
		Debug.Log ("OnStatusMessage received " + lBuddyID + " " + lSplit [1]);
		foreach (BuddyDB lFindingBuddy in dbManager.BuddiesList) {
			if (lFindingBuddy.ID.Equals(lBuddyID)) {
				if (lSplit [1] != "") {
					Debug.Log ("BUDDY BUSY");
					lFindingBuddy.status = "busy";
					lFindingBuddy.appName = lSplit [1];
				} else {
					Debug.Log ("BUDDY PAS BUSY");
					lFindingBuddy.status = "online";
					lFindingBuddy.appName = lSplit [1];
				}

				lFindingBuddy.timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
				break;
			}
		}
	}

    public void OnAndroidLog(string iLogText)
    {
        Debug.Log(iLogText);
    }
}
