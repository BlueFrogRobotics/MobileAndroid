using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Remote control in "Wizard of Oz" mode.
/// </summary>
public class WizardOfOzState : ASubState {

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
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "BottomUI", "RemoteUI", "ControlUI", "WizardOfOzUI" });
        }
    }

}
