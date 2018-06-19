using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Account edition state.
/// </summary>
public class EditAccountState: ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN) {
			DBManager.tmpImgPath = "";
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            PhoneUser lPhoneUser = GameObject.Find("DBManager").GetComponent<DBManager>().CurrentUser;

            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationEditAccount", "TopUI", "BottomUI", "ScrollView" });

            // Creating UI Objects.
            // Bottom UI content.
			GameObject lNewPasswordField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "newpassword", "", "Lock", null, null, null);
			lNewPasswordField.name = "New_PW_Input";
            lNewPasswordField.GetComponent<InputField>().inputType = InputField.InputType.Password;
			LoadingUI.AddObject(lNewPasswordField);
			GameObject lNewPasswordConfField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "reenterpassword", "", "Lock", null, null, null);
			lNewPasswordConfField.name = "New_PWConf_Input";
            lNewPasswordConfField.GetComponent<InputField>().inputType = InputField.InputType.Password;
            LoadingUI.AddObject(lNewPasswordConfField);
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));
            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "allownotifications", false));

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "confirmchanges", "", new List<UnityAction>() { SaveProfileChanges }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", true, null));

            // Top UI content.
			GameObject lUserPicture = lPoolManager.fButton_User_Big("Content_Top", "", new List<UnityAction>() { EditPicture });
			lUserPicture.name = "Connect_User_Picture";
			LoadingUI.AddObject(lUserPicture);

			LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", new List<UnityAction>() { DeleteAccount }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
        }
    }

    /// <summary>
    /// Save changes that were made to the profile.
    /// </summary>
    private void SaveProfileChanges()
    {
		string firstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
		string lastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;

        // If some information is missing, don't go any further.
		if(firstName == "" || lastName == "") {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Le prénom ou le nom ne peuvent pas être vide", "Warning");

            return;
		}

		string password = GameObject.Find("New_PW_Input").GetComponent<InputField>().text;
		string passwordConf = GameObject.Find("New_PWConf_Input").GetComponent<InputField>().text;

        // If new password doesn't match the confirmation, don't go any further.
		if(password != passwordConf) {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Les mots de passe ne sont pas identiques", "Warning");
            return;
		}

		GameObject.Find("DBManager").GetComponent<DBManager>().StartEditAccount(firstName, lastName, password);
    }

    /// <summary>
    /// Delete account from the database and the local saving.
    /// </summary>
	private void DeleteAccount()
	{
		GameObject.Find("PopUps").GetComponent<PopupHandler>().DeleteAccountPopup(onDeleteAccountConfirmed);
	}

    /// <summary>
    /// Callback for proper deletion of the account.
    /// </summary>
	private void onDeleteAccountConfirmed()
	{
		string password = GameObject.Find("PopUps").GetComponent<PopupHandler>().GetDeleteAccountPassword();
		GameObject.Find("DBManager").GetComponent<DBManager>().StartDeleteAccount(password);
	}

    /// <summary>
    /// Change the profile picture.
    /// </summary>
	private void EditPicture()
	{
		GameObject.Find("DBManager").GetComponent<DBManager>().OpenFileBrowser(true);
	}
}
