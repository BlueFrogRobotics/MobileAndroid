 using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State to create a user account.
/// </summary>
public class CreateAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // Cleaning previously created objects.
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // Activate predefined generic elements.
            HandleGeneric lHandler = GameObject.Find("ScriptUI").GetComponent<HandleGeneric>();
			lHandler.DisableGeneric(new ArrayList() { "NavigationCreateAccount", "TopUI", "BottomUI", "ScrollView" });
			lHandler.SetDisplayInfos("Enter Your First Name", "Enter Your Last Name");
			lHandler.SetEditInfos("", "");

            // Creating UI Objects
            // Bottom UI objects
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "createaccount", "", new List<UnityAction>() { CreateAccount }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", false, null));

            GameObject lEmailField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "youremailaddress", "", "Email", null, null, null);
            lEmailField.name = "Create_Email_Input";
            LoadingUI.AddObject(lEmailField);
            GameObject lPasswordField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "yourpassword", "", "Lock", null, null, null);
            lPasswordField.name = "Create_PW_Input";
            lPasswordField.GetComponent<InputField>().inputType = InputField.InputType.Password;
            LoadingUI.AddObject(lPasswordField);
            GameObject lPasswordConfField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "repeatpassword", "", "Lock", null, null, null);
            lPasswordConfField.name = "Create_PWConf_Input";
            lPasswordConfField.GetComponent<InputField>().inputType = InputField.InputType.Password;
            LoadingUI.AddObject(lPasswordConfField);

            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "stayconnected", false));
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false));
            //LoadingUI.AddObject(lPoolManager.fToggle_Underline("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false, null));
			GameObject lUserPicture = lPoolManager.fButton_User_Big("Content_Top", "DefaultUser", new List<UnityAction>() { AddPicture });
			lUserPicture.name = "Connect_User_Picture";
			LoadingUI.AddObject(lUserPicture);
            //NEED TO ADD NAVIGATION ACCOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }

    /// <summary>
    /// Create account callback.
    /// </summary>
    private void CreateAccount()
    {
		string firstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
		string lastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
		string email = GameObject.Find("Create_Email_Input").GetComponent<InputField>().text;
		string password = GameObject.Find("Create_PW_Input").GetComponent<InputField>().text;
		string passwordConf = GameObject.Find("Create_PWConf_Input").GetComponent<InputField>().text;

        // Stop if some information is lacking.
		if(firstName == "" || lastName == "" || email == "" || password == "" || passwordConf == "") {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Veuillez remplir tous les champs", "Warning");
            return;
		}

		if(password != passwordConf) {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Les mots de passe ne sont pas identiques", "Warning");
            return;
		}

        // Send the information to an online PHP script to create the new user in the database.
		GameObject.Find("DBManager").GetComponent<DBManager>().StartCreateAccount(firstName, lastName, email, password, passwordConf);
	}

    /// <summary>
    /// Browse for a new user picture to be displayed.
    /// </summary>
	private void AddPicture()
	{
		GameObject.Find("DBManager").GetComponent<DBManager>().OpenFileBrowser(true);
	}
}
