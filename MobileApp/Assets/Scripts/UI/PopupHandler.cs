using UnityEngine;
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

    public void OpenWindow()
    {
        ResetUI();
        popupWindow.SetActive(true);
        animator.SetTrigger("Open");
    }

    public void OpenYesNo(string iQuestion = "")
    {
        ResetUI();
        popupYesNo.SetActive(true);
        animator.SetTrigger("Open");
    }

    public void OpenShowQrCode()
    {
        ResetUI();
        qrCodeManager.CreateQrCode("12-34-56-78");
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
