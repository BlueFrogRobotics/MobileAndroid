using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Back button manager. Saves the previous state and goes back to it when asked
/// </summary>
public class GoBack : MonoBehaviour
{
    public static string LoadingBuddyMessage;

    [SerializeField]
    private Animator canvasAnimator;

    [SerializeField]
    private LinkManager linkManager;

    private string mCurrentMenu;
    private List<string> mViewTree;

    // Use this for initialization
    void Start()
    {
        mViewTree = new List<string> ();
        mCurrentMenu = "GoConnectAccount";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (mCurrentMenu == "GoConnectAccount")
                Application.Quit();
            else
                PreviousMenu();
        }
    }

    //Go to previously saved menu
    public void PreviousMenu()
    {
        mCurrentMenu = mViewTree.Last();
        mViewTree.Remove(mCurrentMenu);

        canvasAnimator.SetTrigger(mCurrentMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    public void GoToFirstMenu()
    {
        SwitchToMenu("GoFirstConnexion");
    }

    public void GoToTermsOfUse()
    {
        SwitchToMenu("GoTermsOfUses");
    }

    public void GoCreationMenu()
    {
        SwitchToMenu("GoCreateAccount");
    }

    public void GoConnectionMenu()
    {
        SwitchToMenu("GoConnectAccount");
    }

    public void GoEditAccountMenu()
    {
        SwitchToMenu("GoEditAccount");
    }

    public void GoSelectBuddyMenu()
    {
        SwitchToMenu("GoSelectBuddy");
    }

    public void GoAddBuddyMenu()
    {
        SwitchToMenu("GoAddBuddy");
    }

    public void GoEditBuddyMenu()
    {
        SwitchToMenu("GoEditBuddy");
    }

    public void GoConnectedMenu()
    {
        SwitchToMenu("GoConnectBuddy");
    }

    public void LoadChatMenu()
    {
        LoadingBuddyMessage = "Loading chat room...";
        canvasAnimator.SetInteger("MenuBuddy",1);
        SwitchToMenu("GoLoadingBuddy");
    }

    public void GoChatMenu()
    {
        SwitchToMenu("GoMessage");
    }

    public void LoadRemoteControlMenu()
    {
        LoadingBuddyMessage = "Waiting for call confirmation...";
        canvasAnimator.SetInteger("MenuBuddy", 2);
        SwitchToMenu("GoLoadingBuddy");
    }

    public void GoRemoteControlMenu()
    {
        SwitchToMenu("GoRemoteControl");
    }

    private void SwitchToMenu(string iMenu)
    {
        // Do not add LoadingBuddy state in view tree as it is a transitory state
        if (mCurrentMenu != "GoLoadingBuddy")
        {
            mViewTree.Add(mCurrentMenu);
        }
        mCurrentMenu = iMenu;

        canvasAnimator.SetTrigger(mCurrentMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    public void WaitForCallConfirmation(SelectBuddy.RemoteType iType)
    {
        if (iType == SelectBuddy.RemoteType.LOCAL)
        {
            StartCoroutine(WaitForLocalConfirmation());
        }
        else if (iType == SelectBuddy.RemoteType.WEBRTC)
        {
            StartCoroutine(WaitForRTCConfirmation());
        }
    }

    private IEnumerator WaitForLocalConfirmation()
    {
        CallAcceptOTOReceiver lConfirmation = GameObject.Find("CallAcceptReceiver").GetComponent<CallAcceptOTOReceiver>();

        while (lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.WAITING) {
            yield return new WaitForSeconds(0.5F);
        }

		processConnectionState(lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.ACCEPTED);
    }

    private IEnumerator WaitForRTCConfirmation()
    {
        Webrtc lRTC = GameObject.Find("UnityWebrtc").GetComponent<Webrtc>();
        float lTimeWaited = 0;

        while(!lRTC.Connected && lTimeWaited < 15F) {
            lTimeWaited += 0.5F;
            yield return new WaitForSeconds(0.5F);
        }

		processConnectionState(lRTC.Connected,lRTC);
    }

	private void processConnectionState(bool connected, Webrtc lRTC = null)
	{
		if(connected)
		{
            GoRemoteControlMenu();
		}
		else
		{
			if (lRTC != null) {
				lRTC.StopWebRTC ();
			}
			
            PreviousMenu();
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Impossible d'établir la connexion avec le robot", "NoResponse");
        }
	}
}