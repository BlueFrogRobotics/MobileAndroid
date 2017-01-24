﻿using UnityEngine;

public class LaunchTelepresence : MonoBehaviour
{
    [SerializeField]
    private SelectBuddy selectBuddy;

    [SerializeField]
    private GameObject remoteControlRTC;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private OTONetwork oTONetwork;

    //[SerializeField]
    //private GameObject telepresenceGUI;
    
    void Start()
    {

    }

    public void ConnectToBuddy()
    {
        Debug.Log("Remote type is " + selectBuddy.Remote);
        if (selectBuddy.Remote == SelectBuddy.RemoteType.LOCAL) {
            mobileServer.OnConnectionEstablished += StartTelepresence;
            mobileServer.StartTelepresence();
            Debug.Log("Starting local control");
        } else if (selectBuddy.Remote == SelectBuddy.RemoteType.WEBRTC) {
            remoteControlRTC.SetActive(true);
            webRTC.Call();
            Debug.Log("Starting distant control");
        }
    }

    private void StartTelepresence()
    {
        mobileServer.gameObject.SetActive(false);
        oTONetwork.gameObject.SetActive(true);
        //telepresenceGUI.SetActive(true);
    }
}
