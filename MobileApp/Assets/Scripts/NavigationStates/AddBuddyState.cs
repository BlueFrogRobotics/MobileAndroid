using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class AddBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            PopupHandler lPopup = GameObject.Find("PopUps").GetComponent<PopupHandler>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "NavigationEdit", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // BOTTOM UI
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
			LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "ADD THIS CONTACT", "", new List<UnityAction>() { OnAddBuddyClicked }));
            LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "toto", true, null));

			GameObject specialIdField = lPoolManager.fTextField_Icon ("Content_Bottom/ScrollView/Viewport", "ENTER BUDDY'S ID", "", "QRCode", null, null, null);
			specialIdField.name = "specialIdField";
			LoadingUI.AddObject(specialIdField);
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/ScrollView/Viewport", "OR", false));
            GameObject lBuddyID = lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "Scan your contact ID", "", new List<UnityAction>() { lPopup.OpenReadQrCode });
            lBuddyID.name = "Buddy_ID";
            LoadingUI.AddObject(lBuddyID);
            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Connect By Default", false));
            // TOP UI
            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));
        }
    }

    private void OnAddBuddyClicked()
    {
		Debug.Log ("OnAddBuddyClicked");

		string specialId = GameObject.Find("specialIdField").GetComponent<InputField>().text;
		string name = GameObject.Find("Field_LastName").GetComponent<InputField>().text;

		GameObject.Find("DBManager").GetComponent<DBManager>().StartAddBuddyToUser(specialId, name);
    }
}
