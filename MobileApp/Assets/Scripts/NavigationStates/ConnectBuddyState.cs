using UnityEngine;
using System.Collections;

public class ConnectBuddyState : ASubState {

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
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/Bottom_UI", "FULL ACCESS", false ));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "toto", true, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "CHAT WITH BUDDY", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "REMOTE CONTROL", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "BUDDY SETTINGS", "", null));
            // TOP UI
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Top/Top_UI", "Trash", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Top/Top_UI", "", false ));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Top/Top_UI", "Edit", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User_Big("Content_Top", "toto", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_QrCode("Content_Top/Navigation_Account", null));
            }
    }
}
