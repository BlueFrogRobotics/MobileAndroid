using UnityEngine;
using System.Collections;

public class LoadingBuddyState : ASubState {

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
            LoadingUI.AddObject(lPoolManager.fLoading("Content_Top", "Loading..."));
            LoadingUI.AddObject(lPoolManager.fLogo("Content_Top"));
        }
    }
}
