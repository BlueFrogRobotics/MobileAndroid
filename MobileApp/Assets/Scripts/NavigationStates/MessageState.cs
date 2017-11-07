﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MessageState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            ChatManager chatManager = GameObject.Find("Chat").GetComponent<ChatManager>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "MessageUI", "BottomUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { chatManager.SaveMessageHistory, lMenuManager.GoConnectedMenu }));

            GameObject lInputField = lPoolManager.fTextField_Icon("Content_Bottom/Bottom_UI", "ASK SOMETHING...", "", "Message", null, new List<UnityAction<string>>() { chatManager.NewChatMessage }, null);
            lInputField.GetComponent<InputField>().characterLimit = 0;
            LoadingUI.AddObject(lInputField);

            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Bottom/Bottom_UI", "Ico_Join", null));
        }
    }
}

