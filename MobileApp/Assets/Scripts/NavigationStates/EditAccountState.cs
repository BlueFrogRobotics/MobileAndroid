using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EditAccountState: ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            PhoneUser lPhoneUser = GameObject.Find("DBManager").GetComponent<DBManager>().CurrentUser;
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationEdit", "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "ID:5487-BF68-ZD97", "QRCode", null));
            LoadingUI.AddObject(lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your New Password", "", "Lock", null, null, null));
            LoadingUI.AddObject(lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Re-enter Your New Password", "", "Lock", null, null, null));
            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));
            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false));

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "Confirm Changes", "", new List<UnityAction>() { SaveProfileChanges }));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, null));

            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "", null));

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", null));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Floppy", new List<UnityAction>() { SaveProfileChanges }));
            //NEED TO ADD NAVIGATION ACOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }

    private void SaveProfileChanges()
    {
        GameObject.Find("MenuManager").GetComponent<GoBack>().PreviousMenu();
    }
}
