using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Buddy selection menu state.
/// </summary>
public class SelectBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        if (indexState == (int)State.OPEN)
        {
            BuddyIPList lIPList = GameObject.Find("Content_Bottom/ScrollView/Viewport").GetComponent<BuddyIPList>();
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            SelectBuddy lSelect = lMenuManager.GetComponentInChildren<SelectBuddy>();
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "BottomUI", "ScrollView" });
            // Creating UI Objects
            // Bottom UI objects.
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { BackToConnectionMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "select", "", new List<UnityAction>() { lSelect.BuddySelected }));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            // Top UI objects.
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "+", new List<UnityAction>() { lMenuManager.GoAddBuddyMenu }));
            LoadingUI.AddObject(lPoolManager.fTextField_Searching("Content_Top/Top_UI", "search", "", null, new List<UnityAction<string>>() { lIPList.SearchForBuddy }));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Undo", new List<UnityAction>() { lIPList.CreateListDisplay }));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Trash", new List<UnityAction>() { lSelect.DeleteBuddy }));

            // Enable the Buddy list display with auto refresh.
            GameObject.Find("Viewport").GetComponent<BuddyIPList>().enabled = true;
            lIPList.enabled = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(indexState == (int)State.IDLE) {
            // Disable the Buddy list display.
            BuddyIPList lIPList = GameObject.Find("Viewport").GetComponent<BuddyIPList>();
            lIPList.enabled = false;
        }

        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    private void BackToConnectionMenu()
    {
        GameObject.Find("MenuManager").GetComponent<GoBack>().PreviousMenu();
        GameObject.Find("BackgroundListener").GetComponent<BackgroundListener>().UnsubscribeNotifications();
    }
}
