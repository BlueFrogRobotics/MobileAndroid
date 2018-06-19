using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Buddy information edition state.
/// </summary>
public class EditBuddyState : ASubState {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationEditBuddy", "TopUI", "BottomUI", "ScrollView" });

            // Creating UI Objects
            // Bottom UI content.
			GameObject lbuddyNameField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "buddysname", SelectBuddy.BuddyName, "", null, null, null);
			lbuddyNameField.name = "BuddyNameField";
			LoadingUI.AddObject(lbuddyNameField);

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "confirmchanges", "", new List<UnityAction>() { SaveBuddyChanges }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            // Top UI content.
            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));

            //LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", null));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            //LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Floppy", new List<UnityAction>() { SaveBuddyChanges }));
        }
    }

    /// <summary>
    /// Save the changes that were made to the Buddy.
    /// </summary>
    private void SaveBuddyChanges()
    {
		string specialID = SelectBuddy.BuddyID;
		string name = GameObject.Find("BuddyNameField").GetComponent<InputField>().text;

		if(name == "") {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Le nom ne peut pas être vide", "Warning");

            return;
		}

        // Send the changes to the database.
		GameObject.Find("DBManager").GetComponent<DBManager>().StartEditBuddy(specialID, name);
    }
}

