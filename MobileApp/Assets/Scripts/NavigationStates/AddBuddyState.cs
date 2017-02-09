using UnityEngine;
using System.Collections;

public class AddBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "TopUI", "NavigationAccount", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // BOTTOM UI
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/Bottom_UI", "ADD THIS CONTACT", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "toto", true, null));

            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/ScrollView/Viewport", "ENTER BUDDY'S ID", "", "QRCode", null, null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/ScrollView/Viewport", "OR", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "REMOTE CONTROL", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "BUDDY SETTINGS", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "Connect By Default", false));
            // TOP UI
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User_Big("Content_Top", "toto", null));
        }
    }
}
