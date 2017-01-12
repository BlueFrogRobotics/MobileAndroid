using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuddyIPList : MonoBehaviour
{
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
        if (!isActiveAndEnabled)
            return;

        mTime -= Time.deltaTime;
        if(mTime <= 0f) {
            mTime = 20f;
            UpdateIPList();
        }
    }

    private void AddBuddyFromDB()
    {
        string[] lBuddyList = buddyDB.BuddyList.Split('\n');

        for (int i = 0; i < lBuddyList.Length - 1; i++) {
            Debug.Log("Buddy IDs " + lBuddyList[i]);
            string[] lBuddyIDs = lBuddyList[i].Split('|');
            prefabName.text = "Buddy " + lBuddyIDs[0];
            prefabID.text = "IP " + lBuddyIDs[1];

            GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            lClone.transform.SetParent(parentTransform);
            lClone.transform.localScale = Vector3.one;
        }
    }

    public void ClearList()
    {
        mIPList.Clear();
    }

    private void UpdateBuddyList(string iNewBuddyIP)
    {
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
        lClone.transform.localScale = Vector3.one;
        mBuddyNb++;
    }

    private void RemoveBuddyFromList(string iDisconnectedBuddy)
    {
        string[] lTab = iDisconnectedBuddy.Split(':');
        string lBuddyIP = lTab[lTab.Length - 1];
        mIPList.Remove(lBuddyIP);
        UpdateIPList();
    }

    public void UpdateIPList()
    {
        foreach (Transform lChild in parentTransform)
            GameObject.Destroy(lChild.gameObject);
        //AddBuddyFromDB();
        mIPList.Clear();
        mIPList = mobileServer.GetBuddyConnectedList();

        foreach (string lIP in mIPList) {
            prefabName.text = "Buddy " + mBuddyNb;
            prefabID.text = "IP " + lIP;

            //When instantiating the prefab, make sure to give it a unit localScale
            GameObject lClone = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            lClone.transform.SetParent(parentTransform);
            lClone.transform.localScale = Vector3.one;
        }
    }
}
