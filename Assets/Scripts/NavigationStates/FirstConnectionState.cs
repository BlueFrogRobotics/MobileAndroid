using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The first time the application is launched, the user is given two options : either create an account or connect to an existing one.
/// </summary>
public class FirstConnectionState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
			GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // Cleaning previously created objects.
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "ScrollView", "TopUI" });
            // Creating UI Objects
            // Bottom UI content.
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "createaccount", "AddUser", new List<UnityAction>() { lMenuManager.GoCreationMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Big("Content_Bottom/ScrollView/Viewport", "connectaccount", "ConnectUser", new List<UnityAction>() { lMenuManager.GoConnectionMenu }));
            // Top UI content.
            LoadingUI.AddObject(lPoolManager.fSimple_Title("Content_Top/Top_UI", "firstconnection"));
        }
    }
}
