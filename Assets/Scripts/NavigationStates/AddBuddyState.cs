using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State where user can link a Buddy to his account, so to say, "add" it to a user's account.
/// </summary>
public class AddBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            PopupHandler lPopup = GameObject.Find("PopUps").GetComponent<PopupHandler>();
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "NavigationAddBuddy", "BottomUI", "ScrollView" });
            // Creating UI Objects
            // Bottom UI objects
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
			LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "addcontact", "", new List<UnityAction>() { OnAddBuddyClicked }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "toto", true, null));

			GameObject specialIdField = lPoolManager.fTextField_Icon ("Content_Bottom/ScrollView/Viewport", "enterbuddyid", "", "QRCode", null, null, null);
			specialIdField.name = "specialIdField";
			LoadingUI.AddObject(specialIdField);

            // Scan contact id with QRCode is not available right now
            //LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/ScrollView/Viewport", "or", false));
            //GameObject lBuddyID = lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "scancontactid", "", new List<UnityAction>() { lPopup.OpenReadQrCode });
            //lBuddyID.name = "Buddy_ID";
            //LoadingUI.AddObject(lBuddyID);

            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Connect By Default", false));
            // Top UI objects
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
