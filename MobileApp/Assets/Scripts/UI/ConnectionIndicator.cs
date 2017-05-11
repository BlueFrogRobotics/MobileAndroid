using UnityEngine;

/// <summary>
/// Manages the connection indicator UI
/// </summary>
public class ConnectionIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject redImage;

    [SerializeField]
    private GameObject greenImage;

    [SerializeField]
    private OTONetwork oto;

    [SerializeField]
    private Webrtc webRTC;

    void Update()
    {
        //Check which connection type is used
        //and display the red or green image matching the remote status (connected or disconnected)
        if(oto.isActiveAndEnabled) {
            if (oto.HasAPeer) {
                redImage.SetActive(false);
                greenImage.SetActive(true);
            } else {
                greenImage.SetActive(false);
                redImage.SetActive(true);
            }
        } else {
            if (webRTC.ConnectionState == Webrtc.CONNECTION.CONNECTING) {
                redImage.SetActive(false);
                greenImage.SetActive(true);
            }
            else if (webRTC.ConnectionState == Webrtc.CONNECTION.DISCONNECTING) {
                greenImage.SetActive(false);
                redImage.SetActive(true);
            }
        }
    }

    public void CloseConnection()
    {
        greenImage.SetActive(false);
        redImage.SetActive(true);
    }
}