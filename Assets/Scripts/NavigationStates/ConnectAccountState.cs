using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State where a user connects to his account.
/// </summary>
public class ConnectAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            DBManager lDB = GameObject.Find("DBManager").GetComponent<DBManager>();
            // Cleaning previously created objects
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationAccount", "TopUI", "BottomUI", "ScrollView" });
            // Creating UI Objects
            // Bottom UI objects
            GameObject lInputFieldEM = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "mymail", "", "Email", null, null, null);
            lInputFieldEM.name = "EMail_Input";
            LoadingUI.AddObject(lInputFieldEM);
            GameObject lInputFieldPW = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "enterpassword", "", "Lock", null, null, null);
            lInputFieldPW.name = "Password_Input";
            lInputFieldPW.GetComponent<InputField>().inputType = InputField.InputType.Password;
            lInputFieldEM.GetComponent<InputField>().text = lDB.CurrentUser.Email;
            LoadingUI.AddObject(lInputFieldPW);

			LoadingUI.AddObject(lPoolManager.fButton_Text_Underline("Content_Bottom/ScrollView/Viewport", "forgotpassword", new List<UnityAction>() { onForgottenPasswordClicked }));
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));

            GameObject lNotifToggle = lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "allownotifications", false);
            lNotifToggle.name = "Notification_Toggle";
            LoadingUI.AddObject(lNotifToggle);

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoToFirstMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "login", "", new List<UnityAction>() { Connection }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", false , null));

            // Top UI objects
            GameObject lUserPicture = lPoolManager.fButton_User_Big("Content_Top", "", null);
            lUserPicture.name = "Connect_User_Picture";
            LoadingUI.AddObject(lUserPicture);

			LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", new List<UnityAction>() { RemoveLocalAccount }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));

            lDB.GenerateUserDisplay();
        }
    }

    //Gets info from the input fields and connects to remote DB.
    private void Connection()
	{
        string lFirstName;
        string lLastName;

        if (GameObject.Find("TextFirstName") != null) {
            lFirstName = GameObject.Find("TextFirstName").GetComponent<Text>().text;
            lLastName = GameObject.Find("Text_LastName").GetComponent<Text>().text;
        } else {
            lFirstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
            lLastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
        }

		string lEmail = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		string lPassword = GameObject.Find("Password_Input").GetComponent<InputField>().text;
        bool lNotif = GameObject.Find("Notification_Toggle").GetComponent<Toggle>().isOn;
		GameObject.Find("DBManager").GetComponent<DBManager>().StartRequestConnection(lFirstName, lLastName, lEmail, lPassword, lNotif);
    }

	private void RemoveLocalAccount()
	{
		string lFirstName;
		string lLastName;

		if (GameObject.Find("TextFirstName") != null) {
			lFirstName = GameObject.Find("TextFirstName").GetComponent<Text>().text;
			lLastName = GameObject.Find("Text_LastName").GetComponent<Text>().text;
		} else {
			lFirstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
			lLastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
		}

		string lEmail = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		GameObject.Find ("DBManager").GetComponent<DBManager> ().RemoveUserToConfig (lFirstName, lLastName, lEmail);
	}

	private void onForgottenPasswordClicked()
	{
        // Make sure user entered his e-mail address to send him a mail to reset his password.
		string lEmail = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		if(lEmail.Length == 0) {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Veuillez renseigner le champ email", "Warning");
        } else {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenYesNoIcon("Voulez-vous vraiment réinitialiser le mot de passe du compte suivant ?\n" + lEmail, onForgottenPasswordConfirmed, "Locked");
        }
	}

	private void onForgottenPasswordConfirmed()
	{
        // Get the e-mail and send to it a link to reset the password.
		string lEmail = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		GameObject.Find("DBManager").GetComponent<DBManager>().StartForgottenPassword(lEmail);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Destroy everything inside the "account navigation".
		if (indexState == (int)State.CLOSE) {
			GameObject Dots = GameObject.Find ("CanvasApp/Content_Top/Navigation_Account/Dots");
			Debug.Log ("ConnectAccount OnStateExit : " + Dots);
			foreach (Transform lChild in Dots.transform)
				GameObject.Destroy (lChild.gameObject);
		}
		base.OnStateExit(animator, stateInfo, layerIndex);
	}
}
