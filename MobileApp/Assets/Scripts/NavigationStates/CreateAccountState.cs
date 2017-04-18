using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANING PREVIOUSLY CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DISABLE, ENABLE GENERICS
			HandleGeneric lHandler = GameObject.Find("ScriptUI").GetComponent<HandleGeneric>();
			lHandler.SetEditInfos ("Enter Your First Name", "Enter Your Last Name");
			lHandler.DisableGeneric(new ArrayList() { "NavigationEdit", "TopUI", "BottomUI", "ScrollView" });
            // CREATE OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoToFirstMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "CREATE YOUR ACCOUNT", "", new List<UnityAction>() { CreateAccount, lMenuManager.GoConnectionMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", false, null));

            GameObject lEmailField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Email Address", "", "Email", null, null, null);
            lEmailField.name = "Create_Email_Input";
            LoadingUI.AddObject(lEmailField);
            GameObject lPasswordField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Password", "", "Lock", null, null, null);
            lPasswordField.name = "Create_PW_Input";
            LoadingUI.AddObject(lPasswordField);
            GameObject lPasswordConfField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Repeat Your Password", "", "Lock", null, null, null);
            lPasswordConfField.name = "Create_PWConf_Input";
            LoadingUI.AddObject(lPasswordConfField);

            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));
            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false));
            LoadingUI.AddObject(lPoolManager.fToggle_Underline("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false, null));
			LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "DefaultUser", null));
            //NEED TO ADD NAVIGATION ACCOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }

    private void CreateAccount()
    {
        GameObject.Find("DBManager").GetComponent<DBManager>().StartCreateAccount();
    }
}
