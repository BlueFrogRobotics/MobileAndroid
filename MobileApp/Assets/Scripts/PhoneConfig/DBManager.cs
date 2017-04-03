using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;

//Data structure corresponding to DataBase structure
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

    /*[SerializeField]
    private Image profilePicture;
    
    [SerializeField]
    private InputField createFName;

    [SerializeField]
    private InputField createLName;

    [SerializeField]
    private InputField createEMail;

    [SerializeField]
    private InputField createPassword;

    [SerializeField]
    private InputField createPasswordConf;

    [SerializeField]
    private Text requestFirstname;

    [SerializeField]
    private Text requestLastName;

    [SerializeField]
    private InputField requestEMail;

    [SerializeField]
    private InputField requestPassword;*/

    [SerializeField]
    private GameObject popupNoConnection;

    [SerializeField]
    private GameObject DotOFF;

    [SerializeField]
    private Animator canvasAppAnimator;

    [SerializeField]
    private GoBack menuManager;
    
    private string mHost;
    private string mBuddyList;
    private PhoneUser mCurrentUser;
    private PhoneUserList mUserList;
    private Texture2D mProfileTexture;

    void Start()
    {
        //Register Azure public IP for MySQL and PHP requests
        mHost = "52.174.52.152:8080";
        mBuddyList = "";
        mCurrentUser = new PhoneUser();
        mUserList = ReadPhoneUsers();
    }

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            popupNoConnection.SetActive(true);
        else
            popupNoConnection.SetActive(false);
    }

    public void StartRequestConnection()
    {
        StartCoroutine(RequestConnection());
    }

    private IEnumerator RequestConnection()
    {
        string lFirstName = GameObject.Find("TextFirstName").GetComponent<Text>().text;
        string lLastName = GameObject.Find("Text_LastName").GetComponent<Text>().text;
        string lEmail = GameObject.Find("EMail_Input").GetComponent<InputField>().text;
        string lPassword = GameObject.Find("Password_Input").GetComponent<InputField>().text;
        
        //Fill POST parameters
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", lFirstName);
        lForm.AddField("lastname", lLastName);
        lForm.AddField("email", lEmail);
        lForm.AddField("password", lPassword);
        
        WWW lWww = new WWW("http://" + mHost + "/connect.php", lForm);
        yield return lWww;

        //Check if there are errors
        if (lWww.error != null)
            Debug.Log("[ERROR] on WWW Request " + lWww.error);
        else {
            Debug.Log("[WWW] WWW result " + lWww.text);
            if(lWww.text != "KO") {
                //User is logged in and we retrieve the list of Buddies associated with the account
                mBuddyList = lWww.text;
                string lPicture = "";
                foreach(PhoneUser lUser in mUserList.Users)
                {
                    if (lUser.FirstName == lFirstName && lUser.LastName == lLastName && lUser.Email == lEmail)
                        lPicture = lUser.Picture;
                }

                mCurrentUser = new PhoneUser()
                {
                    IsDefaultUser = false,
                    LastName = lLastName,
                    FirstName = lFirstName,
                    Email = lEmail,
                    Picture = lPicture
                };
                ConfirmConnection();
            }
        }
        //ResetRequestParameters();
    }

    public void ConfirmConnection()
    {
        //Activate Canvas animations
        menuManager.GoSelectBuddyMenu();
    }

    public void StartCreateAccount()
    {
        //Debug.Log("First pw : " + createPassword.text + "; Second pw : " + createPasswordConf.text);
        string lPassword = GameObject.Find("Create_PW_Input").GetComponent<InputField>().text;
        string lPasswordConf = GameObject.Find("Create_PWConf_Input").GetComponent<InputField>().text;
        if (string.Compare(lPassword, lPasswordConf) == 0)
            StartCoroutine(CreateAccount());
    }

    private IEnumerator CreateAccount()
    {
        string lFirstName = GameObject.Find("Field_FirstName").GetComponent<InputField>().text;
        string lLastName = GameObject.Find("Field_LastName").GetComponent<InputField>().text;
        string lEmail = GameObject.Find("Create_Email_Input").GetComponent<InputField>().text;
        string lPassword = GameObject.Find("Create_PW_Input").GetComponent<InputField>().text;

        //Fill POST parameters
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", lFirstName);
        lForm.AddField("lastname", lLastName);
        lForm.AddField("email", lEmail);
        lForm.AddField("password", lPassword);

        WWW lWww = new WWW("http://" + mHost + "/createaccount.php", lForm);
        yield return lWww;

        //Check for errors
        if (lWww.error != null)
            Debug.Log("[ERROR] on WWW Request");
        else {
            Debug.Log("Received results : " + lWww.text);
            //New user has been succesfully added to the DataBase. Now add it to the user file
            AddUserToConfig(lFirstName, lLastName, lEmail);
            ConfirmAccountCreation();
        }

        //ResetCreateParameters();
    }

    public void StartAddBuddyToUser(Text iBuddyID)
    {
        StartCoroutine(AddBuddyToUser(iBuddyID.text));
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
            if (lWww.text != "KO")
                Debug.Log("Succesfully added Buddy to current User.");
            else
                Debug.Log("Request failed : " + lWww.text);
        }
    }

    private void ConfirmAccountCreation()
    {
        //Activate Canvas animations
        menuManager.GoConnectionMenu();
    }

    /*private void ResetCreateParameters()
    {
        createPasswordConf.text = "";
        createPassword.text = "";
        createLName.text = "";
        createFName.text = "";
        createEMail.text = "";
    }

    private void ResetRequestParameters()
    {
        requestEMail.text = "";
        requestPassword.text = "";
    }*/

    private void ExportToJson(PhoneUserList iUser)
    {
        //Write User list on user file as JSON format
        string lJSON = JsonUtility.ToJson(iUser, true);
        StreamWriter lStrWriter = new StreamWriter(BuddyTools.Utils.GetStreamingAssetFilePath("users.txt"));
        lStrWriter.Write(lJSON);
        lStrWriter.Close();
    }

    private PhoneUserList ReadPhoneUsers()
    {
        //We read the user file and save the list of registered users on the phone
        PhoneUserList lUserList = new PhoneUserList();

        StreamReader lstreamReader = new StreamReader(BuddyTools.Utils.GetStreamingAssetFilePath("users.txt"));
        string lTemp = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        lUserList = JsonUtility.FromJson<PhoneUserList>(lTemp);
        
        //Then we get the default user and register it as the "current" one
        foreach(PhoneUser lUser in lUserList.Users) {
            if(lUser.IsDefaultUser) {
                mCurrentUser = lUser;
                GameObject.Find("TextFirstName").GetComponent<Text>().text = lUser.FirstName;
                GameObject.Find("Text_LastName").GetComponent<Text>().text = lUser.LastName;
                //LoadUserPicture(lUser.Picture);
                //Debug.Log("Default user is " + lUser.FirstName + " " + lUser.LastName);
                break;
            }
        }

        for(int i=0; i<lUserList.Users.Length - 1; i++)
        {
            GameObject lDot = GameObject.Instantiate(DotOFF);
            lDot.transform.SetParent(GameObject.Find("Dots").transform);
            lDot.transform.localScale = Vector3.one;
        }

        return lUserList;
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

    public Sprite GetCurrentUserImage()
    {
        //Function name is explicit enough. We load the picture file into the sprite
        byte[] lFileData = File.ReadAllBytes(BuddyTools.Utils.GetStreamingAssetFilePath(mCurrentUser.Picture));
        Texture2D lTex = new Texture2D(2, 2);
        lTex.LoadImage(lFileData);
        //Image lProfilePicture = GameObject.Find(iObjectName).GetComponentsInChildren<Image>()[2];
        //lProfilePicture.sprite = Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
        return Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
    }

    private void LoadUserPicture(string iPictureName)
    {
        //Function name is explicit enough. We load the picture file into the sprite
        byte[] lFileData = File.ReadAllBytes(BuddyTools.Utils.GetStreamingAssetFilePath(iPictureName));
        Texture2D lTex = new Texture2D(2, 2);
        lTex.LoadImage(lFileData);
        Image lProfilePicture = GameObject.Find("Connect_User_Picture").GetComponentsInChildren<Image>()[2];
        lProfilePicture.sprite = Sprite.Create(lTex, new Rect(0, 0, lTex.width, lTex.height), new Vector2(0.5F, 0.5F));
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
        GameObject lDot = GameObject.Find("Dot_ON");
        lDot.transform.SetSiblingIndex(Array.IndexOf(mUserList.Users, mCurrentUser) + 1);
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
        GameObject lDot = GameObject.Find("Dot_ON");
        lDot.transform.SetSiblingIndex(Array.IndexOf(mUserList.Users, mCurrentUser) + 1);
        LoadUserPicture(mCurrentUser.Picture);
    }

    private string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }
}
