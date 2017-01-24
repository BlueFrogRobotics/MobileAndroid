using UnityEngine;
using UnityEngine.UI;

public class BackToMenu : MonoBehaviour
{
    [SerializeField]
    private SelectBuddy selectBuddy;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private OTONetwork oto;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private GameObject remoteControlRTC;

    [SerializeField]
    private Animator mAnimator;

    [SerializeField]
    private RawImage mIncomingVideo;

    public void GoBackToMenu()
    {
        //mIncomingVideo.gameObject.SetActive(false);
        if(selectBuddy.Remote == SelectBuddy.RemoteType.LOCAL) {
            oto.Disconnect();
            oto.gameObject.SetActive(false);
            mobileServer.gameObject.SetActive(true);

        } else if (selectBuddy.Remote == SelectBuddy.RemoteType.WEBRTC) {
            webRTC.HangUp();
            remoteControlRTC.SetActive(false);
        }
        mAnimator.SetTrigger("GoConnectBuddy");
        mAnimator.SetTrigger("EndScene");
    } 
}
