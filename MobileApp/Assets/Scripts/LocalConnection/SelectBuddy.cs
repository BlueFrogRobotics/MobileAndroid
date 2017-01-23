using UnityEngine;
using UnityEngine.UI;

public class SelectBuddy : MonoBehaviour
{
    public enum RemoteType { LOCAL, WEBRTC };

    public RemoteType Remote { get; set; }

    [SerializeField]
    private Transform buddyList;

    [SerializeField]
    private OTONetwork oTONetwork;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private GameObject webRTC;

    [SerializeField]
    private Animator canvasAppAnimator;

    [SerializeField]
    private NotificationSender notifications;

    [SerializeField]
    private Text buddyConnectedName;

    [SerializeField]
    private Text buddyConnectedID;

    public void BuddySelected()
    {
        bool lFound = false;

        foreach (Transform mBuddy in buddyList) {
            Toggle lToggle = mBuddy.gameObject.GetComponent<Toggle>();

            if (lToggle != null && lToggle.isOn) {
                string[] lBuddySplit = mBuddy.GetChild(3).GetComponent<Text>().text.Split(' ');
                string lBuddyIP = lBuddySplit[1];
                string lBuddyName = mBuddy.GetChild(2).GetComponent<Text>().text;

                if(lBuddyName.Contains("User2")) {
                    Remote = RemoteType.WEBRTC;
                    webRTC.SetActive(true);

                } else {
                    Remote = RemoteType.LOCAL;
                    oTONetwork.IP = lBuddyIP;
                    mobileServer.BuddyIP = lBuddyIP;
                    Debug.Log("ip " + lBuddyIP + "!");
                }
                
                lFound = true;
                notifications.SendNotification("Connected to Buddy " + lBuddyIP);
                ChangeConnectedBuddyName(lBuddyName, lBuddyIP);
            }            
        }

        if(lFound) {
            canvasAppAnimator.SetTrigger("GoConnectBuddy");
            canvasAppAnimator.SetTrigger("EndScene");
        }

    }

    private void ChangeConnectedBuddyName(string iName, string iID)
    {
        buddyConnectedName.text = iName;
        buddyConnectedID.text = iID;
    }
}
