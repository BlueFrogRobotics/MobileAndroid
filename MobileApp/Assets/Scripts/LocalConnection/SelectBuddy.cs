﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Saves which Buddy has been selected from the list as well as the connection type
/// </summary>
public class SelectBuddy : MonoBehaviour
{
    public enum RemoteType { LOCAL, WEBRTC };

    public RemoteType Remote { get { return mRemote; } set { mRemote = value; } }

    [SerializeField]
    private Transform buddyList;

    [SerializeField]
    private OTONetwork oTONetwork;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private GameObject webRTC;

    [SerializeField]
    private Animator canvasAppAnimator;

    [SerializeField]
    private NotificationSender notifications;

    [SerializeField]
    private Text buddyConnectedName;

    [SerializeField]
    private Text buddyConnectedID;

    private RemoteType mRemote;

    public void BuddySelected()
    {
        bool lFound = false;

        //Look through the list to see which robot has been selected
        foreach (Transform mBuddy in buddyList) {
            Toggle lToggle = mBuddy.gameObject.GetComponent<Toggle>();

            
            //The robot whose Toggle button has been triggered is the one to connect to
            if (lToggle != null && lToggle.isOn) {
                string[] lBuddySplit = mBuddy.GetChild(5).GetComponent<Text>().text.Split(' ');
                string lBuddyID = lBuddySplit[1];
                string lBuddyName = mBuddy.GetChild(4).GetComponent<Text>().text;


                //Check wether it's a WebRTC or local connection
                if (lBuddyName.Contains("Buddy")) {                    
                    mRemote = RemoteType.LOCAL;
                    oTONetwork.IP = lBuddyID;
                    mobileServer.BuddyIP = lBuddyID;
                    Debug.Log("ip " + lBuddyID + "!");
                } else {
                    mRemote = RemoteType.WEBRTC;
                    webRTC.GetComponent<Webrtc>().RemoteID = lBuddyID;
                    webRTC.SetActive(true);
                }
                
                lFound = true;
                notifications.SendNotification("Connected to Buddy " + lBuddyID);
                ChangeConnectedBuddyName(lBuddyName, lBuddyID);
            }            
        }

        if(lFound) {
            canvasAppAnimator.SetTrigger("GoConnectBuddy");
            canvasAppAnimator.SetTrigger("EndScene");
        }

    }

    private void ChangeConnectedBuddyName(string iName, string iID)
    {
        buddyConnectedName.text = iName;
        buddyConnectedID.text = iID;
    }
}
