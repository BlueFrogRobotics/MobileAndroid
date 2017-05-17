using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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

    [SerializeField]
    private Webrtc webRTC;

    public void OpenWindow()
    {
        ResetUI();
        popupWindow.SetActive(true);
        animator.SetTrigger("Open");
    }

    public void DisplayRTCNetworkInfo()
    {
        ResetUI();
        popupWindow.SetActive(true);
        ResetWindowUI();

        GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
        lButton.GetComponent<Button>().onClick.AddListener(CloseWebRTCInfos);

        GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>().text = "Network Status";
        poolManager.fSimple_Text("PopUp_Window/Window/Content", "Connection status local", true);
        GameObject lLocalLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lLocalLevel.name = "LocalLevel";

        poolManager.fSimple_Text("PopUp_Window/Window/Content", "Connection status remote", true);
        GameObject lRemoteLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lRemoteLevel.name = "RemoteLevel";

        poolManager.fSimple_Text("PopUp_Window/Window/Content", "Device performance", true);
        GameObject lDeviceLevel = poolManager.fButton_R("PopUp_Window/Window/Content", "Network0", null);
        lDeviceLevel.name = "DeviceLevel";

        StartCoroutine(UpdateWebRTCInfos());

        animator.SetTrigger("Open");
    }

    public void DisplayError(string iTitle, string iMessage)
	{
		ResetUI();
		popupWindow.SetActive(true);
        ResetWindowUI();

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
        ResetWindowUI();
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

    public void DeleteAccountPopup(UnityAction iCallback)
    {
        ResetUI();
        popupWindow.SetActive(true);
        ResetWindowUI();
        //Debug.Log ("Popup Window " + popupWindow.activeInHierarchy + " / " + popupWindow.activeSelf);

        Text lTitle = GameObject.Find("PopUp_Window/Window/Top_UI/SimpleText").GetComponent<Text>();
        lTitle.text = "DELETE ACCOUNT - CONFIRMATION";

        GameObject lButton = GameObject.Find("PopUp_Window/Window/Top_UI/Button");
        lButton.GetComponent<Button>().onClick.AddListener(ClosePopup);

        poolManager.fSimple_Text("PopUp_Window/Window/Content", "Enter this account's password", true);

        GameObject lInputField = poolManager.fTextField_Icon("PopUp_Window/Window/Content", "Password", "", null, null, null, null);
        lInputField.name = "Popup_PasswordConfirm";
        lInputField.GetComponent<InputField>().inputType = InputField.InputType.Password;

        poolManager.fButton_Square("PopUp_Window/Window/Content", "CONFIRM", "", new List<UnityAction>() { iCallback });

        animator.SetTrigger("Open");
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

    public void CloseReadQrCode()
    {
        qrCodeManager.SwitchQRCodeReader();
        popupReadQrCode.SetActive(false);
        animator.SetTrigger("Close");
    }

    public void ClosePopup()
    {
        animator.SetTrigger("Close");
    }

    private void ResetUI()
    {
        popupWindow.SetActive(false);
        popupYesNo.SetActive(false);
        popupShowQrCode.SetActive(false);
        popupReadQrCode.SetActive(false);
    }

    private void ResetWindowUI()
    {
        GameObject lContent = GameObject.Find("PopUp_Window/Window/Content");

        if (lContent.transform.childCount == 0)
            return;

        foreach (Transform lChild in lContent.transform) {
            GameObject.Destroy(lChild.gameObject);
        }
    }

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

    private IEnumerator UpdateWebRTCInfos()
    {
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

    private void CloseWebRTCInfos()
    {
        StopCoroutine(UpdateWebRTCInfos());
        ClosePopup();
    }
}
