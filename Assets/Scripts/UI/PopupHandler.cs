using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the display of several popups.
/// </summary>
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
    private GameObject popupYesNoIcon;

    [SerializeField]
    private GameObject popupDisplayIcon;

    [SerializeField]
    private GameObject popupReadQrCode;

    [SerializeField]
    private QRCodeManager qrCodeManager;

	[SerializeField]
	private PoolManager poolManager;

    [SerializeField]
    private Webrtc webRTC;

	private GameObject deleteAccountInputPasswordField;

    /// <summary>
    /// Open the popup window.
    /// </summary>
    public void OpenWindow()
    {
        ResetUI();
        popupWindow.SetActive(true);
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// A popup to display information about the WebRTC session quality:
    /// Network status, remote connection status and device performance.
    /// </summary>
    public void DisplayRTCNetworkInfo()
    {
        ResetUI();
        popupWindow.SetActive(true);
        ResetWindowUI();

        GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
        lButton.GetComponent<Button>().onClick.AddListener(CloseWebRTCInfos);

        // Display a level for the device's connection level.
        GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>().text = "Network Status";
        poolManager.fSimple_Text("PopUp_Window/Window/Content", "connectionstatuslocal", true);
        GameObject lLocalLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lLocalLevel.name = "LocalLevel";

        // Display a level for the remote's connection level.
        poolManager.fSimple_Text("PopUp_Window/Window/Content", "connectionstatusremote", true);
        GameObject lRemoteLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lRemoteLevel.name = "RemoteLevel";

        // Display a level for the device's performance level.
        poolManager.fSimple_Text("PopUp_Window/Window/Content", "deviceperformance", true);
        GameObject lDeviceLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lDeviceLevel.name = "DeviceLevel";

        // Start a coroutine to update the information.
        StartCoroutine(UpdateWebRTCInfos());

        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Display an error popup with a title and a message.
    /// </summary>
    /// <param name="iTitle">Title of the error popup.</param>
    /// <param name="iMessage">Message of the error.</param>
    public void DisplayError(string iTitle, string iMessage)
	{
        // Reset the content of the Popup window.
		ResetUI();
		popupWindow.SetActive(true);
        ResetWindowUI();

        // Display the title and the body message with a button to close the window.
        Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
		lTitle.text = iTitle;
		GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
		lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
		poolManager.fSimple_Text("PopUp_Window/Window/Content", iMessage, true);

		animator.SetTrigger("Open");
	}

    /// <summary>
    /// Popup to display the access rights for a specific Buddy.
    /// </summary>
	public void AccesRightWindow()
    {
        // Reset the content of the Popup window.
        ResetUI();
		popupWindow.SetActive(true);
        ResetWindowUI();

        // Display a title and several toggles for access rights.
        Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
		lTitle.text = "REQUEST ACCESS";
		GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
		lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
		poolManager.fToggle ("PopUp_Window/Window/Content", "accessdistantcontrol", true);
		poolManager.fToggle ("PopUp_Window/Window/Content", "adminaccess", true);
		poolManager.fSimple_Text("PopUp_Window/Window/Content", "allowsdirectaccessparameters", true);
		poolManager.fButton_Square("PopUp_Window/Window/Content", "sendrequest", "", null);

        // Open the window.
		animator.SetTrigger("Open");
	}

    /// <summary>
    /// Display a popup to ask the user to confirm the account deletion.
    /// </summary>
    /// <param name="iCallback">Callback once the account deletion is confirmed.</param>
    public void DeleteAccountPopup(UnityAction iCallback)
    {
        // Reset the content of the Popup window.
        ResetUI();
        popupWindow.SetActive(true);
        ResetWindowUI();

        // Display a title and a simple text.
        Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
        lTitle.text = "DELETE ACCOUNT - CONFIRMATION";

        GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
        lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);

        poolManager.fSimple_Text("PopUp_Window/Window/Content", "enteraccountpassword", true);

        // Ask the user to enter and confirm his password.
		deleteAccountInputPasswordField = poolManager.fTextField_Icon("PopUp_Window/Window/Content", "password", "", null, null, null, null);
		deleteAccountInputPasswordField.name = "Popup_PasswordConfirm";
		deleteAccountInputPasswordField.GetComponent<InputField>().inputType = InputField.InputType.Password;

        poolManager.fButton_Square("PopUp_Window/Window/Content", "confirm", "", new List<UnityAction>() { iCallback });

        // Open the window.
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Get the content of the password in the account deletion popup.
    /// </summary>
    /// <returns>The user password.</returns>
	public string GetDeleteAccountPassword()
	{
		return deleteAccountInputPasswordField.GetComponent<InputField>().text;
	}

    /// <summary>
    /// Popup to ask for confirmation of an action or its cancellation.
    /// </summary>
    /// <param name="title">Title of the message.</param>
    /// <param name="message">The body of the message.</param>
    /// <param name="iCallback">Callback after confirmation.</param>
	public void PopupConfirmCancel(string title, string message, UnityAction iCallback)
    {
        // Reset the content of the Popup window.
        ResetUI();
		popupWindow.SetActive(true);
		ResetWindowUI();

		Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
		lTitle.text = title;

        // Display the message and the two buttons to either "confirm" or "cancel".
		poolManager.fSimple_Text("PopUp_Window/Window/Content", message, true);
		poolManager.fButton_Square("PopUp_Window/Window/Content", "confirm", "", new List<UnityAction>() { iCallback, ClosePopup });
		poolManager.fButton_Square("PopUp_Window/Window/Content", "cancel", "", new List<UnityAction>() { ClosePopup });

        // Open the window.
		animator.SetTrigger("Open");
	}

    /// <summary>
    /// Open a Yes/No popup.
    /// </summary>
    /// <param name="iQuestion">The question to be displayed.</param>
    public void OpenYesNo(string iQuestion = "")
    {
        // Reset the content of the Popup window.
        ResetUI();
        popupYesNo.SetActive(true);
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Open a Yes/No popup with a special icon.
    /// </summary>
    /// <param name="iQuestion">The question to be displayed.</param>
    /// <param name="iCallBackYes">Callback after the "yes" button is pressed.</param>
    /// <param name="iIconName">The name of the icon to display.</param>
    public void OpenYesNoIcon(string iQuestion, UnityAction iCallBackYes, string iIconName = "")
    {
        // Reset the content of the Popup window.
        ResetUI();
        popupYesNoIcon.SetActive(true);

        // Display the text, the yes and no buttons and the icon.
        Text lQuestion = GameObject.Find("Popup_YesNoIcon/Text").GetComponent<Text>();
        lQuestion.text = iQuestion;

        Button lYesButton = GameObject.Find("Popup_YesNoIcon/YesNo/Button_Yes").GetComponent<Button>();
        //Bug 7: MG
        lYesButton.onClick.RemoveAllListeners();
        lYesButton.onClick.AddListener(iCallBackYes);
        lYesButton.onClick.AddListener(ClosePopup);
        

        Button lNoButton = GameObject.Find("Popup_YesNoIcon/YesNo/Button_No").GetComponent<Button>();
        //Bug 7: MG
        lNoButton.onClick.RemoveAllListeners();
        lNoButton.onClick.AddListener(ClosePopup);

        if(string.IsNullOrEmpty(iIconName))
            iIconName = "Locked";

        Image lIcon = GameObject.Find("Popup_YesNoIcon/Top_Icon/Icon").GetComponent<Image>();
        lIcon.sprite = poolManager.GetSprite(iIconName);

        // Open the window.
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Open a popup to display a message with an icon.
    /// </summary>
    /// <param name="iMessage">The message to display.</param>
    /// <param name="iIconName">The icon to display.</param>
    public void OpenDisplayIcon(string iMessage, string iIconName = "")
    {
        // Reset the content of the Popup window.
        ResetUI();
        popupDisplayIcon.SetActive(true);

        // Display the text and the icon.
        Text lQuestion = GameObject.Find("Popup_DisplayIcon/Text").GetComponent<Text>();
        lQuestion.text = iMessage;

        if (string.IsNullOrEmpty(iIconName))
            iIconName = "Locked";

        Image lIcon = GameObject.Find("Popup_DisplayIcon/Top_Icon/Icon").GetComponent<Image>();
        lIcon.sprite = poolManager.GetSprite(iIconName);

        // Open the window.
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Open a popup to display a QR code.
    /// </summary>
    /// <param name="iCode">The content of the QR code.</param>
	public void OpenShowQrCode(string iCode = "12-34-56-78")
    {
        // Reset the content of the Popup window.
        ResetUI();
        // Create the QR code from the content and display it.
		qrCodeManager.CreateQrCode(iCode);
        popupShowQrCode.SetActive(true);
        // Open the window.
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Open a popup to read the content of a QR code.
    /// </summary>
    public void OpenReadQrCode()
    {
        // Reset the content of the Popup window.
        ResetUI();
        // Open a QR code reader.
        qrCodeManager.SwitchQRCodeReader();
        popupReadQrCode.SetActive(true);
        // Open the window.
        animator.SetTrigger("Open");
    }

    /// <summary>
    /// Close the QR code reader.
    /// </summary>
    public void CloseReadQrCode()
    {
        qrCodeManager.SwitchQRCodeReader();
        popupReadQrCode.SetActive(false);
        animator.SetTrigger("Close");
    }

    /// <summary>
    /// Close the popup window.
    /// </summary>
    public void ClosePopup()
    {
        animator.SetTrigger("Close");
    }

    /// <summary>
    /// Reset which popup is currently displayed.
    /// </summary>
    private void ResetUI()
    {
        popupWindow.SetActive(false);
        popupYesNo.SetActive(false);
		popupYesNoIcon.SetActive(false);
        popupShowQrCode.SetActive(false);
        popupReadQrCode.SetActive(false);
		popupDisplayIcon.SetActive(false);
    }

    /// <summary>
    /// Destroy the content of the window that was generated.
    /// </summary>
    private void ResetWindowUI()
    {
        GameObject lContent = GameObject.Find("PopUp_Window/Window/Content");

        if (lContent.transform.childCount == 0)
            return;

        foreach (Transform lChild in lContent.transform) {
            GameObject.Destroy(lChild.gameObject);
        }
    }

    /// <summary>
    /// Display and indicator according to the level of a signal.
    /// </summary>
    /// <param name="iIndicator">The indicator level between 0 (min) and 1 (max).</param>
    /// <returns>The name of the sprite to display.</returns>
    private string SignalLevel(float iIndicator)
    {
        string lSignalSprite = "";
        if (iIndicator > 0.8F)
            lSignalSprite = "Network4";
        else if (iIndicator > 0.6F)
            lSignalSprite = "Network3";
        else if (iIndicator > 0.4F)
            lSignalSprite = "Network2";
        else if (iIndicator > 0.2F)
            lSignalSprite = "Network1";
        else
            lSignalSprite = "Network0";

        return lSignalSprite;
    }

    /// <summary>
    /// Coroutine to update the WebRTC information:
    /// Local connection intensity, remote connection instensity, device performance level.
    /// </summary>
    /// <returns>An enumerator.</returns>
    private IEnumerator UpdateWebRTCInfos()
    {
        // Get the images of the different signals.
        Image lLocal = GameObject.Find("LocalLevel").GetComponentsInChildren<Image>()[1];
        Image lRemote = GameObject.Find("RemoteLevel").GetComponentsInChildren<Image>()[1];
        Image lDevice = GameObject.Find("DeviceLevel").GetComponentsInChildren<Image>()[1];

        while (true)
        {
            //Debug.Log("RTC INFOS " + webRTC.ConnectionInfo);
            string[] lInfos = webRTC.ConnectionInfo.Split('|');
            lLocal.sprite = poolManager.GetSprite(SignalLevel(float.Parse(lInfos[0])));
            lRemote.sprite = poolManager.GetSprite(SignalLevel(float.Parse(lInfos[1])));
            lDevice.sprite = poolManager.GetSprite(SignalLevel(float.Parse(lInfos[2])));

            yield return new WaitForSeconds(1F);
        }
    }

    /// <summary>
    /// Close the WebRTC information popup.
    /// </summary>
    private void CloseWebRTCInfos()
    {
        StopCoroutine(UpdateWebRTCInfos());
        ClosePopup();
    }
}
