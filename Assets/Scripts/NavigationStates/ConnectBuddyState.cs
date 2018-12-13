using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// State where the user has selected a Buddy and then selects in what way to interact with it.
/// </summary>
public class ConnectBuddyState : ASubState {

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
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "NavigationDisplay", "BottomUI", "ScrollView" });
            // Creating UI Objects
            // Bottom UI objects
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { GoSelectMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/Bottom_UI", "fullaccess", false ));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "toto", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

            // Display the different remote interaction possibilities.
			LoadingUI.AddObject(lPoolManager.fBuddy_Status("Content_Bottom/ScrollView/Viewport", SelectBuddy.BuddyID, true));
			//LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "chatwithbuddy", "", new List<UnityAction>() { lMenuManager.LoadChatMenu }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "remotecontrol", "", new List<UnityAction>() { lMenuManager.LoadRemoteControlMenu, ResetSoundManager }));
            //LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "wizardofoz", "", new List<UnityAction>() { lMenuManager.LoadWizardOfOz, ResetSoundManager }));
            //LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "BUDDY SETTINGS", "", new List<UnityAction>() { lMenuManager.GoBuddySettings }));
            //LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "REQUEST ACCESS", "", new List<UnityAction>() { ShowAccessRequest }));

            // Top UI objects
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", new List<UnityAction>() { DeleteBuddy }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Edit", new List<UnityAction>() { lMenuManager.GoEditBuddyMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));
            GameObject.Find("Navigation_Account/Text_LastName").GetComponent<Text>().text = SelectBuddy.BuddyName;
            GameObject.Find("Navigation_Account/TextFirstName").GetComponent<Text>().text = SelectBuddy.BuddyID;
            //LoadingUI.AddObject(lPoolManager.fButton_QrCode("Content_Top/Navigation_Account", null));
        }
    }

    /// <summary>
    /// Go back to the Buddy selection menu.
    /// </summary>
    private void GoSelectMenu()
    {
        GameObject.Find("MenuManager").GetComponent<GoBack>().PreviousMenu();
        SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();

        if(lSelect.Remote == SelectBuddy.RemoteType.WEBRTC) {
            GameObject.Find("UnityWebrtc").SetActive(false);
        }        
    }

    /// <summary>
    /// Popup to request some remote access rights for Buddy.
    /// </summary>
	private void ShowAccessRequest()
	{
		GameObject.Find("PopUps").GetComponent<PopupHandler>().AccesRightWindow();
	}

    /// <summary>
    /// Unlink Buddy from the active account.
    /// </summary>
	private void DeleteBuddy()
	{
        GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenYesNoIcon("Voulez-vous supprimer ce Buddy de la liste ?", onDeleteBuddyConfirmed, "Warning");
    }

    /// <summary>
    /// Callback to unlink Buddy from the account through a database request.
    /// </summary>
	private void onDeleteBuddyConfirmed()
	{
		GameObject.Find("DBManager").GetComponent<DBManager>().StartRemoveBuddyFromUser(SelectBuddy.BuddyID);
	}

    /// <summary>
    /// Reset the sound for a remote control session.
    /// </summary>
	private void ResetSoundManager()
	{
		SoundManager.reset = true;
	}

}
