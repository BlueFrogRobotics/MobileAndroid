using UnityEngine;
using System.Collections.Generic;

public class LoadingState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            LoadingUI.ClearUI();
            Debug.Log("Toto");
            //animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "Test", "Apps", null);
            //animator.GetComponent<PoolManager>().fButton_Big("Content_Bottom/ScrollView/Viewport", "CREATE AN ACCOUNT", "AddUser", null);
            //animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null);
            //animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/Bottom_UI", "");
            //animator.GetComponent<PoolManager>().fButton_R("Content_Bottom/Bottom_UI", "VLeft", null);
            //animator.GetComponent<PoolManager>().fBubble("Blue", "Toto par en vadrouille", "Jun, Mon at 9:45am");
            //animator.GetComponent<PoolManager>().fBubble("White", "Ok ça roule ! bon voyage ;)", "Jun, Mon at 9:47am");
            //animator.GetComponent<PoolManager>().fButton_User_Big("Content_Top", "toto", null);
            //animator.GetComponent<PoolManager>().fButton_QrCode("Content_Top", null);
            //animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "toto", null);
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fLoading("Content_Top", "Loading..."));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fLogo("Content_Top"));
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if (indexState == 3)
        {

        }
    } 
}
