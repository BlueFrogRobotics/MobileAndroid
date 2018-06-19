using UnityEngine;
using System.Collections;

public class TermsOfUsesState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "BottomUI", "ScrollView" });
            // Creating UI Objects
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/Bottom_UI", "", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Bottom/Bottom_UI", "Up", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTerms("Content_Bottom/ScrollView/Viewport", ""));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "agreetermsofservice", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Title("Content_Top/Top_UI", "termsofservice"));
        }
    }
}
