using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HttpResponse
{
	public bool ok;
	public string msg;
}

[Serializable]
public class PhoneUser
{
    public bool IsDefaultUser;
    public string LastName;
    public string FirstName;
    public string Email;
    public string Picture;
}

//Array of active users on phone
[Serializable]
public class PhoneUserList
{
    public PhoneUser[] Users;
}

/// <summary>
/// Manages all requests to DataBase : connection, account creation, etc.
/// </summary>
public class DBManager : MonoBehaviour
{
    public string BuddyList { get { return mBuddyList; } }
    public PhoneUser CurrentUser { get { return mCurrentUser; } }

    [SerializeField]
    private PoolManager poolManager;

    [SerializeField]
    private GameObject popupNoConnection;

    [SerializeField]
    private GameObject DotOFF;

    [SerializeField]
    private GameObject DotON;

    [SerializeField]
    private Transform DotTransform;

    [SerializeField]
    private Animator canvasAppAnimator;

    [SerializeField]
    private GoBack menuManager;

    [SerializeField]
    private Text textFirstName;

    [SerializeField]
    private Text textLastName;

    private string mHost;
    private string mBuddyList;
	private string[] mCookie;
    private PhoneUser mCurrentUser;
    private PhoneUserList mUserList;
    private Texture2D mProfileTexture;

	private PopupHandler popupHandler;

