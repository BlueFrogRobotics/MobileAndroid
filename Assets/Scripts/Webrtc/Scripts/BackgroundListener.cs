using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

using Buddy;

/// <summary>
/// WebRTC element that listens on Crossbar.IO channels for connection requests, chat messages and other types or messages.
/// The listener is connected to a URI corresponding to our server, in a special realm.
/// </summary>
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

    /// <summary>
    /// Initializer of the background listener.
    /// </summary>
    /// <param name="iUri">URI of the server to connect to.</param>
    /// <param name="iRealm">Name of the realm on the server.</param>
    private void StartBackgroundListener(string iUri, string iRealm)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

            mJavaListener = new AndroidJavaObject("my.maylab.unitywebrtc.BackgroundListener", iUri, iRealm,
                DBManager.GetStreamingAssetFullPath("client_cert.pem"),
                DBManager.GetStreamingAssetFullPath("server_key.pem"),
                jo);
        }
    }

    /// <summary>
    /// Subscribe to the channel for a connection request.
    /// </summary>
    public void SubscribeConnectRequest()
    {
        mJavaListener.Call("SubscribeConnectRequest");
    }

    /// <summary>
    /// Subscribe to the chat message channels.
    /// </summary>
    public void SubscribeChatChannel()
    {
        mJavaListener.Call("SubscribeChat", SelectBuddy.BuddyID);
    }

    /// <summary>
    /// Subscribe to the status channels of all Buddies linked to an account.
    /// </summary>
    /// <param name="iBuddies">List of all Buddies linked to a specific user account.</param>
	public void SubscribeStatusChannels(List<BuddyDB> iBuddies)
	{
		string lBuddyIDs = "";
		for(int i = 0; i < iBuddies.Count; i++) {
			lBuddyIDs += iBuddies[i].ID + "/";
		}
        Debug.Log("lBuddyIDs = " + lBuddyIDs);
		mJavaListener.Call("SubscribeStatus", lBuddyIDs);
    }

    /// <summary>
    /// Subscribe to the notification channels of all Buddies linked to an account.
    /// </summary>
    /// <param name="iBuddies">List of all Buddies linked to a specific user account.</param>
    public void SubscribeNotificationChannels(List<BuddyDB> iBuddies)
    {
        string lBuddyIDs = "";
        for (int i = 0; i < iBuddies.Count; i++) {
            lBuddyIDs += iBuddies[i].ID + "/";
        }
        mJavaListener.Call("StartNotificationService", lBuddyIDs);
    }

    /// <summary>
    /// Unsubscribe from the notification channels.
    /// </summary>
    public void UnsubscribeNotifications()
    {
        mJavaListener.Call("StopNotificationService");
    }

    /// <summary>
    /// Send a message on the chat channel.
    /// </summary>
    /// <param name="iMessage">The message to send on the chat.</param>
    public void SendChatMessage(string iMessage)
    {
        mJavaListener.Call("SendChatMessage", iMessage);
    }

    /// <summary>
    /// Publish a request for a remote control session on a Buddy.
    /// </summary>
    /// <param name="iWebrtcID">The device's ID on the WebRTC server.</param>
    /// <param name="iRemoteID">The target Buddy ID.</param>
    /// <param name="iMode">0: Standard remote control - 1: Wizard of Oz.</param>
    public void PublishConnectionRequest(string iWebrtcID, string iRemoteID, int iMode)
    {
        mJavaListener.Call("Publish", "ConnectRequest", iWebrtcID + "/" + iRemoteID + "/" + iMode);
    }

    /// <summary>
    /// Method called from Java when a message on the "Connection request" channel was received.
    /// </summary>
    /// <param name="iMessage">The message received.</param>
    public void OnConnectionRequest(string iMessage)
    {
        Debug.Log("Received message : " + iMessage);
    }

    /// <summary>
    /// Method called from Java when a message was received from the chat channel.
    /// </summary>
    /// <param name="iMessage">Message received from the remote Buddy.</param>
    public void OnMessageReceived(string iMessage)
    {
        // Send the message to the chat manager to handle the display.
        chatManager.NewBuddyMessage(iMessage);
    }

    /// <summary>
    /// Called from Java when a message was received on a notification channel.
    /// </summary>
    /// <param name="iMessage">The notification message.</param>
    public void OnNotificationMessage(string iMessage)
    {
        //Format of the notification is the following BuddyID / Title @ Message
        string[] lSplit = iMessage.Split('/');

        string lBuddyID = lSplit[0];
        // Find the Buddy that sent the message.
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

        // Display the notification.
        notification.SendNotification(lMessage, lTitle);
    }

    /// <summary>
    /// Called from Java when a message was received on a status channel.
    /// </summary>
    /// <param name="iMessage">Status message.</param>
	public void OnStatusMessage(string iMessage)
	{
        // Status format is buddyID | appName
		string[] lSplit = iMessage.Split('|');
		string lBuddyID = lSplit[0];
		Debug.Log ("OnStatusMessage received " + lBuddyID + " " + lSplit [1]);
        // Find the Buddy and update its status with the correct application name.
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

    /// <summary>
    /// Called from Java to log a text on Unity (can be seen on the Reporter).
    /// </summary>
    /// <param name="iLogText">The text to log on the Unity console.</param>
    public void OnAndroidLog(string iLogText)
    {
        Debug.Log(iLogText);
    }
}
