using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoBack : MonoBehaviour
{
    [SerializeField]
    private Animator canvasAnimator;

    [SerializeField]
    private LinkManager linkManager;

    private string mPreviousMenu;
    private string mCurrentMenu;

    // Use this for initialization
    void Start()
    {
        mPreviousMenu = "ConnectAccount";
        mCurrentMenu = "ConnectAccount";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (mCurrentMenu == "ConnectAccount")
                Application.Quit();
            else
                PreviousMenu();
        }
    }

    public void PreviousMenu()
    {
        switch (mPreviousMenu)
        {
            case "ConnectAccount":
                canvasAnimator.SetTrigger("GoConnectAccount");
                break;
            case "FirstConnection":
                canvasAnimator.SetTrigger("GoFirstConnexion");
                break;
            case "CreateAccount":
                canvasAnimator.SetTrigger("GoCreateAccount");
                break;
            case "SelectBuddy":
                canvasAnimator.SetTrigger("GoSelectBuddy");
                break;
            case "Connected":
                canvasAnimator.SetTrigger("GoConnectBuddy");
                break;
            case "AdminMenu":
                linkManager.SetMenuBuddyValue(4);
                break;
        }
        mCurrentMenu = mPreviousMenu;
        canvasAnimator.SetTrigger("EndScene");
    }

    public void SavePreviousMenu(string iMenu)
    {
        mCurrentMenu = "";
        mPreviousMenu = iMenu;
    }
}
