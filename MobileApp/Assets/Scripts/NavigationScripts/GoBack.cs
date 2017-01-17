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

    // Use this for initialization
    void Start()
    {
        mPreviousMenu = "ConnectAccount";
    }

    // Update is called once per frame
    void Update()
    {

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
        canvasAnimator.SetTrigger("EndScene");
    }

    public void SavePreviousMenu(string iMenu)
    {
        mPreviousMenu = iMenu;
    }
}
