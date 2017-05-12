using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private string mPreviousMenu;
    private string mCurrentMenu;

    // Use this for initialization
    void Start()
    {
        mPreviousMenu = "GoConnectAccount";
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
        string lTemp = mCurrentMenu;
        canvasAnimator.SetTrigger(mPreviousMenu);
        Debug.Log("Previous menu " + mPreviousMenu);
        mCurrentMenu = mPreviousMenu;
        mPreviousMenu = lTemp;
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
        SwitchToMenu("GoCreationAccount");
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
        mPreviousMenu = mCurrentMenu;
        mCurrentMenu = iMenu;
        canvasAnimator.SetTrigger(iMenu);
        canvasAnimator.SetTrigger("EndScene");
    }

    private void SwitchToMenu(int iMenu, string iWaitingMessage = "Loading ...")
    {
        LoadingBuddyMessage = iWaitingMessage;
        mPreviousMenu = mCurrentMenu;
        mCurrentMenu = "GoConnectBuddy";
        linkManager.SetMenuBuddyValue(iMenu);
        canvasAnimator.SetTrigger("GoLoadingBuddy");
        canvasAnimator.SetTrigger("EndScene");
    }
}
