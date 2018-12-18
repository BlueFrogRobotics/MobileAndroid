using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Back button manager. Saves the previous state and goes back to it when asked.
/// This class should be used for every menu changed since it remembers the history of all screens that have been navigated through.
/// </summary>
public class GoBack : MonoBehaviour
{
    public static string LoadingBuddyMessage;

    [SerializeField]
    private GameObject WizardOfOzUI;

    [SerializeField]
    private Animator canvasAnimator;

    [SerializeField]
    private LinkManager linkManager;

    private string mCurrentMenu;
    private List<string> mViewTree;

    private BackToMenu mBackToMenu;
    

    void Start()
    {
        mViewTree = new List<string>();
        mCurrentMenu = "GoConnectAccount";
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mCurrentMenu == "GoConnectAccount")
                Application.Quit();
            else if (mCurrentMenu == "GoRemoteControl" || mCurrentMenu == "GoWizardOfOz")
            {
                Webrtc lRTC = GameObject.Find("UnityWebrtc").GetComponent<Webrtc>();
                if (lRTC.Connected)
                {
                    lRTC.StopWebRTC();
                    PreviousMenu();
                }
            }
            else
                PreviousMenu();
        }
    }

    /// <summary>
    /// Go to the previous menu.
    /// </summary>
    public void PreviousMenu()
    {
        if (mViewTree.Count == 0)
            return ;
        mCurrentMenu = mViewTree[mViewTree.Count - 1];
        mViewTree.Remove(mCurrentMenu);

        canvasAnimator.SetTrigger(mCurrentMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    /// <summary>
    /// Go to the "First connection" menu.
    /// </summary>
    public void GoToFirstMenu()
    {
        SwitchToMenu("GoFirstConnexion");
    }

    /// <summary>
    /// Go to the "Terms of Use" menu.
    /// </summary>
    public void GoToTermsOfUse()
    {
        SwitchToMenu("GoTermsOfUses");
    }

    /// <summary>
    /// Go to the account creation menu.
    /// </summary>
    public void GoCreationMenu()
    {
        SwitchToMenu("GoCreateAccount");
    }

    /// <summary>
    /// Go to the account connection menu.
    /// </summary>
    public void GoConnectionMenu()
    {
        SwitchToMenu("GoConnectAccount");
    }

    /// <summary>
    /// Go to the account edition menu.
    /// </summary>
    public void GoEditAccountMenu()
    {
        SwitchToMenu("GoEditAccount");
    }

    /// <summary>
    /// Go to the Buddy selection menu.
    /// </summary>
    public void GoSelectBuddyMenu()
    {
        SwitchToMenu("GoSelectBuddy");
    }

    /// <summary>
    /// Go to the "Add a Buddy" menu.
    /// </summary>
    public void GoAddBuddyMenu()
    {
        SwitchToMenu("GoAddBuddy");
    }

    /// <summary>
    /// Go to the Buddy edition menu.
    /// </summary>
    public void GoEditBuddyMenu()
    {
        SwitchToMenu("GoEditBuddy");
    }

    /// <summary>
    /// Go to the "Connected to Buddy" menu.
    /// </summary>
    public void GoConnectedMenu()
    {
        SwitchToMenu("GoConnectBuddy");
    }

    /// <summary>
    /// Load the chat room.
    /// </summary>
    public void LoadChatMenu()
    {
        LoadingBuddyMessage = "loadingchatroom";
        canvasAnimator.SetInteger("MenuBuddy", 1);
        SwitchToMenu("GoLoadingBuddy");
    }

    /// <summary>
    /// Go to the chat menu.
    /// </summary>
    public void GoChatMenu()
    {
        SwitchToMenu("GoMessage");
    }

    /// <summary>
    /// Go to the remote control menu.
    /// </summary>
    public void LoadRemoteControlMenu()
    {
        SelectBuddy lSelect = this.GetComponentInChildren<SelectBuddy>();
        //Check if selected Buddy is accessible for a remote control session or not.
        if (lSelect.BuddyAccess())
        {
            LoadingBuddyMessage = "waitingcallconfirmation";
            canvasAnimator.SetInteger("MenuBuddy", 2);
            canvasAnimator.SetInteger("RemoteMode", 0);
            SwitchToMenu("GoLoadingBuddy");
        }
        else
        {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Buddy est déjà en ligne", "NoResponse");
        }
    }

    /// <summary>
    /// Load the "Wizard of Oz" remote control menu.
    /// </summary>
    public void LoadWizardOfOz()
    {
        SelectBuddy lSelect = this.GetComponentInChildren<SelectBuddy>();
        if (lSelect.BuddyAccess())
        {
            LoadingBuddyMessage = "waitingcallconfirmation";
            canvasAnimator.SetInteger("MenuBuddy", 3);
            canvasAnimator.SetInteger("RemoteMode", 1);
            SwitchToMenu("GoLoadingBuddy");
        }
        else
        {
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Buddy est déjà en ligne", "NoResponse");
        }
    }

    /// <summary>
    /// Go to the "classic" remote control menu.
    /// </summary>
    public void GoRemoteControlMenu()
    {
        SwitchToMenu("GoRemoteControl");
    }

    /// <summary>
    /// Go to the "Wizard of Oz" remote control menu.
    /// </summary>
    public void GoWizardOfOzMenu()
    {
        SwitchToMenu("GoWizardOfOZ");
    }

    /// <summary>
    /// Switch to another menu.
    /// </summary>
    /// <param name="iMenu">The name of the menu to change to.</param>
    private void SwitchToMenu(string iMenu)
    {
        // Do not add LoadingBuddy state in view tree as it's a transition state.
        if (mCurrentMenu != "GoLoadingBuddy")
        {
            mViewTree.Add(mCurrentMenu);
        }
        mCurrentMenu = iMenu;

        canvasAnimator.SetTrigger(mCurrentMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    /// <summary>
    /// Wait for call confirmation on Buddy before displaying the remote control menu.
    /// </summary>
    /// <param name="iType">The remote control session type (Local or WebRTC).</param>
    /// <param name="lWizardOfOz">Is the session a "Wizard of Oz" one ?</param>
    public void WaitForCallConfirmation(SelectBuddy.RemoteType iType, bool lWizardOfOz)
    {
        if (iType == SelectBuddy.RemoteType.LOCAL)
            StartCoroutine(WaitForLocalConfirmation(lWizardOfOz));
        else if (iType == SelectBuddy.RemoteType.WEBRTC)
            StartCoroutine(WaitForRTCConfirmation(lWizardOfOz));
    }

    /// <summary>
    /// Coroutine to wait for the local call confirmation.
    /// </summary>
    /// <param name="lWizardOfOz">Is the session a "Wizard of Oz" one ?</param>
    /// <returns>An enumerator.</returns>
    private IEnumerator WaitForLocalConfirmation(bool lWizardOfOz)
    {
        CallAcceptOTOReceiver lConfirmation = GameObject.Find("CallAcceptReceiver").GetComponent<CallAcceptOTOReceiver>();

        while (lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.WAITING)
        {
            yield return new WaitForSeconds(0.5F);
        }

        processConnectionState(lConfirmation.Status == CallAcceptOTOReceiver.CallStatus.ACCEPTED, lWizardOfOz);
    }

    /// <summary>
    /// Coroutine to wait for the WebRTC call confirmation.
    /// </summary>
    /// <param name="lWizardOfOz">Is the session a "Wizard of Oz" one ?</param>
    /// <returns>An enumerator.</returns>
    private IEnumerator WaitForRTCConfirmation(bool lWizardOfOz)
    {
        Webrtc lRTC = GameObject.Find("UnityWebrtc").GetComponent<Webrtc>();
        float lTimeWaited = 0;

        while (!lRTC.Connected && lTimeWaited < 30F)
        {
            lTimeWaited += 0.5F;
            yield return new WaitForSeconds(0.5F);
        }

        processConnectionState(lRTC.Connected, lWizardOfOz, lRTC);
    }

    private void processConnectionState(bool connected, bool lWizardOfOz, Webrtc lRTC = null)
    {
        //If Buddy is connected and available for a remote control session.
        if (connected)
        {
            if (lWizardOfOz)
                GoWizardOfOzMenu();
            else
                GoRemoteControlMenu();
        }
        //Else, stay at the "Connected to Buddy" menu, disable the WebRTC and display an error.
        else
        {
            if (lRTC != null)
            {
                lRTC.StopWebRTC();
            }

            PreviousMenu();
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Impossible d'établir la connexion avec le robot", "NoResponse");
        }
    }
}