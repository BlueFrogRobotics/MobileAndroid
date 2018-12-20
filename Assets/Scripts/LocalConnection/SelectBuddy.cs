using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Saves which Buddy has been selected from the list as well as the connection type.
/// </summary>
public class SelectBuddy : MonoBehaviour
{
    public enum RemoteType { LOCAL, WEBRTC, DEMO };

    public RemoteType Remote { get { return mRemote; } set { mRemote = value; } }

    private static string sBuddyID;

    public static string BuddyName { get; /*private*/ set; }
    public static string BuddyID { get { return sBuddyID; } set { sBuddyID = value.ToUpper(); } }

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

    /// <summary>
    /// Confirm the selection of a Buddy and connect to it.
    /// This is called in the selection screen when the "Select" button is pressed.
    /// </summary>
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
                mBuddyOnline = buddyStatus.BuddyAccess(BuddyID, false);

                //Check wether it's a WebRTC or local connection
                if (lBuddyName.Contains("DEMO")) {
                    mRemote = RemoteType.DEMO;
                } else if (lBuddyType.Contains("IP")) {
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

        //Go to the "connected to Buddy" menu if Buddy was found, else display an error message.
        if (lFound) {
            if (mBuddyOnline) {
                GameObject.Find("Content_Bottom/ScrollView/Viewport").GetComponent<BuddyIPList>().enabled = false;
                GameObject.Find("MenuManager").GetComponent<GoBack>().GoConnectedMenu();
            } else {
                GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Buddy est actuellement hors ligne", "NoResponse");
            }
        }
    }

    public bool BuddyAccess()
    {
        return buddyStatus.BuddyAccess(BuddyID, true);
    }

    /// <summary>
    /// Unlink Buddy from the active account.
    /// </summary>
    public void DeleteBuddy()
    {
        Debug.LogWarning("CLIC ON DELETE");


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
                lFound = true;
                notifications.SendNotification("Buddy :" + BuddyName + " : " + BuddyID + " deleted.");
            }
        }

        //Go to the "connected to Buddy" menu if Buddy was found, else display an error message.
        if (lFound) {
            Debug.LogWarning("FOUND!");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenYesNoIcon("Voulez-vous supprimer ce Buddy de la liste ?", onDeleteBuddyConfirmed, "Warning");
        } else {
            Debug.LogWarning("Please select a Buddy.");
            GameObject.Find("PopUps").GetComponent<PopupHandler>().OpenDisplayIcon("Sélectionnez un Buddy à supprimer.", "Warning");
        }
    }

    /// <summary>
    /// Callback to unlink Buddy from the account through a database request.
    /// </summary>
	private void onDeleteBuddyConfirmed()
    {
        Debug.LogWarning("DELETION CONFIRMED: " + BuddyName + " : " + BuddyID);
        GameObject.Find("DBManager").GetComponent<DBManager>().StartRemoveBuddyFromUser(SelectBuddy.BuddyID, null);
        StartCoroutine(RefreshBuddyList());
    }

    private IEnumerator RefreshBuddyList()
    {
        Debug.LogWarning("REFRESH CALLBACK ...");
        yield return new WaitForSeconds(5F);
        Debug.LogWarning("REFRESHING");
        BuddyIPList lIPList = GameObject.Find("Content_Bottom/ScrollView/Viewport").GetComponent<BuddyIPList>();
        if (lIPList) {
            lIPList.CreateListDisplay();
            Debug.LogWarning("REFRESHED");
        } else
            Debug.LogWarning("IPList not found");
    }
}
