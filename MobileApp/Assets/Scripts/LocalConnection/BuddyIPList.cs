using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the displayed list of Buddies the user can connect to
/// </summary>
public class BuddyIPList : MonoBehaviour
{
    [SerializeField]
    private GameObject searchingPrefab;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private Text prefabName;

    [SerializeField]
    private Text prefabID;

    [SerializeField]
    private Transform parentTransform;

    [SerializeField]
    private AppMobileServer mobileServer;

    [SerializeField]
    private DBManager buddyDB;

    private int mBuddyNb = 0;
    private float mTime = 20f;
    private List<string> mIPList = new List<string>();

    void Start()
    {
        mobileServer.OnNewBudyConnected += UpdateBuddyList;
        mobileServer.OnBuddyDisconnected += RemoveBuddyFromList;
    }

    void OnEnable()
    {
        mobileServer.gameObject.SetActive(true);
        UpdateIPList();
    }

    void OnDisable()
    {
        mTime = 0f;
    }

    void FixedUpdate()
    {
        //Refresh list every 20 seconds
        if (!isActiveAndEnabled)
            return;

        mTime -= Time.deltaTime;
        if(mTime <= 0f) {
            mTime = 20f;
            UpdateIPList();
        }
    }

    public void ClearList()
    {
        mIPList.Clear();
    }

    private void UpdateBuddyList(string iNewBuddyIP)
    {
        //Check if new Buddy is not already contained in the connected list
        string[] lTab = iNewBuddyIP.Split(':');
        string lBuddyIP = lTab[lTab.Length - 1];

        if (mIPList.Contains(lBuddyIP))
            return;

        mIPList.Add(lBuddyIP);
        prefabName.text = "Buddy " + mBuddyNb;
        prefabID.text = "IP " + iNewBuddyIP;
        
        //When instantiating the prefab, make sure to give it a unit localScale
        GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
        lClone.transform.SetParent(parentTransform);
        //The following makes sure that the newly connected Buddy is not under the "searching" icon
        lClone.transform.SetSiblingIndex(parentTransform.childCount - 2);
        lClone.transform.localScale = Vector3.one;
        mBuddyNb++;
    }

    private void RemoveBuddyFromList(string iDisconnectedBuddy)
    {
        //Self-explanatory
        string[] lTab = iDisconnectedBuddy.Split(':');
        string lBuddyIP = lTab[lTab.Length - 1];
        mIPList.Remove(lBuddyIP);
        UpdateIPList();
    }

    public void UpdateIPList()
    {
        //Remove all present displayed robots
        foreach (Transform lChild in parentTransform)
            GameObject.Destroy(lChild.gameObject);

        //Add Buddies from all different sources. 
        //NOTE : WebRTC Buddies and Database ones will be merged
        AddBuddyFromDB();
        AddWebRTCBuddy();
        mIPList.Clear();
        mIPList = mobileServer.GetBuddyConnectedList();

        //Instiate the prefab for each found Buddy in the displayed list
        foreach (string lIP in mIPList) {
            prefabName.text = "Buddy " + mBuddyNb;
            prefabID.text = "IP " + lIP;

            //When instantiating the prefab, make sure to give it a unit localScale
            GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            lClone.transform.SetParent(parentTransform);
            lClone.transform.localScale = Vector3.one;
        }
        //Add the searching logo to the list for better visuals
        GameObject lSearching = Instantiate(searchingPrefab, transform.position, transform.rotation) as GameObject;
        lSearching.transform.SetParent(parentTransform);
        lSearching.transform.localScale = Vector3.one;
    }

    private void AddBuddyFromDB()
    {
        //Retrieve the list from Database source and add it to the displayed list
        string[] lBuddyList = buddyDB.BuddyList.Split('\n');

        for (int i = 0; i < lBuddyList.Length - 1; i++)
        {
            Debug.Log("Buddy IDs " + lBuddyList[i]);
            string[] lBuddyIDs = lBuddyList[i].Split('|');
            prefabName.text = "Buddy " + lBuddyIDs[0];
            prefabID.text = "ID " + lBuddyIDs[1];

            GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            lClone.transform.SetParent(parentTransform);
            lClone.transform.localScale = Vector3.one;
        }
    }

    private void AddWebRTCBuddy()
    {
        //For now, it's just a hard coded Buddy that the user can connect to
        prefabName.text = "User2";
        prefabID.text = "WebRTC ID";
        GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
        lClone.transform.SetParent(parentTransform);
        lClone.transform.localScale = Vector3.one;
    }

    public void SearchForBuddy(Text iSearch)
    {
        //We display here only the Buddy that contain the keyword 'iSearch'
        //If the string is null, we show all the Buddy
        string lSearch = iSearch.text;
        Debug.Log("Searching for " + lSearch);
        if(string.IsNullOrEmpty(lSearch)) {
            foreach(Transform lChild in parentTransform) {
                lChild.gameObject.SetActive(true);
            }
        }
        //Else, we display only selected Buddies
        else {
            foreach(Transform lChild in parentTransform) {
                Text lBuddyName = lChild.GetComponentInChildren<Text>();
                Debug.Log("Buddy has name " + lBuddyName.text);

                if (!lBuddyName.text.Contains(lSearch))
                    lChild.gameObject.SetActive(false);
            }
        }
    }
}
