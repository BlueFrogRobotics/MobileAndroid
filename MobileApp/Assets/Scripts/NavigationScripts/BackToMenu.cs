using UnityEngine;
using UnityEngine.UI;

public class BackToMenu : MonoBehaviour
{
    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private OTONetwork oto;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private Animator mAnimator;

    [SerializeField]
    private RawImage mIncomingVideo;

    public void GoBackToMenu()
    {
        //mIncomingVideo.gameObject.SetActive(false);
        oto.Disconnect();
        oto.gameObject.SetActive(false);
        mobileServer.gameObject.SetActive(true);
        webRTC.HangUp();
        mAnimator.SetTrigger("GoConnectBuddy");
        mAnimator.SetTrigger("EndScene");
    } 
}
