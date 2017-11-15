using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class FirstConnectionState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
			GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "ScrollView", "TopUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "CREATE YOUR ACCOUNT", "AddUser", new List<UnityAction>() { lMenuManager.GoCreationMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "CONNECT YOUR ACCOUNT", "ConnectUser", new List<UnityAction>() { lMenuManager.GoConnectionMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Title("Content_Top/Top_UI", "FIRST CONNECTION"));
        }
    }
}
