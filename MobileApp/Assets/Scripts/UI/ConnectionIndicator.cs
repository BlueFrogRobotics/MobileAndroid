using UnityEngine;

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
}