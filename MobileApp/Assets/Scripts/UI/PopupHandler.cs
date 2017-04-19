using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupHandler : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject popupWindow;

    [SerializeField]
    private GameObject popupShowQrCode;

    [SerializeField]
    private GameObject popupYesNo;

    [SerializeField]
    private GameObject popupReadQrCode;

    [SerializeField]
    private QRCodeManager qrCodeManager;

	[SerializeField]
	private PoolManager poolManager;

    public void OpenWindow()
    {
        ResetUI();
        popupWindow.SetActive(true);
        animator.SetTrigger("Open");
    }

	public void DisplayError(string iTitle, string iMessage)
	{
		ResetUI();
		popupWindow.SetActive(true);

		Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
		lTitle.text = iTitle;
		GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
		lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
		poolManager.fSimple_Text("PopUp_Window/Window/Content", iMessage, true);

		animator.SetTrigger("Open");
	}

	public void AccesRightWindow()
	{
		ResetUI();
		popupWindow.SetActive(true);
		//Debug.Log ("Popup Window " + popupWindow.activeInHierarchy + " / " + popupWindow.activeSelf);

		Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
		lTitle.text = "REQUEST ACCESS";
		GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
		lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
		poolManager.fToggle ("PopUp_Window/Window/Content", "Access Distant Control", true);
		poolManager.fToggle ("PopUp_Window/Window/Content", "*Admin Access", true);
		poolManager.fSimple_Text("PopUp_Window/Window/Content", "*Allows you to directly access your Buddy's parameters", true);
		poolManager.fButton_Square("PopUp_Window/Window/Content", "SEND REQUEST", "", null);

		animator.SetTrigger("Open");
		//Debug.Log ("Popup Window " + popupWindow.activeInHierarchy + " / " + popupWindow.activeSelf);
	}

    public void OpenYesNo(string iQuestion = "")
    {
        ResetUI();
        popupYesNo.SetActive(true);
        animator.SetTrigger("Open");
    }

	public void OpenShowQrCode(string iCode = "12-34-56-78")
    {
        ResetUI();
		qrCodeManager.CreateQrCode(iCode);
        popupShowQrCode.SetActive(true);
        animator.SetTrigger("Open");
    }

    public void OpenReadQrCode()
    {
        ResetUI();
        qrCodeManager.SwitchQRCodeReader();
        popupReadQrCode.SetActive(true);
        animator.SetTrigger("Open");
    }

    public void ClosePopup()
    {
        animator.SetTrigger("Close");
    }

    public void ResetUI()
    {
        popupWindow.SetActive(false);
        popupYesNo.SetActive(false);
        popupShowQrCode.SetActive(false);
        popupReadQrCode.SetActive(false);
    }
}
