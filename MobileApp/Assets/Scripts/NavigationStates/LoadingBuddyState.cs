using UnityEngine;
using System;
using System.Collections;

public class LoadingBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(null);
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", GoBack.LoadingBuddyMessage));
            LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));

            switch(animator.GetInteger("MenuBuddy"))
            {
                case 1: //Chat menu selected
                    GameObject.Find("Chat").GetComponent<ChatManager>().LoadMessageHistory();
                    BackgroundListener lListener = GameObject.Find("BackgroundListener").GetComponent<BackgroundListener>();
                    lListener.SubscribeChatChannel();
                    break;
                case 2 : //Distant control selected
                    GameObject.Find ("UnityWebrtc").GetComponent<Webrtc>().InitWebRTC();
                    GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>().ConnectToBuddy();
                    break;
            }

        }

        else if(indexState == (int)State.IDLE)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            switch(animator.GetInteger("MenuBuddy"))
            {
                case 1: //Chat menu selected
                    lMenuManager.GoChatMenu();
                    break;
                case 2: //Distant control selected
                    SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
                    lMenuManager.WaitForCallConfirmation(lSelect.Remote);
                    break;
            }
        }
    }
}
