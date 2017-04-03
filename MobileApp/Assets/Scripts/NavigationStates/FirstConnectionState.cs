using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class FirstConnectionState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "ScrollView", "TopUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "CREATE YOUR ACCOUNT", "AddUser", new List<UnityAction>() { GoToCreationMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "CONNECT YOUR ACCOUNT", "ConnectUser", new List<UnityAction>() { GoToConnectionMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Title("Content_Top/Top_UI", "FIRST CONNECTION"));
        }
    }

    private void GoToCreationMenu()
    {
        Animator lAnimator = GameObject.Find("CanvasApp").GetComponent<Animator>();
        lAnimator.SetTrigger("GoCreateAccount");
        lAnimator.SetTrigger("EndScene");
    }

    private void GoToConnectionMenu()
    {
        Animator lAnimator = GameObject.Find("CanvasApp").GetComponent<Animator>();
        lAnimator.SetTrigger("GoConnectAccount");
        lAnimator.SetTrigger("EndScene");
    }
}
