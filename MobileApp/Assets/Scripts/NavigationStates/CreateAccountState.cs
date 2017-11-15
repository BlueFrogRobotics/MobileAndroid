 using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == (int)State.OPEN)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANING PREVIOUSLY CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DISABLE, ENABLE GENERICS
			HandleGeneric lHandler = GameObject.Find("ScriptUI").GetComponent<HandleGeneric>();
			lHandler.DisableGeneric(new ArrayList() { "NavigationCreateAccount", "TopUI", "BottomUI", "ScrollView" });
			lHandler.SetDisplayInfos("Enter Your First Name", "Enter Your Last Name");
			lHandler.SetEditInfos("", "");
            // CREATE OBJECTS
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.PreviousMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "CREATE YOUR ACCOUNT", "", new List<UnityAction>() { CreateAccount }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", false, null));

            GameObject lEmailField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Email Address", "", "Email", null, null, null);
            lEmailField.name = "Create_Email_Input";
            LoadingUI.AddObject(lEmailField);
            GameObject lPasswordField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Your Password", "", "Lock", null, null, null);
            lPasswordField.name = "Create_PW_Input";
            lPasswordField.GetComponent<InputField>().inputType = InputField.InputType.Password;
            LoadingUI.AddObject(lPasswordField);
            GameObject lPasswordConfField = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Repeat Your Password", "", "Lock", null, null, null);
            lPasswordConfField.name = "Create_PWConf_Input";
            lPasswordConfField.GetComponent<InputField>().inputType = InputField.InputType.Password;
            LoadingUI.AddObject(lPasswordConfField);

            LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false));
            //LoadingUI.AddObject(lPoolManager.fToggle_Underline("Content_Bottom/ScrollView/Viewport", "Agree to the Terms of Service", false, null));
			LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "DefaultUser", null));
            //NEED TO ADD NAVIGATION ACCOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
        }
    }

    private void CreateAccount()
    {
		string firstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
		string lastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
		string email = GameObject.Find("Create_Email_Input").GetComponent<InputField>().text;
		string password = GameObject.Find("Create_PW_Input").GetComponent<InputField>().text;
		string passwordConf = GameObject.Find("Create_PWConf_Input").GetComponent<InputField>().text;

		if(firstName == "" || lastName == "" || email == "" || password == "" || passwordConf == "")
		{
			//GameObject.Find ("PopUps").GetComponent<PopupHandler>().DisplayError("Erreur", "Veuillez remplir tous les champs");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Veuillez remplir tous les champs", "Warning");

            return;
		}

		if(password != passwordConf)
		{
			//GameObject.Find ("PopUps").GetComponent<PopupHandler>().DisplayError("Erreur", "Les mots de passe ne sont pas identiques");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Les mots de passe ne sont pas identiques", "Warning");
            return;
		}

		GameObject.Find("DBManager").GetComponent<DBManager>().StartCreateAccount(firstName, lastName, email, password, passwordConf);
    }
}
