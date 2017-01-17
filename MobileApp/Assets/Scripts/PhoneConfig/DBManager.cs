using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DBManager : MonoBehaviour
{
    public string BuddyList { get { return mBuddyList; } }

    [SerializeField]
    private InputField mCreateFName;

    [SerializeField]
    private InputField mCreateLName;

    [SerializeField]
    private InputField mCreateEMail;

    [SerializeField]
    private InputField mCreatePassword;

    [SerializeField]
    private InputField mCreatePasswordConf;

    [SerializeField]
    private Text mRequestFirstname;

    [SerializeField]
    private Text mRequestLastName;

    [SerializeField]
    private InputField mRequestEMail;

    [SerializeField]
    private InputField mRequestPassword;

    [SerializeField]
    private Animator mCanvasAppAnimator;

    private string mUserMail;
    private string mUserFName;
    private string mUserLName;
    private string mHost;
    private string mBuddyList;

    void Start()
    {
        mHost = "52.174.52.152";
        mBuddyList = "";
    }

    public void StartRequestConnection()
    {
        StartCoroutine(RequestConnection());
    }

    private IEnumerator RequestConnection()
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", mRequestFirstname.text);
        lForm.AddField("lastname", mRequestLastName.text);
        lForm.AddField("email", mRequestEMail.text);
        lForm.AddField("password", mRequestPassword.text);
        
        WWW lWww = new WWW("http://" + mHost + "/connect.php", lForm);
        yield return lWww;

        if (lWww.error != null)
            Debug.Log("[ERROR] on WWW Request " + lWww.error);
        else {
            Debug.Log("[WWW] WWW result " + lWww.text);
            if(lWww.text != "KO") {
                mBuddyList = lWww.text;
                ConfirmConnection();
                mUserMail = mRequestEMail.text;
                mUserFName = mRequestFirstname.text;
                mUserLName = mRequestLastName.text;
            }
        }
        ResetRequestParameters();
    }

    private void ConfirmConnection()
    {
        mCanvasAppAnimator.SetTrigger("EndScene");
        mCanvasAppAnimator.SetTrigger("GoSelectBuddy");
    }

    public void StartCreateAccount()
    {
        Debug.Log("First pw : " + mCreatePassword.text + "; Second pw : " + mCreatePasswordConf.text);

        if(string.Compare(mCreatePassword.text, mCreatePasswordConf.text) == 0)
            StartCoroutine(CreateAccount());
    }

    private IEnumerator CreateAccount()
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", mCreateFName.text);
        lForm.AddField("lastname", mCreateLName.text);
        lForm.AddField("email", mCreateEMail.text);
        lForm.AddField("password", mCreatePassword.text);

        WWW lWww = new WWW("http://" + mHost + "/createaccount.php", lForm);
        yield return lWww;

        if (lWww.error != null)
            Debug.Log("[ERROR] on WWW Request");
        else {
            Debug.Log("Received results : " + lWww.text);
            ConfirmAccountCreation();
        }

        ResetCreateParameters();
    }

    public void StartAddBuddyToUser(Text iBuddyID)
    {
        StartCoroutine(AddBuddyToUser(iBuddyID.text));
    }

    private IEnumerator AddBuddyToUser(string iBuddyID)
    {
        WWWForm lForm = new WWWForm();
        lForm.AddField("firstname", mUserFName);
        lForm.AddField("lastname", mUserLName);
        lForm.AddField("email", mUserMail);
        lForm.AddField("buddyid", iBuddyID);
        
        WWW lWww = new WWW("http://" + mHost + "/AddBuddy.php", lForm);
        yield return lWww;

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
        mCanvasAppAnimator.SetTrigger("EndScene");
        mCanvasAppAnimator.SetTrigger("GoConnectAccount");
    }

    private void ResetCreateParameters()
    {
        mCreatePasswordConf.text = "";
        mCreatePassword.text = "";
        mCreateLName.text = "";
        mCreateFName.text = "";
        mCreateEMail.text = "";
    }

    private void ResetRequestParameters()
    {
        mRequestEMail.text = "";
        mRequestPassword.text = "";
    }
}
