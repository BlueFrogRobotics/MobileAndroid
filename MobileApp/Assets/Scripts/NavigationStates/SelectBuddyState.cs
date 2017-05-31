using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SelectBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        if (indexState == 1)
        {
            BuddyIPList lIPList = GameObject.Find("Content_Bottom/ScrollView/Viewport").GetComponent<BuddyIPList>();
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            SelectBuddy lSelect = lMenuManager.GetComponentInChildren<SelectBuddy>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // DISPLAY ALL BUDDY's CONTACTS
            //LoadingUI.AddObject(lPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "YOUR BUDDY CONTACT(S)"));
            //LoadingUI.AddObject(lPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "My BUDDY NAME", "ID:5458-FR74-DG59", "", true, true, null));
            //LoadingUI.AddObject(lPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "My BUDDY NAME2", "ID:5458-FR32-D589", "", false, true, null));
            ////DISPLAY BUDDY's NOT YET ADDED ON A SEARCH
            //LoadingUI.AddObject(lPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "NOT ADDED & AVAILABLE IN LOCAL"));
            //LoadingUI.AddObject(lPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "BUDDY NOT ADDED", "ID:5458-FR99-ZX59", "Sprites/Ico_App", true, false, null));
            //LoadingUI.AddObject(lPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "BUDDY NOT ADDED2", "ID:5458-FR54-JF59", "", true, false, null));
            //LoadingUI.AddObject(lPoolManager.fSearching("Content_Bottom/ScrollView/Viewport"));
            // OTHER UI OBJECT
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { BackToConnectionMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "SELECT", "", new List<UnityAction>() { lSelect.BuddySelected }));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "+", new List<UnityAction>() { lMenuManager.GoAddBuddyMenu }));
            LoadingUI.AddObject(lPoolManager.fTextField_Searching("Content_Top/Top_UI", "Search...", "", null, new List<UnityAction<string>>() { lIPList.SearchForBuddy }));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Undo", new List<UnityAction>() { lIPList.CreateListDisplay }));
            //NEED TO ADD NAVIGATION ACOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
            GameObject.Find("Viewport").GetComponent<BuddyIPList>().enabled = true;
            lIPList.enabled = true;
            lIPList.InSelectBuddy = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(indexState == 2) {
            BuddyIPList lIPList = GameObject.Find("Viewport").GetComponent<BuddyIPList>();
            lIPList.enabled = false;
            lIPList.InSelectBuddy = false;
        }

        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    private void BackToConnectionMenu()
    {
        GameObject.Find("DBManager").GetComponent<DBManager>().GenerateUserDisplay();
        GameObject.Find("MenuManager").GetComponent<GoBack>().GoConnectionMenu();
    }
}
