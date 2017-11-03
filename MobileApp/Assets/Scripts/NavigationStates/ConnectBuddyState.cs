using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class ConnectBuddyState : ASubState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (indexState == 1)
        {
            GoBack lMenuManager = GameObject.Find("MenuManager").GetComponent<GoBack>();
            // CLEANNING PREVIOUS CREATED OBJECT
            LoadingUI.ClearUI();
            PoolManager lPoolManager = animator.GetComponent<PoolManager>();
            // DESACTIVATE, ACTIVATE GENERICS
            GameObject.Find("ScriptUI").GetComponent<HandleGeneric>().DisableGeneric(new ArrayList() { "TopUI", "NavigationDisplay", "BottomUI", "ScrollView" });
            // CREATING OBJECTS
            // BOTTOM UI
            LoadingUI.AddObject(lPoolManager.fButton_L("Content_Bottom/Bottom_UI", "VLeft", new List<UnityAction>() { GoSelectMenu }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Bottom/Bottom_UI", "FULL ACCESS", false ));
            //LoadingUI.AddObject(lPoolManager.fButton_User("Content_Bottom/Bottom_UI", "toto", true, new List<UnityAction>() { lMenuManager.GoEditAccountMenu }));

			LoadingUI.AddObject(lPoolManager.fBuddy_Status("Content_Bottom/ScrollView/Viewport", SelectBuddy.BuddyID, true));
			LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "CHAT WITH BUDDY", "", new List<UnityAction>() { lMenuManager.GoChatMenu, LaunchChat }));
            LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "REMOTE CONTROL", "", new List<UnityAction>() { StartRemoteControl }));
            //LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "BUDDY SETTINGS", "", new List<UnityAction>() { lMenuManager.GoBuddySettings }));
			//LoadingUI.AddObject(lPoolManager.fButton_Square("Content_Bottom/ScrollView/Viewport", "REQUEST ACCESS", "", new List<UnityAction>() { ShowAccessRequest }));

            // TOP UI
			LoadingUI.AddObject(lPoolManager.fButton_L("Content_Top/Top_UI", "Trash", new List<UnityAction>() { DeleteBuddy }));
            LoadingUI.AddObject(lPoolManager.fSimple_Text("Content_Top/Top_UI", "", false));
            LoadingUI.AddObject(lPoolManager.fButton_R("Content_Top/Top_UI", "Edit", new List<UnityAction>() { lMenuManager.GoEditBuddyMenu }));

            LoadingUI.AddObject(lPoolManager.fButton_User_Big("Content_Top", "Default_Buddy", null));
            GameObject.Find("Navigation_Account/Text_LastName").GetComponent<Text>().text = SelectBuddy.BuddyName;
            GameObject.Find("Navigation_Account/TextFirstName").GetComponent<Text>().text = SelectBuddy.BuddyID;
            //LoadingUI.AddObject(lPoolManager.fButton_QrCode("Content_Top/Navigation_Account", null));
        }
    }

    private void GoSelectMenu()
    {
        GameObject.Find("MenuManager").GetComponent<GoBack>().PreviousMenu();
        SelectBuddy lSelect = GameObject.Find("SelectBuddy").GetComponent<SelectBuddy>();

        if(lSelect.Remote == SelectBuddy.RemoteType.WEBRTC) {
            GameObject.Find("UnityWebrtc").SetActive(false);
        }        
    }

    private void StartRemoteControl()
    {
        GameObject.Find ("UnityWebrtc").GetComponent<Webrtc>().InitWebRTC();
        GameObject.Find("MenuManager").GetComponent<GoBack>().GoRemoteControlMenu();
        GameObject.Find("LaunchTelepresence").GetComponent<LaunchTelepresence>().ConnectToBuddy();
    }

	private void ShowAccessRequest()
	{
		GameObject.Find("PopUps").GetComponent<PopupHandler>().AccesRightWindow();
	}

    private void LaunchChat()
    {
        GameObject.Find("Chat").GetComponent<ChatManager>().LoadMessageHistory();
        BackgroundListener lListener = GameObject.Find("BackgroundListener").GetComponent<BackgroundListener>();
        lListener.SubscribeChatChannel();
    }

	private void DeleteBuddy()
	{
		//GameObject.Find("PopUps").GetComponent<PopupHandler>().PopupConfirmCancel("Supression", "Voulez-vous supprimer ce Buddy de la liste ?", onDeleteBuddyConfirmed);
        GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenYesNoIcon("Voulez-vous supprimer ce Buddy de la liste ?", onDeleteBuddyConfirmed, "Warning");
    }

	private void onDeleteBuddyConfirmed()
	{
		GameObject.Find("DBManager").GetComponent<DBManager>().StartRemoveBuddyFromUser(SelectBuddy.BuddyID);
	}

}
