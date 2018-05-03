using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseConnection : MonoBehaviour {

    //private AndroidJavaObject currentActivity;

    public void Start()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        //AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //currentActivity.Call("getToken");
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("avant token");
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        Debug.Log("Raw data message: " + e.Message.RawData);
    }
}
