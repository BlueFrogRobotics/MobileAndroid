using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EditBuddyState : ASubState {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationEditBuddy", "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            //LoadingUI.AddObject(lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "12-34-56-78", "", "QRCode", null, null, null));
            //LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/ScrollView/Viewport", "OR", false));
            //LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "Scan your contact ID", null, null));
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Set as default connection", false));

			GameObject lbuddyNameField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "buddysname", SelectBuddy.BuddyName, "", null, null, null);
			lbuddyNameField.name = "BuddyNameField";
			LoadingUI.AddObject(lbuddyNameField);

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "confirmchanges", "", new List<UnityAction>() { SaveBuddyChanges }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));

            //LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", null));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            //LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Floppy", new List<UnityAction>() { SaveBuddyChanges }));
        }
    }

    private void SaveBuddyChanges()
    {
		string specialID = SelectBuddy.BuddyID;
		string name = GameObject.Find("BuddyNameField").GetComponent<InputField>().text;

		if(name == "")
		{
            //GameObject.Find("PopUps").GetComponent<PopupHandler>().DisplayError("Erreur", "Le nom ne peut pas être vide");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Le nom ne peut pas être vide", "Warning");

            return;
		}

		GameObject.Find("DBManager").GetComponent<DBManager>().StartEditBuddy(specialID, name);
        //GameObject.Find("MenuManager").GetComponent<GoBack>().PreviousMenu();
    }
}

