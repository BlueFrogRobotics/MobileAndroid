using UnityEngine;
using UnityEngine.Events;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State where the user can remotely control the Buddy.
/// </summary>
public class RemoteControlState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        if (indexState == (int)State.OPEN)
		{
			BackToMenu.prevMenuActivated = false;
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            // Activate predefined generic elements.
            // No need to generate anything else, the Remote screen is one of the few that has already everything.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "BottomUI", "RemoteUI", "ControlUI" });
        }
    }
}
