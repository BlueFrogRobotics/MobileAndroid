using UnityEngine;
using System.Collections;

public class MessageState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "MessageUI", "BottomUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/Bottom_UI", "ASK SOMETHING...", "", "", null, null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Bottom/Bottom_UI", "Ico_Join", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBubble("Blue", "Toto par en vadrouille", "Jun, Mon at 9:45am"));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBubble("White", "Ok ça roule ! bon voyage ;)", "Jun, Mon at 9:47am"));
        }
    }
}

