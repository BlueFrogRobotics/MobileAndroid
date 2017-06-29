using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ConnectAccountState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            DBManager lDB = GameObject.Find("DBManager").GetComponent<DBManager>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "NavigationAccount", "TopUI", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            //This makes getting stuff easier
            GameObject lInputFieldEM = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "mymail@example.com", "", "Email", null, null, null);
            lInputFieldEM.name = "EMail_Input";
            LoadingUI.AddObject(lInputFieldEM);
            GameObject lInputFieldPW = lPoolManager.fTextField_Icon("Content_Bottom/ScrollView/Viewport", "Enter your password", "", "Lock", null, null, null);
            lInputFieldPW.name = "Password_Input";
            lInputFieldPW.GetComponent<InputField>().inputType = InputField.InputType.Password;
            lInputFieldEM.GetComponent<InputField>().text = lDB.CurrentUser.Email;
            LoadingUI.AddObject(lInputFieldPW);

            LoadingUI.AddObject(lPoolManager.fButton_Text_Underline("Content_Bottom/ScrollView/Viewport", "Forgot your password?", null));
            //LoadingUI.AddObject(lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Stay Connected", false));

            GameObject lNotifToggle = lPoolManager.fToggle("Content_Bottom/ScrollView/Viewport", "Allow Notifications", false);
            lNotifToggle.name = "Notification_Toggle";
            LoadingUI.AddObject(lNotifToggle);

            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { lMenuManager.GoToFirstMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/Bottom_UI", "LOGIN", "", new List<UnityAction>() { Connection }));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "", false , null));

            GameObject lUserPicture = lPoolManager.fButton_User_Big("Content_Top", "", null);
            lUserPicture.name = "Connect_User_Picture";
            LoadingUI.AddObject(lUserPicture);

			LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", new List<UnityAction>() { RemoveLocalAccount }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            //LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Edit", null));// new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));
            //NEED TO ADD NAVIGATION ACOUNT SCRIPT TO HANDLE "NavigationAccount" UI ELEMENTS !!!
            //lDB.ReadPhoneUsers();
        }
    }

    //Gets info from the input fields and connects to remote DB.
    private void Connection()
	{
        string firstName;
        string lastName;

        if (GameObject.Find("TextFirstName") != null) {
            firstName = GameObject.Find("TextFirstName").GetComponent<Text>().text;
            lastName = GameObject.Find("Text_LastName").GetComponent<Text>().text;
        } else {
            firstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
            lastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
        }

		string email = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		string password = GameObject.Find("Password_Input").GetComponent<InputField>().text;
        bool lNotif = GameObject.Find("Notification_Toggle").GetComponent<Toggle>().isOn;
		GameObject.Find("DBManager").GetComponent<DBManager>().StartRequestConnection(firstName, lastName, email, password, lNotif);
    }

	private void RemoveLocalAccount()
	{
		string firstName;
		string lastName;

		if (GameObject.Find("TextFirstName") != null) {
			firstName = GameObject.Find("TextFirstName").GetComponent<Text>().text;
			lastName = GameObject.Find("Text_LastName").GetComponent<Text>().text;
		} else {
			firstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
			lastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
		}

		string email = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
		GameObject.Find ("DBManager").GetComponent<DBManager> ().RemoveUserToConfig (firstName, lastName, email);
	}
}
