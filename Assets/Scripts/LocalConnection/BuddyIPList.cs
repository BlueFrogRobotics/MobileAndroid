using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the displayed list of Buddies the user can connect to.
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

    /// <summary>
    /// Check every 10s the status of all Buddies. 
    /// </summary>
    /// <returns>An enumerator.</returns>
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

    /// <summary>
    /// Check if the Buddy from which we received a message is already displayed on the list or not.
    /// </summary>
    /// <param name="iNewBuddyIP">The IP address of the Buddy that sent a message.</param>
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

    /// <summary>
    /// Remove a disconnected Buddy from the display list.
    /// </summary>
    /// <param name="iDisconnectedBuddy">The IP address of the Buddy that is now disconnected.</param>
    private void RemoveBuddyFromList(string iDisconnectedBuddy)
    {
        //Self-explanatory
		if(iDisconnectedBuddy.Length == 0) {
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
    }

    /// <summary>
    /// Display the list of all Buddies: local and Internet ones.
    /// </summary>
    public void CreateListDisplay()
    {
        //Remove all present displayed robots
        foreach (Transform lChild in parentTransform)
            GameObject.Destroy(lChild.gameObject);

        //Add Buddies from all different sources. 
        AddBuddyFromDB();
    }

    /// <summary>
    /// Create the displayed list if there is nothing.
    /// </summary>
    private void UpdateListDisplay()
    {
        GameObject lViewport = GameObject.Find("Content_Bottom/ScrollView/Viewport");

        if(lViewport.transform.childCount == 0) {
            CreateListDisplay();
        }
    }

    /// <summary>
    /// Display Buddies that are linked to the user account on the MYSQL Database.
    /// </summary>
    private void AddBuddyFromDB()
    {
        //Retrieve the list from Database source and add it to the displayed list
        Debug.Log("Adding DB Buddy");
        //Display a separation line for Online Buddies.
        GameObject lDistantSeparator = mPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "buddycontact");
        lDistantSeparator.name = "DistantSeparator";
        LoadingUI.AddObject(lDistantSeparator);

        //For demo purpose.
        if (buddyDB.CurrentUser.LastName.Contains("DEMO")) {
			GameObject lBuddyDemo = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy DEMO", "DEMO", "", false, true, "online", null);
            LoadingUI.AddObject(lBuddyDemo);
            return;
        }

        //Display all Buddies linked to the active account.
		foreach (BuddyDB lBuddy in buddyDB.BuddiesList) {
            GameObject lBuddyDB = mPoolManager.fBuddy_Contact ("Content_Bottom/ScrollView/Viewport", lBuddy.name, lBuddy.ID, "", false, true, lBuddy.status, null);
			LoadingUI.AddObject(lBuddyDB);
		}
    }
    
    /// <summary>
    /// Display Buddies that are connected locally to the phone server.
    /// </summary>
    private void AddLocalBuddy()
    {
        //Display a separation line for local Buddies.
        GameObject lLocalSeparator = mPoolManager.fBuddy_Separator("Content_Bottom/ScrollView/Viewport", "connectedonwifi");
        lLocalSeparator.name = "LocalSeparator";
        LoadingUI.AddObject(lLocalSeparator);
        mIPList.Clear();
        mIPList = mobileServer.GetBuddyConnectedList();

        //For demo purpose.
		if(buddyDB.CurrentUser.LastName.Contains("DEMO")) {
			GameObject lBuddyDemo = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy DEMO Local", "DEMO", "", true, true, "online", null);
			LoadingUI.AddObject(lBuddyDemo);
		}

        //Display all Buddies that are locally connected.
        int lCount = 0;
        foreach (string lIP in mIPList)
        {
			GameObject lBuddyLocal = mPoolManager.fBuddy_Contact("Content_Bottom/ScrollView/Viewport", "Buddy " + lCount, lIP, "", true, true, "online", null);
            LoadingUI.AddObject(lBuddyLocal);
            lCount++;
        }
    }

    /// <summary>
    /// Display Buddies whose name corresponds to the input.
    /// </summary>
    /// <param name="iSearch">The searching pattern to match.</param>
    public void SearchForBuddy(string iSearch)
    {
        //We display here only the Buddy whose name contains the keyword 'iSearch'
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
