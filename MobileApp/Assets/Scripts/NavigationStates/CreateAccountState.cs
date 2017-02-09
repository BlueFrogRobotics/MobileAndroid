using UnityEngine;
using System.Collections;

public class CreateAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "NavigationAccount", "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/Bottom_UI", "CREATE YOUR ACCOUNT", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "", false, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Email Address", "", "Email", null, null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Password", "", "Lock", null, null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Repeat Your Password", "", "Lock", null, null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle_Underline("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User_Big("Content_Top", "", null));
            //NEED TO ADD NAVIGATION ACOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }
}
