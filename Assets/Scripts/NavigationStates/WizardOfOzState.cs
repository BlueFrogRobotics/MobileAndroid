﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class WizardOfOzState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        if (indexState == (int)State.OPEN)
		{
			BackToMenu.prevMenuActivated = false;
            //GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            //PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "BottomUI", "RemoteUI", "ControlUI", "WizardOfOzUI" });
            // CREATING OBJECTS
            //LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoConnectedMenu }));
            //LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", "Loading..."));
            //LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));
        }
    }

}