    void Start()
    {
        //Register Azure public IP for MySQL and PHP requests
		mHost = "52.174.52.152:8080";
        mBuddyList = "";
        mCurrentUser = new PhoneUser();
        ReadPhoneUsers();
		popupHandler = GameObject.Find("PopUps").GetComponent<PopupHandler>();
    }

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            popupNoConnection.SetActive(true);
        else
            popupNoConnection.SetActive(false);
    }

	public void StartRequestConnection(string firstName, string lastName, string email, string password)
    {
		StartCoroutine(ConnectAccountSess(firstName, lastName, email, password));
    }

	private IEnumerator ConnectAccountSess(string firstName, string lastName, string email, string password)
	{
		if (email.Contains("demo") && password.Contains("demo"))
		{
			mCurrentUser = new PhoneUser()
			{
				IsDefaultUser = false,
				LastName = "DEMO",
				FirstName = "Mr.",
				Email = "demo@demo.com",
				Picture = "DefaultUser"
			};
			ConfirmConnection();
			yield break;
		}

		WWWForm lForm = new WWWForm ();
		lForm.AddField ("email", email);
		lForm.AddField ("password", password);

		WWW lWWW = new WWW ("http://" + mHost + "/connectSess.php", lForm);
		yield return lWWW;

		if (lWWW.error != null)
		{
			Debug.Log ("[ERROR] on WWW Request " + lWWW.error + " / " + lWWW.text);
		}
		else
		{
			HttpResponse resp = JsonUtility.FromJson<HttpResponse>(lWWW.text);
			if(resp.ok)
			{
				Debug.Log ("WWW Success : " + lWWW.responseHeaders ["SET-COOKIE"]);
				mCookie = lWWW.responseHeaders ["SET-COOKIE"].Split (new char[] { '=', ';' });

				mBuddyList = lWWW.text;
				string lPicture = "";
				foreach(PhoneUser lUser in mUserList.Users)
				{
					if (lUser.FirstName == firstName && lUser.LastName == lastName && lUser.Email == email)
						lPicture = lUser.Picture;
				}

				mCurrentUser = new PhoneUser()
				{
					IsDefaultUser = false,
					LastName = lastName,
					FirstName = firstName,
					Email = email,
					Picture = lPicture
				};
				RetrieveBuddyList();
				ConfirmConnection();
			}
			else
			{
				popupHandler.DisplayError("Erreur", resp.msg);
			}
		}
	}

    public void ConfirmConnection()
    {
        //Activate Canvas animations
        menuManager.GoSelectBuddyMenu();
    }

	public void StartCreateAccount(string firstName, string lastName, string email, string password, string passwordConf)
    {
		if (string.Compare(password, passwordConf) == 0)
			StartCoroutine(CreateAccount(firstName, lastName, email, password));
    }

	private IEnumerator CreateAccount(string firstName, string lastName, string email, string password)
	{
		WWWForm lForm = new WWWForm ();
		lForm.AddField ("firstname", firstName);
		lForm.AddField ("lastname", lastName);
		lForm.AddField ("email", email);
		lForm.AddField ("password", password);
		lForm.AddField ("hiddenkey", "key");

		WWW lWww = new WWW("http://" + mHost + "/createAccountSess.php", lForm);
		yield return lWww;

		if(lWww.error != null) {
			Debug.Log("[ERROR] on WWW Request" + lWww.error);
		} else {
			HttpResponse resp = JsonUtility.FromJson<HttpResponse>(lWww.text);
			if(resp.ok) {
				popupHandler.DisplayError("Succes", resp.msg);
				AddUserToConfig(firstName, lastName, email);
				menuManager.GoConnectionMenu();
				ResetCreateParameters();
                ReadPhoneUsers();
            } else {
				popupHandler.DisplayError("Erreur", resp.msg);
			}
		}
	}

	public void StartEditAccount(string firstName, string lastName, string password, string passwordConf)
    {
		if(string.Compare(password, passwordConf) == 0) {
			StartCoroutine(EditAccount(firstName, lastName, password));
		}
    }

	private IEnumerator EditAccount(string firstName, string lastName, string password)
	{
		string email = mCurrentUser.Email;
		Debug.Log("User email" + email);
	
		WWWForm lForm = new WWWForm ();
		lForm.AddField ("firstname", firstName);
		lForm.AddField ("lastname", lastName);
		lForm.AddField ("email", email);
		lForm.AddField ("password", password);
		lForm.AddField ("hiddenkey", "key");

		WWW lWww = new WWW("http://" + mHost + "/editAccount.php", lForm.data, addSessionCookie(lForm.headers));
		yield return lWww;

		if(lWww.error != null) {
			Debug.Log("[ERROR] on WWW Request" + lWww.error);
		} else {
			HttpResponse resp = JsonUtility.FromJson<HttpResponse>(lWww.text);
			if(resp.ok) {
				popupHandler.DisplayError("Succes", resp.msg);
				EditUserToConfig(firstName, lastName, email);
				menuManager.PreviousMenu();
			} else {
				popupHandler.DisplayError("Erreur", resp.msg);
			}
		}
	}

	public void StartAddBuddyToUser(string iBuddyID, string name)
	{
		StartCoroutine(AddBuddySess(iBuddyID, name));
	}
	
    private IEnumerator AddBuddyToUser(string iBuddyID)
    {
        //Add desired Buddy to the user's list of Buddies on the DataBase
        //Fill POST parameters
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", mCurrentUser.FirstName);
        lForm.AddField("lastname", mCurrentUser.LastName);
        lForm.AddField("email", mCurrentUser.Email);
        lForm.AddField("buddyid", iBuddyID);
        
        WWW lWww = new WWW("http://" + mHost + "/AddBuddy.php", lForm);
        yield return lWww;

        //Check for errors
        if (lWww.error != null)
            Debug.Log("[ERROR] on WWW Request");
        else
        {
            if (lWww.text != "KO") {
                Debug.Log("Succesfully added Buddy to current User.");
				RetrieveBuddyList();
            }
            else
                Debug.Log("Request failed : " + lWww.text);
        }
    }

	private IEnumerator AddBuddySess(string iBuddyID, string name)
	{
		Debug.Log ("AddBuddySess " + iBuddyID + " " + name);

		WWWForm lForm = new WWWForm ();
		lForm.AddField ("specialID", iBuddyID);
		lForm.AddField ("name", name);

		WWW lWWW = new WWW ("http://" + mHost + "/addBuddySess.php", lForm.data, addSessionCookie(lForm.headers));
		yield return lWWW;

		if (lWWW.error != null)
			Debug.Log ("[ERROR] on WWW Request :" + lWWW.error);
		else {
			Debug.Log ("WWW Success : " + lWWW.text);
			HttpResponse resp = JsonUtility.FromJson<HttpResponse>(lWWW.text);
			if(resp.ok) {
				popupHandler.DisplayError("Succes", resp.msg);
				menuManager.PreviousMenu();
				RetrieveBuddyList();
			} else {
				popupHandler.DisplayError("Erreur", resp.msg);
			}
		}
	}

	public void StartEditBuddy(string iSpecialID, string iName)
	{
		StartCoroutine(EditBuddy(iSpecialID, iName));
	}

	private IEnumerator EditBuddy(string iSpecialID, string iName)
	{
		WWWForm lForm = new WWWForm ();
		lForm.AddField ("specialID", iSpecialID);
		lForm.AddField ("name", iName);

		WWW lWWW = new WWW ("http://" + mHost + "/editBuddy.php", lForm.data, addSessionCookie(lForm.headers));
		yield return lWWW;

		if (lWWW.error != null)
			Debug.Log ("[ERROR] on WWW Request :" + lWWW.error);
		else {
			HttpResponse resp = JsonUtility.FromJson<HttpResponse>(lWWW.text);
			if(resp.ok) {
				popupHandler.DisplayError("Succes", resp.msg);
				menuManager.PreviousMenu();
				RetrieveBuddyList();
				Debug.Log("WWW Success");
			} else {
				popupHandler.DisplayError("Erreur", resp.msg);
				Debug.Log(resp.msg);
			}
		}
	}

	private void RetrieveBuddyList()
	{
		StartCoroutine(RetrieveBuddyListCo());
	}

	private IEnumerator RetrieveBuddyListCo()
	{
		WWWForm lForm = new WWWForm();

		WWW lWWW = new WWW ("http://" + mHost + "/retrieveBuddyList.php", new byte[] { 0 }, addSessionCookie(lForm.headers));
		yield return lWWW;

		if(lWWW.error != null)
		{
			Debug.Log("[ERROR] on WWW Request :" + lWWW.error);
		}
		else
		{
			if(lWWW.text.CompareTo("not logged") == 0)
			{
				popupHandler.DisplayError("Erreur", "Veuillez vous identifier");
			}
			else
			{
				Debug.Log("WWW Success : " + lWWW.text);
				mBuddyList = lWWW.text;
				menuManager.GoSelectBuddyMenu();
			}
		}
	}

    public void ReadPhoneUsers()
    {
        //We read the user file and save the list of registered users on the phone
        Debug.Log("Reading phone users");
        PhoneUserList lUserList = new PhoneUserList();

        StreamReader lstreamReader = new StreamReader(BuddyTools.Utils.GetStreamingAssetFilePath("users.txt"));
        string lTemp = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        lUserList = JsonUtility.FromJson<PhoneUserList>(lTemp);

        //Then we get the default user and register it as the "current" one
        foreach (PhoneUser lUser in lUserList.Users)
        {
            if (lUser.IsDefaultUser)
            {
                mCurrentUser = lUser;
                textFirstName.text = lUser.FirstName;
                textLastName.text = lUser.LastName;
                //GameObject.Find("TextFirstName").GetComponent<Text>().text = lUser.FirstName;
                //GameObject.Find("Text_LastName").GetComponent<Text>().text = lUser.LastName;
                //LoadUserPicture(lUser.Picture);
                //Debug.Log("Default user is " + lUser.FirstName + " " + lUser.LastName);
                break;
            }
        }

        mUserList = lUserList;

        foreach (Transform lChild in DotTransform)
            GameObject.Destroy(lChild.gameObject);

        Debug.Log("Length " + lUserList.Users.Length);

        for (int i = 0; i < lUserList.Users.Length - 1; i++)
        {
            GameObject lDot = GameObject.Instantiate(DotOFF);
            lDot.transform.SetParent(DotTransform);
            lDot.transform.localScale = Vector3.one;
        }

        GameObject lDotOn = GameObject.Instantiate(DotON);
        lDotOn.name = "Dot_ON";
        lDotOn.transform.SetParent(DotTransform);
        lDotOn.transform.localScale = Vector3.one;
        lDotOn.transform.SetSiblingIndex(Array.IndexOf(mUserList.Users, mCurrentUser));
    }

    private void ResetCreateParameters()
    {
		InputField ifCurrent = GameObject.Find("Field_FirstName").GetComponent<InputField>();
		ifCurrent.Select();
		ifCurrent.text = ""; // with unity 5.6 > change this line and the previous to ifCurrent.Clear();

		ifCurrent = GameObject.Find("Field_LastName").GetComponent<InputField>();
		ifCurrent.Select();
		ifCurrent.text = "";

		ifCurrent = GameObject.Find("Create_Email_Input").GetComponent<InputField>();
		ifCurrent.Select();
		ifCurrent.text = "";

		ifCurrent = GameObject.Find("Create_PW_Input").GetComponent<InputField>();
		ifCurrent.Select();
		ifCurrent.text = "";

		ifCurrent = GameObject.Find("Create_PWConf_Input").GetComponent<InputField>();
		ifCurrent.Select();
		ifCurrent.text = "";
    }

    private void ExportToJson(PhoneUserList iUser)
    {
        //Write User list on user file as JSON format
        string lJSON = JsonUtility.ToJson(iUser, true);
        StreamWriter lStrWriter = new StreamWriter(BuddyTools.Utils.GetStreamingAssetFilePath("users.txt"));
        lStrWriter.Write(lJSON);
        lStrWriter.Close();
    }

    private void AddUserToConfig(string iFName, string iLName, string iEMail, bool iIsDefaultUser = false, string iPicture="")
    {
        //Create new user
        PhoneUser lNewUser = new PhoneUser()
        {
            FirstName = iFName,
            LastName = iLName,
            Email = iEMail,
            IsDefaultUser = iIsDefaultUser,
            Picture = iPicture
        };

        //Add new user to current list
        //As only arrays can be used in json, we need to create an intermediate bigger list.
        int lUserLength = mUserList.Users.Length;
        PhoneUser[] lTempList = new PhoneUser[lUserLength+1];

        for(int i=0; i<lUserLength; i++) {
            lTempList[i] = mUserList.Users[i];
        }
        lTempList[lUserLength] = lNewUser;

        mUserList.Users = lTempList;

        //Save the new user file
        ExportToJson(mUserList);
    }

	private void EditUserToConfig(string firstname, string lastname, string email)
	{
		for(int i = 0; i < mUserList.Users.Length; i++) {
			if(mUserList.Users[i].Email.CompareTo(email) == 0) {
				mUserList.Users[i].FirstName = firstname;
				mUserList.Users[i].LastName = lastname;
			}
		}
	}

    public Sprite GetCurrentUserImage()
    {
        //Function name is explicit enough. We load the picture file into the sprite
        if(!string.IsNullOrEmpty(mCurrentUser.Picture)) {
            byte[] lFileData = File.ReadAllBytes(BuddyTools.Utils.GetStreamingAssetFilePath(mCurrentUser.Picture));
            Texture2D lTex = new Texture2D(2, 2);
            lTex.LoadImage(lFileData);
            //Image lProfilePicture = GameObject.Find(iObjectName).GetComponentsInChildren<Image>()[2];
            //lProfilePicture.sprite = Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
            return Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        } else {
            Sprite lSprite = Resources.Load<Sprite>("DefaultUser") as Sprite;
            return lSprite;
        }
    }

    private void LoadUserPicture(string iPictureName)
    {
        //Function name is explicit enough. We load the picture file into the sprite
        if((!string.IsNullOrEmpty(iPictureName)))
        {
            byte[] lFileData = File.ReadAllBytes(BuddyTools.Utils.GetStreamingAssetFilePath(iPictureName));
            Texture2D lTex = new Texture2D(2, 2);
            lTex.LoadImage(lFileData);
            Image lProfilePicture = GameObject.Find("Connect_User_Picture").GetComponentsInChildren<Image>()[2];
            lProfilePicture.sprite = Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        } else {
            //Sprite lSprite = Resources.Load<Sprite>("DefaultUser") as Sprite;
            Image lProfilePicture = GameObject.Find("Connect_User_Picture").GetComponentsInChildren<Image>()[2];
            lProfilePicture.sprite = poolManager.GetSprite("DefaultUser");
        }
    }

    public void DisplayNextUser()
    {
        //Self-explanatory
        int lIndex = Array.IndexOf(mUserList.Users, mCurrentUser);

        if(lIndex != mUserList.Users.Length - 1) {
            mCurrentUser = mUserList.Users[lIndex + 1];
        } else {
            mCurrentUser = mUserList.Users[0];
        }
        GameObject.Find("TextFirstName").GetComponent<Text>().text = mCurrentUser.FirstName;
        GameObject.Find("Text_LastName").GetComponent<Text>().text = mCurrentUser.LastName;
        GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
        GameObject lDot = GameObject.Find("Dots/Dot_ON");
        lDot.transform.SetSiblingIndex(Array.IndexOf(mUserList.Users, mCurrentUser));
        LoadUserPicture(mCurrentUser.Picture);
    }

    public void DisplayPreviousUser()
    {
        //Self-explanatory
        int lIndex = Array.IndexOf(mUserList.Users, mCurrentUser);

        if (lIndex != 0)
        {
            mCurrentUser = mUserList.Users[lIndex - 1];
        } else {
            mCurrentUser = mUserList.Users[mUserList.Users.Length - 1];
        }
        GameObject.Find("TextFirstName").GetComponent<Text>().text = mCurrentUser.FirstName;
        GameObject.Find("Text_LastName").GetComponent<Text>().text = mCurrentUser.LastName;
        GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
        GameObject lDot = GameObject.Find("Dots/Dot_ON");
        lDot.transform.SetSiblingIndex(Array.IndexOf(mUserList.Users, mCurrentUser));
        LoadUserPicture(mCurrentUser.Picture);
    }

    private string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

	private Dictionary<string, string> addSessionCookie(Dictionary<string, string> dictionary)
	{
		if(mCookie != null)
		{
			dictionary.Add("Cookie", "PHPSESSID=" + mCookie[4]);
		}

		return dictionary;
	}
}
