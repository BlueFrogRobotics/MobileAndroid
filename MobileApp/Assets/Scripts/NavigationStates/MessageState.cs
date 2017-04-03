using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MessageState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "MessageUI", "BottomUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoConnectedMenu }));
            LoadingUI.AddObject(lPoolManager.fTextField_Icon("Content_Bottom/Bottom_UI", "ASK SOMETHING...", "", "", null, null, null));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Bottom/Bottom_UI", "Ico_Join", null));
            LoadingUI.AddObject(lPoolManager.fBubble("Blue", "Toto par en vadrouille", "Jun, Mon at 9:45am"));
            LoadingUI.AddObject(lPoolManager.fBubble("White", "Ok ça roule ! bon voyage ;)", "Jun, Mon at 9:47am"));
        }
    }
}

