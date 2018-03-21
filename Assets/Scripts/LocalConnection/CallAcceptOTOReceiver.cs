using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to receive an acknowledge of remote robot. Remote can either accept or refuse the incoming "call"
/// i.e. the remote control request
/// </summary>
public class CallAcceptOTOReceiver : OTONetReceiver
{
    public enum CallStatus { WAITING, ACCEPTED, REJECTED };
    public CallStatus Status { get; private set; }

    [SerializeField]
    private BackToMenu mBackToMenu;

    [SerializeField]
    private RawImage mIncomingVideo;

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
        Status = CallStatus.WAITING;
        DisableUI();
    }

    public void DisableUI()
    {
        mIncomingVideo.gameObject.SetActive(false);
        //mWaitingText.gameObject.SetActive(true);
    }

    private void CloseCall()
    {
        Status = CallStatus.REJECTED;
        Debug.Log("Received CloseCall");
        DisableUI();
        mBackToMenu.GoBackToMenu();
    }

    private void OpenCall()
    {
        Status = CallStatus.ACCEPTED;
        Debug.Log("Received AcceptCall");
        mIncomingVideo.gameObject.SetActive(true);
        //mWaitingText.gameObject.SetActive(false);
    }
}
