using UnityEngine;
using System;
using System.Collections;

public class LoadingBuddyState : ASubState {

    private Animator mAnimator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(null);
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", GoBack.LoadingBuddyMessage));
            LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));
        }

        else if(indexState == 2)
        {
            int lMenuSelected = animator.GetInteger("MenuBuddy");
            //Chat menu selected
            if(lMenuSelected == 1) {
                animator.SetTrigger("EndScene");
            }
            //Distant control selected
            else if(lMenuSelected == 3) {
                mAnimator = animator;
                SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();
                GameObject.Find("MenuManager").GetComponent<GoBack>().WaitForCallConfirmation(lSelect.Remote);
            }
        }
    }
}
