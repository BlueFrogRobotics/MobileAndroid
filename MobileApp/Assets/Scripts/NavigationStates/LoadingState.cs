using UnityEngine;
using System.Collections;

public class LoadingState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(null);
            // CREATING OBJECTS
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/ScrollView/Viewport", "Test", "Apps", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Big("Content_Bottom/ScrollView/Viewport", "CREATE AN ACCOUNT", "AddUser", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSimple_Text("Content_Bottom/Bottom_UI", false, ""));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Bottom/Bottom_UI", "VLeft", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBubble("Blue", "Toto par en vadrouille", "Jun, Mon at 9:45am"));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBubble("White", "Ok ça roule ! bon voyage ;)", "Jun, Mon at 9:47am"));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User_Big("Content_Top", "toto", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_QrCode("Content_Top/Navigation_Account", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "toto", null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Repeat Your Password", "", "Lock", null, null, null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle_Underline("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false, null));
            //LoadingUI.AddObject(animator.GetComponent<PoolManager>().fToggle("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fLoading("Content_Top", "Loading..."));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fLogo("Content_Top"));
        }
    }
}
