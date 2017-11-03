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
        //Because Chat and RemoteControl do not have their own views...
        while (mViewTree.Last () == mCurrentMenu)
            mViewTree.Remove (mCurrentMenu);
        
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
        linkManager.SetMenuBuddyValue(15);
        SwitchToMenu("GoConnectBuddy");
    }

    public void GoChatMenu()
    {
        SwitchToMenu(1);
    }

    public void GoRemoteControlMenu()
    {
        SwitchToMenu(3, "Waiting for call confirmation ...");
    }

    public void GoBuddySettings()
    {
        SwitchToMenu(4);
    }

    //Self-explanatory
    private void SwitchToMenu(string iMenu)
    {
        mViewTree.Add (mCurrentMenu);
        mCurrentMenu = iMenu;
        canvasAnimator.SetTrigger(mCurrentMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    private void SwitchToMenu(int iMenu, string iWaitingMessage = "Loading ...")
    {
        LoadingBuddyMessage = iWaitingMessage;
        mCurrentMenu = "GoConnectBuddy";
        linkManager.SetMenuBuddyValue(iMenu);
        canvasAnimator.SetTrigger("GoLoadingBuddy");
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
			linkManager.SetMenuBuddyValue (3);
			canvasAnimator.SetTrigger ("EndScene");
		}
		else
		{
			if (lRTC != null) {
				lRTC.StopWebRTC ();
			}
			
			canvasAnimator.SetTrigger("GoConnectBuddy");
			canvasAnimator.SetTrigger("EndScene");
			//GameObject.Find("PopUps").GetComponent<PopupHandler>().DisplayError("Erreur", "Impossible d'établir la connection avec le robot");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Impossible d'établir la connexion avec le robot", "NoResponse");
        }
	}
}