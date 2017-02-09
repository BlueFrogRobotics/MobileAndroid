using UnityEngine;
using System.Collections;

public class SelectBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DesactivateGeneric(new ArrayList() { "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // DISPLAY ALL BUDDY's CONTACTS
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "YOUR BUDDY CONTACT(S)"));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "My BUDDY NAME", "ID:5458-FR74-DG59", "", true, true, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "My BUDDY NAME2", "ID:5458-FR32-D589", "", false, true, null));
            //DISPLAY BUDDY's NOT YET ADDED ON A SEARCH
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "NOT ADDED & AVAILABLE IN LOCAL"));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "BUDDY NOT ADDED", "ID:5458-FR99-ZX59", "Sprites/Ico_App", true, false, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "BUDDY NOT ADDED2", "ID:5458-FR54-JF59", "", true, false, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fSearching("Content_Bottom/ScrollView/Viewport"));
            // OTHER UI OBJECT
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Bottom/Bottom_UI", "VLeft", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_Square("Content_Bottom/Bottom_UI", "LOGIN", "", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_User("Content_Bottom/Bottom_UI", "", true, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_L("Content_Top/Top_UI", "+", null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fTextField_Searching("Content_Top/Top_UI", "Tape Your Search...", "", null, null));
            LoadingUI.AddObject(animator.GetComponent<PoolManager>().fButton_R("Content_Top/Top_UI", "Undo", null));
            //NEED TO ADD NAVIGATION ACOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }
}
