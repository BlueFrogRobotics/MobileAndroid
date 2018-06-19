using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Administration of Buddy state. Supposed to display the same content as the explorer on Buddy.
/// </summary>
public class MenuAdminState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "BottomUI" });
            // Creating UI Objects
            // Display information to change Buddy settings. For now, it's a mock menu.
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoConnectedMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/Bottom_UI", "buddysettings", false));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/Bottom_UI", "indev", false));
        }
    }
}
