using UnityEngine;
using System.Collections;

public class TermsOfUsesState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/Bottom_UI", "", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Bottom/Bottom_UI", "Up", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTerms("Content_Bottom/ScrollView/Viewport", ""));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Title("Content_Top/Top_UI", "Terms of Service"));
        }
    }
}
