using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to receive an acknowledge of remote robot. Remote can either accept or refuse the incoming "call"
/// i.e. the remote control request
/// </summary>
public class CallAcceptOTOReceiver : OTONetReceiver
{
    [SerializeField]
    private BackToMenu mBackToMenu;

    [SerializeField]
    private RawImage mIncomingVideo;

    //[SerializeField]
    //private Text mWaitingText;

    [SerializeField]
    private OTONetwork oTO;

    public override void ReceiveData(byte[] iData, int iNData)
    {
        switch(iData[0])
        {
            case 0:
                CloseCall();
                break;

            case 1:
                OpenCall();
                break;
        }
    }

    void OnEnable()
    {
        DisableUI();
    }

    public void DisableUI()
    {
        mIncomingVideo.gameObject.SetActive(false);
        //mWaitingText.gameObject.SetActive(true);
    }

    private void CloseCall()
    {
        Debug.Log("Received CloseCall");
        DisableUI();
        mBackToMenu.GoBackToMenu();
    }

    private void OpenCall()
    {
        Debug.Log("Received AcceptCall");
        mIncomingVideo.gameObject.SetActive(true);
        //mWaitingText.gameObject.SetActive(false);
    }
}
