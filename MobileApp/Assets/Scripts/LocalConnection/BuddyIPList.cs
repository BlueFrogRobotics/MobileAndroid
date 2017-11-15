using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the displayed list of Buddies the user can connect to
/// </summary>
public class BuddyIPList : MonoBehaviour
{
    [SerializeField]
    private PoolManager mPoolManager;

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
		StartCoroutine(CheckBuddiesStatus());
		CreateListDisplay();
    }

    void OnDisable()
    {
		mTime = 0f;
		StopCoroutine(CheckBuddiesStatus());
    }

    void FixedUpdate()
    {
        //Refresh list every 20 seconds
        if (!isActiveAndEnabled)
            return;

        mTime -= Time.deltaTime;
        if(mTime <= 0f) {
            mTime = 20f;
            //CreateListDisplay();
            UpdateListDisplay();
        }
    }

    public void ClearList()
    {
        mIPList.Clear();
    }

	private IEnumerator CheckBuddiesStatus()
	{
        while (true)
		{
			if (!buddyDB.IsBuddiesListEmpty ()) {
				foreach (BuddyDB lBuddy in buddyDB.BuddiesList) {
					Int32 currTimestamp = (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
					//Debug.Log ("Buddy " + lBuddy.name + " " + lBuddy.status);
					if (currTimestamp - lBuddy.timestamp > 40) {
						lBuddy.status = "offline";
					}
				}
			}

			yield return new WaitForSeconds(10F);
		}
	}

    private void UpdateBuddyList(string iNewBuddyIP)
    {
        //Check if new Buddy is not already contained in the connected list
        string[] lTab = iNewBuddyIP.Split(':');
        string lBuddyIP = lTab[lTab.Length - 1];

        if (mIPList.Contains(lBuddyIP) || this.enabled==false)
            return;

		GameObject lBuddyLocal = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "New Buddy !", iNewBuddyIP, "", true, true, "online", null);
        lBuddyLocal.transform.SetSiblingIndex(parentTransform.childCount - 2);
        LoadingUI.AddObject(lBuddyLocal);
        mBuddyNb++;
    }

    private void RemoveBuddyFromList(string iDisconnectedBuddy)
    {
        //Self-explanatory
        string[] lTab = iDisconnectedBuddy.Split(':');
        string lBuddyIP = lTab[lTab.Length - 1];
        mIPList.Remove(lBuddyIP);

        GameObject lList = GameObject.Find("Content_Bottom/ScrollView/Viewport");
        bool lLocal = false;
        foreach(Transform lChild in lList.transform)
        {
            if (!lLocal || lChild.gameObject.name != "LocalSeparator")
                continue;
            else if (lChild.gameObject.name == "LocalSeparator")
                lLocal = true;
            else if(lLocal && lChild.GetComponentsInChildren<Text>()[1].text.Contains(lBuddyIP)) {
                GameObject.Destroy(lChild.gameObject);
                break;
            }
        }
        //CreateListDisplay();
    }

    public void CreateListDisplay()
    {
        //Remove all present displayed robots
        foreach (Transform lChild in parentTransform)
            GameObject.Destroy(lChild.gameObject);

        //Add Buddies from all different sources. 
        //NOTE : WebRTC Buddies and Database ones will be merged
        AddBuddyFromDB();
        AddLocalBuddy();
        //Add the searching logo to the list for better visuals
        GameObject lSearching = mPoolManager.fSearching("Content_Bottom/ScrollView/Viewport");
        lSearching.name = "SearchingContact";
        LoadingUI.AddObject(lSearching);
    }

    private void UpdateListDisplay()
    {
        GameObject lViewport = GameObject.Find("Content_Bottom/ScrollView/Viewport");

        if(lViewport.transform.childCount == 0) {
            CreateListDisplay();
        } else {
            bool lDistantPart = true;
            foreach (Transform lChild in parentTransform) {
                if (lChild.gameObject.name == "DistantSeparator")
                    continue;
                else if (lChild.gameObject.name == "LocalSeparator") {
                    lDistantPart = false;
                    continue;
                } else {

                }
            }
        }
    }

    private void AddBuddyFromDB()
    {
        //Retrieve the list from Database source and add it to the displayed list
        Debug.Log("Adding DB Buddy");
        GameObject lDistantSeparator = mPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "YOUR BUDDY CONTACT(S)");
        lDistantSeparator.name = "DistantSeparator";
        LoadingUI.AddObject(lDistantSeparator);

        if (buddyDB.CurrentUser.LastName.Contains("DEMO")) {
			GameObject lBuddyDemo = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy DEMO", "DEMO", "", false, true, "online", null);
            LoadingUI.AddObject(lBuddyDemo);
            return;
        }

		foreach (BuddyDB lBuddy in buddyDB.BuddiesList) {
            //Debug.Log("************");
            //Debug.Log("Buddy Name = " + lBuddy.name);
            //Debug.Log("Buddy State = " + lBuddy.status);
            GameObject lBuddyDB = mPoolManager.fBuddy_Contact ("Content_Bottom/ScrollView/Viewport", lBuddy.name, lBuddy.ID, "", false, true, lBuddy.status, null);
			LoadingUI.AddObject(lBuddyDB);
		}
    }
    
    private void AddLocalBuddy()
    {
        GameObject lLocalSeparator = mPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "CONNECTED ON YOUR WIFI");
        lLocalSeparator.name = "LocalSeparator";
        LoadingUI.AddObject(lLocalSeparator);
        mIPList.Clear();
        mIPList = mobileServer.GetBuddyConnectedList();

		if(buddyDB.CurrentUser.LastName.Contains("DEMO")) {
			GameObject lBuddyDemo = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy DEMO Local", "DEMO", "", true, true, "online", null);
			LoadingUI.AddObject(lBuddyDemo);
		}

        //Instiate the prefab for each found Buddy in the displayed list
        int lCount = 0;
        foreach (string lIP in mIPList)
        {
			GameObject lBuddyLocal = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy " + lCount, lIP, "", true, true, "online", null);
            LoadingUI.AddObject(lBuddyLocal);
            lCount++;
        }
    }

    public void SearchForBuddy(string iSearch)
    {
        //We display here only the Buddy that contain the keyword 'iSearch'
        //If the string is null, we show all the Buddy
        Debug.Log("Searching for " + iSearch);
        if(string.IsNullOrEmpty(iSearch)) {
            foreach(Transform lChild in parentTransform) {
                lChild.gameObject.SetActive(true);
            }
        }
        //Else, we display only selected Buddies
        else {
            string lLowerSearch = iSearch.ToLower();
            foreach (Transform lChild in parentTransform) {
                Text lBuddyName = lChild.GetComponentInChildren<Text>();
                Debug.Log("Buddy has name " + lBuddyName.text);

                if (!lBuddyName.text.ToLower().Contains(lLowerSearch) && lChild.gameObject.name != "LocalSeparator" && lChild.gameObject.name != "DistantSeparator")
                    lChild.gameObject.SetActive(false);
            }
        }
    }
}
