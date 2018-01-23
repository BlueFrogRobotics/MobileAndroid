using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Buddy;

[Serializable]
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

public class BuddyDB
{
    public string ID;
    public string name;
    public string status;
    public string appName;
    public Int32 timestamp;
}

/// <summary>
/// Manages all requests to DataBase : connection, account creation, etc.
/// </summary>
public class DBManager : MonoBehaviour
{
    public string BuddyList { get { return mBuddyList; } }
    public List<BuddyDB> BuddiesList { get { return mBuddiesList; } }
    public PhoneUser CurrentUser { get { return mCurrentUser; } }

    [SerializeField]
    private BackgroundListener backgroundListener;

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

    [SerializeField]
    private InputField inputFirstName;

    [SerializeField]
    private InputField inputLastName;

    private bool mNotifAllowed;
    private int mDisplayedIndex;
    private string mHost;
    private string mBuddyList;
    private List<BuddyDB> mBuddiesList;
    private string mUserFilePath;
    private string[] mCookie;
    private PhoneUser mCurrentUser;
    private PhoneUserList mUserList;

    private PopupHandler popupHandler;

    private PhoneUser mUser;
	public static string tmpImgPath = "";
	private bool mImagePreselected = false;
	private Texture2D mTex;

    private List<string> mBuddiesIDs;

    void Start()
	{
		//Register Azure public IP for MySQL and PHP requests
		mHost = "52.174.52.152:8080";
		mBuddyList = "";
		mUserFilePath = Application.persistentDataPath + "/users.txt";
		mCurrentUser = new PhoneUser ();
		ReadPhoneUsers (true);
		popupHandler = GameObject.Find ("PopUps").GetComponent<PopupHandler> ();
		mBuddiesIDs = new List<string> ();
		mBuddiesList = new List<BuddyDB> ();
		mTex = new Texture2D (2, 2);
		Debug.Log ("DB MANAGER STARTED");
	}

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            popupNoConnection.SetActive(true);
        else
            popupNoConnection.SetActive(false);

