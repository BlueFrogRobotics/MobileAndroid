using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Go back from the Remote Control menu to Connected menu. Manage all networks related to it
/// </summary>
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

    public void GoBackToMenu()
    {
        //Reactivate networks depending on which type of remote connection was used
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
