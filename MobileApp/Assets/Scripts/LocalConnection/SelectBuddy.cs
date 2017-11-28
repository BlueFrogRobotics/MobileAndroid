using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Saves which Buddy has been selected from the list as well as the connection type
/// </summary>
public class SelectBuddy : MonoBehaviour
{
    public enum RemoteType { LOCAL, WEBRTC, DEMO };

    public RemoteType Remote { get { return mRemote; } set { mRemote = value; } }

    public static string BuddyName { get; /*private*/ set; }
	public static string BuddyID { get; /*private*/ set; }

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
	private BuddyStatus buddyStatus;

	private RemoteType mRemote;
	private bool mBuddyOnline = false;

    public void BuddySelected()
    {
        bool lFound = false;

        Transform lBuddyList = GameObject.Find("Content_Bottom/ScrollView/Viewport").transform;

        //Look through the list to see which robot has been selected
        foreach (Transform mBuddy in lBuddyList) {
            BuddyContactHandler lToggle = mBuddy.gameObject.GetComponentInChildren<BuddyContactHandler>();

            //The robot whose Toggle button has been triggered is the one to connect to
            if (lToggle != null && lToggle.IsOn) {
                string[] lBuddySplit = mBuddy.GetChild(6).GetComponent<Text>().text.Split(' ');
                string lBuddyType = lBuddySplit[0];
                string lBuddyID = lBuddySplit[1];
                string lBuddyName = mBuddy.GetChild(5).GetComponent<Text>().text;
                BuddyName = lBuddyName;
                BuddyID = lBuddyID;
				mBuddyOnline = buddyStatus.BuddyOnline(BuddyID);

                //Check wether it's a WebRTC or local connection
                if(lBuddyName.Contains("DEMO")) {
                    mRemote = RemoteType.DEMO;
                } else if(lBuddyType.Contains("IP")) {
                    mRemote = RemoteType.LOCAL;
                    oTONetwork.IP = lBuddyID;
                    mobileServer.BuddyIP = lBuddyID;
                    Debug.Log("ip " + lBuddyID + "!");
                    
                } else {
                    mRemote = RemoteType.WEBRTC;
                    webRTC.GetComponent<Webrtc>().RemoteID = lBuddyID;
                    webRTC.SetActive(true);
                }
                
                lFound = true;
                notifications.SendNotification("Connected to Buddy " + lBuddyID);
            }            
        }

		if (lFound) {
			if (mBuddyOnline) {
				GameObject.Find ("Content_Bottom/ScrollView/Viewport").GetComponent<BuddyIPList> ().enabled = false;
				GameObject.Find ("MenuManager").GetComponent<GoBack> ().GoConnectedMenu ();
			} else {
				GameObject.Find ("PopUps").GetComponent<PopupHandler> ().OpenDisplayIcon ("Buddy est actuellement hors ligne", "Warning");
			}
		}
    }
}
