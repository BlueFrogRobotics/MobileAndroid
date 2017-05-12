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

    [SerializeField]
    private ConnectionIndicator indicator;

    [SerializeField]
    private GameObject leftSensor;

    [SerializeField]
    private GameObject middleSensor;

    [SerializeField]
    private GameObject rightSensor;

    [SerializeField]
    private GameObject backSensor;

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
            //webRTC.StopWebRTC();
        }

        leftSensor.GetComponent<ObstacleManager>().lvl = 0;
        middleSensor.GetComponent<ObstacleManager>().lvl = 0;
        rightSensor.GetComponent<ObstacleManager>().lvl = 0;
        backSensor.GetComponent<ObstacleManager>().lvl = 0;

        indicator.CloseConnection();

        mAnimator.SetTrigger("GoConnectBuddy");
        mAnimator.SetTrigger("EndScene");
    } 
}
