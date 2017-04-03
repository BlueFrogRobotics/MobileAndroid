using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ConnectBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "NavigationDisplay", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // BOTTOM UI
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoSelectBuddyMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/Bottom_UI", "FULL ACCESS", false ));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "toto", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "CHAT WITH BUDDY", "", new List<UnityAction>() { lMenuManager.GoChatMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "REMOTE CONTROL", "", new List<UnityAction>() { StartRemoteControl }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "BUDDY SETTINGS", "", new List<UnityAction>() { lMenuManager.GoBuddySettings }));
            // TOP UI
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", null));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Edit", new List<UnityAction>() { lMenuManager.GoEditBuddyMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));
            GameObject.Find("Navigation_Account/Text_LastName").GetComponent<Text>().text = SelectBuddy.BuddyName;
            GameObject.Find("Navigation_Account/TextFirstName").GetComponent<Text>().text = SelectBuddy.BuddyID;
            //LoadingUI.AddObject(lPoolManager.fButton_QrCode("Content_Top/Navigation_Account", null));
        }
    }

    private void StartRemoteControl()
    {
        GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
        lMenuManager.GoRemoteControlMenu();
        LaunchTelepresence lLauncher = GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>();
        lLauncher.ConnectToBuddy();
    }
}
