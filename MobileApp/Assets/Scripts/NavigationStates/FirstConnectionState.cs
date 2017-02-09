using UnityEngine;
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
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "ScrollView", "TopUI" });
            // CREATING OBJECTS
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Big("Content_Bottom/ScrollView/Viewport", "CREATE YOUR ACCOUNT", "AddUser", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Big("Content_Bottom/ScrollView/Viewport", "CONNECT YOUR ACCOUNT", "ConnectUser", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Title("Content_Top/Top_UI", "FIRST CONNECTION"));
        }
    }
}
