using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Intermediate state between the "Connected to Buddy" screen and the selected menu to load.
/// </summary>
public class LoadingBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        // When entering the loading menu:
        if (indexState == (int)State.OPEN)
        {
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(null);
            // Creating UI Objects
            LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", GoBack.LoadingBuddyMessage));
            LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));

            switch(animator.GetInteger("MenuBuddy")) {
                case 1: // Chat menu selected, load message history and subscribe to the remote chat channel.
                    GameObject.Find("Chat").GetComponent<ChatManager>().LoadMessageHistory();
                    BackgroundListener lListener = GameObject.Find("BackgroundListener").GetComponent<BackgroundListener>();
                    lListener.SubscribeChatChannel();
                    break;
                case 2 : // Distant control selected, initialize the WebRTC communication.
                    GameObject.Find ("UnityWebrtc").GetComponent<Webrtc>().InitWebRTC();
                    GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>().ConnectToBuddy(animator.GetInteger("RemoteMode"));
                    break;
                case 3 : // Wizard of Oz distant control selected, initialize the WebRTC communication.
                    GameObject.Find("UnityWebrtc").GetComponent<Webrtc>().InitWebRTC();
                    GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>().ConnectToBuddy(animator.GetInteger("RemoteMode"));
                    break;
                case 4: //  Automatic Distant control selected ( Korian's version - temporary - If really needed, have to use authorization account )
                    GameObject.Find("UnityWebrtc").GetComponent<Webrtc>().InitWebRTC();
                    GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>().ConnectToBuddy(animator.GetInteger("RemoteMode"));
                    break;
            }

        }

        // Called at the "update" time of the state machine.
        else if(indexState == (int)State.IDLE) {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            switch(animator.GetInteger("MenuBuddy"))
            {
                case 1: // Chat menu selected, everything is ready for the chat to start, simply go to the chat screen.
                    lMenuManager.GoChatMenu();
                    break;
                case 2: // Distant control selected, wait for the call to be accepted on Buddy.
                    SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
                    lMenuManager.WaitForCallConfirmation(lSelect.Remote, ConnectBuddyState.AvailableRemoteMode.REMOTE_CONTROL);
                    break;
                case 3: // Wizard of Oz distant control selected, wait for the call to be accepted on Buddy.
                    SelectBuddy lSelectWOZ = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
                    lMenuManager.WaitForCallConfirmation(lSelectWOZ.Remote, ConnectBuddyState.AvailableRemoteMode.WOZ); 
                    break;
                case 4: // Automatic distant control selected, wait for the call to be accepted on Buddy.
                    SelectBuddy lSelectTakeControl = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
                    lMenuManager.WaitForCallConfirmation(lSelectTakeControl.Remote, ConnectBuddyState.AvailableRemoteMode.TAKE_CONTROL);
                    break;
            }
        }
    }
}