		if(mImagePreselected)
		{
			mImagePreselected = false;
		}
	}

	public void OpenFileBrowser(bool resizeImage)
	{
		using (AndroidJavaClass cls = new AndroidJavaClass("com.bfr.filebrowserlib.FileBrowser"))
		{
			using (AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
			{
				cls.CallStatic ("openGallery", jc.GetStatic<AndroidJavaObject> ("currentActivity"), Application.persistentDataPath, resizeImage);
			}
		}
	}

    public bool IsBuddiesListEmpty()
    {
        return (mBuddiesList.Count > 0 ? false : true);
    }

    public void StartRequestConnection(string iFirstName, string iLastName, string iEmail, string iPassword, bool iNotif)
    {
        mNotifAllowed = iNotif;
        StartCoroutine(ConnectAccountSess(iFirstName, iLastName, iEmail, iPassword));
    }

    private IEnumerator ConnectAccountSess(string iFirstName, string iLastName, string iEmail, string iPassword)
    {
        //TO Clean 
        /* if (iEmail.Contains("demo") && iPassword.Contains("demo"))
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
         }*/
        Debug.Log("iLastName = " + iLastName);
        WWWForm lForm = new WWWForm();
        lForm.AddField("email", iEmail);
        lForm.AddField("password", iPassword);

        WWW lWWW = new WWW("http://" + mHost + "/connectSess.php", lForm);
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    Debug.Log("WWW Success : " + lWWW.responseHeaders["SET-COOKIE"]);
                    mCookie = lWWW.responseHeaders["SET-COOKIE"].Split(new char[] { ';' });

                    mBuddyList = lWWW.text;
                    string lPicture = "";
                    bool lFound = false;

                    foreach (PhoneUser lUser in mUserList.Users)
                    {
                        if (lUser.FirstName == iFirstName && lUser.LastName == iLastName && lUser.Email == iEmail)
                        {
                            lPicture = lUser.Picture;
                            lFound = true;
                            //Bug 1: MG
                            mCurrentUser = lUser;
                        }
                    }
                    //Bug 1: MG
                    //					mCurrentUser = new PhoneUser() {
                    //						IsDefaultUser = false,
                    //						LastName = iLastName,
                    //						FirstName = iFirstName,
                    //						Email = iEmail,
                    //						Picture = lPicture
                    //					};
                    if (!lFound)
                    {
                        AddUserToConfig(iFirstName, iLastName, iEmail);
                    }

                    RetrieveBuddyList();
                    ConfirmConnection();
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                }
            }
        }
    }

    public void ConfirmConnection()
    {
        foreach (Transform lChild in DotTransform)
            GameObject.Destroy(lChild.gameObject);
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
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", firstName);
        lForm.AddField("lastname", lastName);
        lForm.AddField("email", email);
        lForm.AddField("password", password);
        lForm.AddField("hiddenkey", "key");

        WWW lWWW = new WWW("http://" + mHost + "/createAccountSess.php", lForm);
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            
            if (resp != null)
            {
                if (resp.ok)
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");

					String imgName = saveImage(email);
					AddUserToConfig(firstName, lastName, email, imgName);
                    
                    ReadPhoneUsers(true, /*Op 1: MG*/ true);
                    menuManager.GoConnectionMenu();
					ResetCreateParameters();
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                }
            }
        }
    }

    public void StartForgottenPassword(string email)
    {
        StartCoroutine(ForgottenPassword(email));
    }

    private IEnumerator ForgottenPassword(string email)
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("email", email);

        WWW lWWW = new WWW("http://" + mHost + "/forgottenPassword.php", lForm);
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                popupHandler.OpenDisplayIcon(resp.msg, resp.ok ? "Check" : "Warning");
            }
        }
    }

    public void StartEditAccount(string firstName, string lastName, string password)
    {
        StartCoroutine(EditAccount(firstName, lastName, password));
    }

    private IEnumerator EditAccount(string firstName, string lastName, string password)
    {
        string email = mCurrentUser.Email;
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", firstName);
        lForm.AddField("lastname", lastName);
        lForm.AddField("email", email);
        if (password != "")
        {
            lForm.AddField("password", password);
        }

        WWW lWWW = new WWW("http://" + mHost + "/editAccount.php", lForm.data, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");
					String imgName = saveImage(email);
					String imgPath = Application.persistentDataPath + "/" + email + ".jpg";
					EditUserToConfig(firstName, lastName, email, File.Exists(imgPath) ? imgPath : imgName);
					tmpImgPath = "";
                    menuManager.PreviousMenu();
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                }
            }
        }
    }

    public void StartAddBuddyToUser(string iBuddyID, string name)
    {
        StartCoroutine(AddBuddySess(iBuddyID, name));
    }

    private IEnumerator AddBuddySess(string iBuddyID, string name)
    {
        Debug.Log("AddBuddySess " + iBuddyID + " " + name);

        WWWForm lForm = new WWWForm();
        lForm.AddField("specialID", iBuddyID);
        lForm.AddField("name", name);

        WWW lWWW = new WWW("http://" + mHost + "/addBuddySess.php", lForm.data, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");
                    menuManager.PreviousMenu();
                    RetrieveBuddyList();
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                }
            }
        }
    }

    public void StartEditBuddy(string iSpecialID, string iName)
    {
        StartCoroutine(EditBuddy(iSpecialID, iName));
    }

    private IEnumerator EditBuddy(string iSpecialID, string iName)
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("specialID", iSpecialID);
        lForm.AddField("name", iName);

        WWW lWWW = new WWW("http://" + mHost + "/editBuddy.php", lForm.data, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");
                    menuManager.PreviousMenu();
                    Debug.Log("WWW Success");
                    //Modify the name of the selected buddy
                    SelectBuddy.BuddyName = iName;
                    foreach (BuddyDB lBuddy in mBuddiesList) {
                        if (lBuddy.ID == SelectBuddy.BuddyID)
                            lBuddy.name = SelectBuddy.BuddyName;
                    }
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                    Debug.Log(resp.msg);
                }
            }
        }
    }

    public void StartRemoveBuddyFromUser(string specialID)
    {
        StartCoroutine(RemoveBuddyFromUser(specialID));
    }

    private IEnumerator RemoveBuddyFromUser(string specialID)
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("specialID", specialID);
        lForm.AddField("hiddenkey", "key");

        WWW lWWW = new WWW("http://" + mHost + "/removeBuddyFromUser.php", lForm.data, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");
                    menuManager.PreviousMenu();
                    RetrieveBuddyList();
                    Debug.Log("WWW Success");
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                    Debug.Log(resp.msg);
                }
            }
        }
    }

    public void StartDeleteAccount(string password)
    {
        StartCoroutine(DeleteAccount(password));
    }

    private IEnumerator DeleteAccount(string password)
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("password", password);
        lForm.AddField("hiddenkey", "key");

        WWW lWWW = new WWW("http://" + mHost + "/deleteAccount.php", lForm.data, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            HttpResponse resp = parseResp(lWWW);
            if (resp != null)
            {
                if (resp.ok)
                {
                    GameObject.Find("PopUps").GetComponent<PopupHandler>().ClosePopup();
                    popupHandler.OpenDisplayIcon(resp.msg, "Check");

                    RemoveUserFromLocalStorage(mCurrentUser);
                    ReadPhoneUsers();
                    menuManager.GoConnectionMenu();

                    Debug.Log("WWW Success");
                }
                else
                {
                    popupHandler.OpenDisplayIcon(resp.msg, "Warning");
                    Debug.Log(resp.msg);
                }
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

        WWW lWWW = new WWW("http://" + mHost + "/retrieveBuddyList.php", new byte[] { 0 }, addSessionCookie(lForm.headers));
        yield return lWWW;

        if (requestOK(lWWW))
        {
            if (lWWW.text.CompareTo("not logged") == 0)
            {
                popupHandler.OpenDisplayIcon("Veuillez vous identifier", "Warning");
            }
            else
            {
                //Debug.Log("WWW Success : " + lWWW.text);
                mBuddyList = lWWW.text;
                mBuddiesList = new List<BuddyDB>();

                if (!string.IsNullOrEmpty(mBuddyList))
                {
                    string[] lBuddyList = mBuddyList.Split('\n');

                    for (int i = 0; i < lBuddyList.Length - 1; i++)
                    {
                        string[] lBuddyIDs = lBuddyList[i].Split('|');
                        mBuddiesList.Add(new BuddyDB { ID = lBuddyIDs[1], name = lBuddyIDs[0] });
                    }
                }

                backgroundListener.SubscribeStatusChannels(mBuddiesList);
                if (mNotifAllowed)
                    backgroundListener.SubscribeNotificationChannels(mBuddiesList);
            }
        }
    }

    public void SubscribeStatus()
    {
        backgroundListener.SubscribeStatusChannels(mBuddiesList);
    }

    public void ReadPhoneUsers(bool iFirstRead = false, /*Op 1: MG*/ bool iCreateAccount = false)
    {
        //We read the user file and save the list of registered users on the phone
        PhoneUserList lUserList = new PhoneUserList();

        if (File.Exists(mUserFilePath))
        {
            //Debug.Log("User file found : " + mUserFilePath);            
        }
        else
        {
            File.Copy(ResourceManager.StreamingAssetFilePath("users.txt"), mUserFilePath);

            if (iFirstRead)
                canvasAppAnimator.SetTrigger("GoFirstConnexion");
        }

        StreamReader lstreamReader = new StreamReader(mUserFilePath);
        string lTemp = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        lUserList = JsonUtility.FromJson<PhoneUserList>(lTemp);

        if (iFirstRead && lUserList.Users.Length == 0)
            canvasAppAnimator.SetTrigger("GoFirstConnexion");
        else if (iFirstRead && lUserList.Users.Length != 0)
            canvasAppAnimator.SetTrigger("GoConnectAccount");

        //Then we get the default user and register it as the "current" one
        foreach (PhoneUser lUser in lUserList.Users)
        {
            if (lUser.IsDefaultUser)
            {
                mCurrentUser = lUser;
                textFirstName.text = lUser.FirstName;
                textLastName.text = lUser.LastName;
                break;
            }
        }

        if (iCreateAccount) mCurrentUser = mUserList.Users [mUserList.Users.Length - 1];

        mUserList = lUserList;

        //TO Ameliorate when adding selecting the photo of a new user 
        //We can use iCreateAccount option instead
        GameObject lImage = GameObject.Find("Connect_User_Picture");
        if (!iFirstRead && lImage != null)
        {
            string lDisplayedPicture = mCurrentUser.Picture;
            LoadUserPicture(lDisplayedPicture);
            GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
            GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
        }

    }

    private void RemoveUserFromLocalStorage(PhoneUser user)
    {
        int lUserLength = mUserList.Users.Length;
        PhoneUser[] lTempList = new PhoneUser[lUserLength - 1];
        Debug.Log("Length " + lUserLength);
        for (int i = 0, j = 0; i < lUserLength; i++, j++)
        {
            if (mUserList.Users[i].Email == user.Email && mUserList.Users[i].FirstName == user.FirstName && mUserList.Users[i].LastName == user.LastName)
            {
                j--;
                Debug.Log("User successfully removed from local storage");
            }
            else
            {
                lTempList[j] = mUserList.Users[i];
            }
        }
        
        mUserList.Users = lTempList;
        ExportToJson(mUserList);     

        foreach (Transform lChild in DotTransform)
            GameObject.Destroy(lChild.gameObject);
    }

	public void GenerateUserDisplay()
    {
        for (int i = 0; i < mUserList.Users.Length - 1; i++)
        {
            GameObject lDot = GameObject.Instantiate(DotOFF);
            lDot.transform.SetParent(DotTransform);
            lDot.transform.localScale = Vector3.one;
        }

        GameObject lDotOn = GameObject.Instantiate(DotON);
        lDotOn.name = "Dot_ON";
        lDotOn.transform.SetParent(DotTransform);
        lDotOn.transform.localScale = Vector3.one;
        mDisplayedIndex = Array.IndexOf(mUserList.Users, mCurrentUser);
        lDotOn.transform.SetSiblingIndex(mDisplayedIndex);
        lDotOn.transform.SetAsLastSibling();
        lDotOn.transform.SetSiblingIndex(mDisplayedIndex);
        //We add a last one to add an existing account
        GameObject lDotLast = GameObject.Instantiate(DotOFF);
        lDotLast.transform.SetParent(DotTransform);
        lDotLast.transform.localScale = Vector3.one;
       
        textFirstName.text = mCurrentUser.FirstName;
        textLastName.text = mCurrentUser.LastName;
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

		tmpImgPath = "";
    }

    private void ExportToJson(PhoneUserList iUser)
    {
        //Write User list on user file as JSON format
        string lJSON = JsonUtility.ToJson(iUser, true);
        StreamWriter lStrWriter = new StreamWriter(mUserFilePath);
        lStrWriter.Write(lJSON);
        lStrWriter.Close();
    }

	private void AddUserToConfig(string iFName, string iLName, string iEMail, string iPicture = "", bool iIsDefaultUser = false)
    {
        Debug.Log("Adding user to config");
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
        PhoneUser[] lTempList = new PhoneUser[lUserLength + 1];

        for (int i = 0; i < lUserLength; i++)
        {
            lTempList[i] = mUserList.Users[i];
        }
        lTempList[lUserLength] = lNewUser;

        mUserList.Users = lTempList;

        //Save the new user file
        ExportToJson(mUserList);
        mCurrentUser = lNewUser;
    }

	private void EditUserToConfig(string firstname, string lastname, string email, string picture = "")
    {
        for (int i = 0; i < mUserList.Users.Length; i++)
        {
            if (mUserList.Users[i].Email.CompareTo(email) == 0)
            {
                mUserList.Users[i].FirstName = firstname;
				mUserList.Users[i].LastName = lastname;
				mUserList.Users[i].Picture = picture;
            }
        }
        ExportToJson(mUserList);
    }

    public void RemoveUserToConfig(string iFName, string iLName, string iEMail)
    {
        mUser = new PhoneUser()
        {
            FirstName = iFName,
            LastName = iLName,
            Email = iEMail
        };
        popupHandler.OpenYesNoIcon("VOULEZ-VOUS VRAIMENT SUPPRIMER LE COMPTE LOCAL ?\n" + iEMail, DeleteUserFromList, "Warning");
    }

    public void DeleteUserFromList()
    {
        RemoveUserFromLocalStorage(mUser);
        ReadPhoneUsers();
        GenerateUserDisplay ();
    }

	private byte[] openFile(String path)
	{
		if(path.Contains("/"))
		{
			return File.ReadAllBytes(path);
		}

		return File.ReadAllBytes(ResourceManager.StreamingAssetFilePath(path));
	}

    public Sprite GetCurrentUserImage()
    {
        //Function name is explicit enough. We load the picture file into the sprite
        if (!string.IsNullOrEmpty(mCurrentUser.Picture))
        {
			byte[] lFileData = openFile(mCurrentUser.Picture);
            mTex.LoadImage(lFileData);
			return Sprite.Create(mTex, new Rect(0, 0, mTex.width, mTex.height), new Vector2(0.5F, 0.5F));
        }
        else
        {
            Sprite lSprite = Resources.Load<Sprite>("DefaultUser") as Sprite;
            return lSprite;
        }
    }

    private void LoadUserPicture(string iPictureName)
	{
		Image lProfilePicture = GameObject.Find("Connect_User_Picture").GetComponentsInChildren<Image>()[2];
        if ((!string.IsNullOrEmpty(iPictureName)))
        {
			byte[] lFileData = openFile(iPictureName);
            mTex.LoadImage(lFileData);
			lProfilePicture.sprite = Sprite.Create(mTex, new Rect(0, 0, mTex.width, mTex.height), new Vector2(0.5F, 0.5F));
        }
        else
		{
			lProfilePicture.sprite = poolManager.GetSprite("DefaultUser");
		}
    }

    public void DisplayNextUser()
    {
        //Self-explanatory
        int lIndex = Array.IndexOf(mUserList.Users, mCurrentUser);
        string lDisplayedPicture = "";

        GameObject lTrashButton = GameObject.Find("Content_Top/Top_UI/Button_L(Clone)");

        if (lIndex != mUserList.Users.Length - 1)
        {
            mCurrentUser = mUserList.Users[lIndex + 1];
            lDisplayedPicture = mCurrentUser.Picture;
            GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
            GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
            mDisplayedIndex++;
            lTrashButton.SetActive(true);
        }
        else
        {
            //If we reach the end of users saved locally, we show the "Add already created account option"
            if (mDisplayedIndex == lIndex)
            {
                textFirstName.gameObject.SetActive(false);
                textLastName.gameObject.SetActive(false);
                inputFirstName.gameObject.SetActive(true);
                inputLastName.gameObject.SetActive(true);
                inputFirstName.text = "Enter your First Name";
                inputLastName.text = "Enter your Last Name";
                GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
                GameObject.Find("EMail_Input").GetComponent<InputField>().text = "";
                mDisplayedIndex++;
                lTrashButton.SetActive(false);
            }
            else
            {
                mCurrentUser = mUserList.Users[0];
                lDisplayedPicture = mCurrentUser.Picture;
                mDisplayedIndex = 0;
                textFirstName.gameObject.SetActive(true);
                textLastName.gameObject.SetActive(true);
                inputFirstName.gameObject.SetActive(false);
                inputLastName.gameObject.SetActive(false);
                GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
                GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
                lTrashButton.SetActive(true);
            }
        }
        textFirstName.text = mCurrentUser.FirstName;
        textLastName.text = mCurrentUser.LastName;
        GameObject lDot = GameObject.Find("Dots/Dot_ON");
        lDot.transform.SetSiblingIndex(mDisplayedIndex);
        LoadUserPicture(lDisplayedPicture);
    }

    public void DisplayPreviousUser()
    {
        //Self-explanatory
        int lIndex = Array.IndexOf(mUserList.Users, mCurrentUser);
        string lDisplayedPicture = "";
        GameObject lTrashButton = GameObject.Find("Content_Top/Top_UI/Button_L(Clone)");

        if (lIndex != 0 && mDisplayedIndex != mUserList.Users.Length)
        {
            mCurrentUser = mUserList.Users[lIndex - 1];
            lDisplayedPicture = mCurrentUser.Picture;
            GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
            GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
            mDisplayedIndex--;
            lTrashButton.SetActive(true);
        }
        else
        {
            mCurrentUser = mUserList.Users[mUserList.Users.Length - 1];
            if (mDisplayedIndex == 0)
            {
                mDisplayedIndex = mUserList.Users.Length;
                textFirstName.gameObject.SetActive(false);
                textLastName.gameObject.SetActive(false);
                inputFirstName.gameObject.SetActive(true);
                inputLastName.gameObject.SetActive(true);
                inputFirstName.text = "Enter your First Name";
                inputLastName.text = "Enter your Last Name";
                GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
                GameObject.Find("EMail_Input").GetComponent<InputField>().text = "";
                lTrashButton.SetActive(false);
            }
            else
            {
                mDisplayedIndex = mUserList.Users.Length - 1;
                lDisplayedPicture = mCurrentUser.Picture;
                textFirstName.gameObject.SetActive(true);
                textLastName.gameObject.SetActive(true);
                inputFirstName.gameObject.SetActive(false);
                inputLastName.gameObject.SetActive(false);
                GameObject.Find("Password_Input").GetComponent<InputField>().text = "";
                GameObject.Find("EMail_Input").GetComponent<InputField>().text = mCurrentUser.Email;
                lTrashButton.SetActive(true);
            }
        }
        textFirstName.text = mCurrentUser.FirstName;
        textLastName.text = mCurrentUser.LastName;
        GameObject lDot = GameObject.Find("Dots/Dot_ON");
        lDot.transform.SetSiblingIndex(mDisplayedIndex);
        LoadUserPicture(lDisplayedPicture);
    }

    private string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    private Dictionary<string, string> addSessionCookie(Dictionary<string, string> dictionary)
    {
        if (mCookie != null)
        {
            dictionary.Add("Cookie", mCookie[0]);
        }

        return dictionary;
    }

    private bool requestOK(WWW www)
    {
        if (www.error != null)
        {
            Debug.Log("[ERROR] on WWW Request " + www.url + " : " + www.error + " / " + www.text);
            popupHandler.OpenDisplayIcon("ECHEC DE COMMUNICATION AVEC LE SERVEUR", "Warning");
            return false;
        }

        return true;
    }

    private HttpResponse parseResp(WWW www)
    {
        HttpResponse resp = null;
        try
        {
            resp = JsonUtility.FromJson<HttpResponse>(www.text);
        }
        catch (Exception e)
        {
            popupHandler.OpenDisplayIcon("UN PROBLEME EST SURVENU LORS DE LA LECTURE DE LA REPONSE DU SERVEUR", "Warning");
        }

        return resp;
    }

	public void onImageSelected(string tmpUserImg)
	{
		tmpImgPath = Application.persistentDataPath + "/" + tmpUserImg + ".jpg";
		mImagePreselected = true;
		LoadUserPicture(tmpImgPath);
	}

	private String saveImage(string imgName)
	{
		if (tmpImgPath != "")
		{
			using (AndroidJavaClass cls = new AndroidJavaClass("com.bfr.filebrowserlib.FileBrowser"))
			{
				cls.CallStatic ("save", imgName);
			}
			return (Application.persistentDataPath + "/" + imgName + ".jpg");
		}

		return "";
	}
}
