using UnityEngine;

public class LaunchTelepresence : MonoBehaviour
{ 
    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private OTONetwork oTONetwork;

    //[SerializeField]
    //private GameObject telepresenceGUI;
    
    void Start()
    {

    }

    public void ConnectToBuddy()
    {
        mobileServer.OnConnectionEstablished += StartTelepresence;
        mobileServer.StartTelepresence();
    }

    private void StartTelepresence()
    {
        mobileServer.gameObject.SetActive(false);
        oTONetwork.gameObject.SetActive(true);
        //telepresenceGUI.SetActive(true);
    }
}
