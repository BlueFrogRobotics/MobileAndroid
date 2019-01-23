using UnityEngine;

/// <summary>
/// Manages the way the remote control request is done.
/// </summary>
public class LaunchTelepresence : MonoBehaviour
{
    [SerializeField]
    private SelectBuddy selectBuddy;

    [SerializeField]
    private GameObject remoteControlRTC;

    [SerializeField]
    private Webrtc webRTC;

    [SerializeField]
    private BackgroundListener rtcListener;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private OTONetwork oTONetwork;
    
    /// <summary>
    /// Called when a remote connection is requested by the user.
    /// </summary>
    /// <param name="iMode">The remote type for a WebRTC type: either classic, Wizard-of-Oz, or Automatic distant control.</param>
    public void ConnectToBuddy(int iMode)
    {
        //Check what if selected Buddy is controllable via Local or WebRTC method
        Debug.Log("Remote type is " + selectBuddy.Remote);
        if (selectBuddy.Remote == SelectBuddy.RemoteType.LOCAL) {
            //Add callback to start the UI and remote control networking after
            //the other networks are shut down
            mobileServer.OnConnectionEstablished += StartTelepresence;
            mobileServer.StartTelepresence();
            Debug.Log("Starting local control");
        } else if (selectBuddy.Remote == SelectBuddy.RemoteType.WEBRTC) {
            Debug.Log("Starting distant control with remote " + webRTC.RemoteID);
            remoteControlRTC.SetActive(true);
            rtcListener.PublishConnectionRequest(webRTC.ID,webRTC.RemoteID, iMode);
        }
    }

    /// <summary>
    /// Start local remote-control.
    /// </summary>
    private void StartTelepresence()
    {
        mobileServer.gameObject.SetActive(false);
        oTONetwork.gameObject.SetActive(true);
    }
}
