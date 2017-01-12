using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Networking;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    [SerializeField]
    private Text mUserName;

    [SerializeField]
    private Text mUserFirstname;

    [SerializeField]
    private Text mBuddyName;

    [SerializeField]
    private Text mBuddyID;

    private string mContainerName;
    private string mKeyUrl;
    private string mContainerUrl;
    private string mConfigPath;
    private BuddyConf mConf;

    void Start()
    {
        mContainerName = "buddyconfigcontainer";
        mKeyUrl = "https://bfrblobstorage.azurewebsites.net/api/SasTokenForBfrBlobAccess?code=c5YEDA5GSinc2SLav820TCnY4RCk3UvPsahNfc3x8QRIC/owsM08dA==";
        mContainerUrl = "https://bfrdatacollection.blob.core.windows.net/" + mContainerName + "/";
        mConfigPath = Application.persistentDataPath + "/config.txt";

        GetFromBlobStorage("Buddycelli.txt");
        //mConf = ReadConfigFile();
        //ExportToJson(lConf);

        //StartCoroutine(SpamBlobs(mConf));
        //StartCoroutine(GetFromBlobStorageCo("Buddy1939.txt"));
    }

    private IEnumerator SpamBlobs(BuddyConf iConf)
    {
        for (int i = 3001; i <= 5000; i++) {
            string lFileName = "Buddy" + i + ".txt";
            PutOnBlobStorage(iConf, lFileName);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GetFromBlobStorage(string iFileNameOnBlobStore)
    {
        StartCoroutine(GetFromBlobStorageCo(iFileNameOnBlobStore));
    }

    private IEnumerator GetFromBlobStorageCo(string iFileNameOnBlobStore)
    {
        float lTime = Time.time;

        string lFullPath = iFileNameOnBlobStore;
        string lData = "{\"container\": \"" + mContainerName + "\", \"blobName\": \"" + lFullPath + "\", \"permissions\": \"Read\"}";

        Dictionary<string, string> lH = new Dictionary<string, string>();
        lH["Content-Type"] = "application/json";
        WWW lWWW = new WWW(mKeyUrl, System.Text.Encoding.UTF8.GetBytes(lData), lH);

        yield return lWWW;

        if (!string.IsNullOrEmpty(lWWW.error))
            Debug.Log("[WWW] Error on request : " + lWWW.error);

        KeyResponse lKey = JsonUtility.FromJson<KeyResponse>(lWWW.text);

        //Debug.Log("Full uri : " + lContainerUrl + lFullPath + lKey.token);

        lWWW = new WWW(mContainerUrl + lFullPath + lKey.token);
        yield return lWWW;

        if (!string.IsNullOrEmpty(lWWW.error))
            Debug.Log("[WWW] Error on request : " + lWWW.error);

        //Debug.Log("GET Task finished");
        mConf = JsonUtility.FromJson<BuddyConf>(lWWW.text);
        StreamWriter lWriter = new StreamWriter(mConfigPath);
        lWriter.Write(lWWW.text);
        lWriter.Close();

        Debug.Log("Time elapsed " + (Time.time - lTime) + "s");
    }

    public void SaveConfig()
    {
        mConf.users[0].firstname = mUserFirstname.text;
        mConf.users[0].name = mUserName.text;
        PutOnBlobStorage(mConf, mBuddyName.text + mBuddyID.text + ".txt");
    }

    private void PutOnBlobStorage(BuddyConf iConf, string iFileNameOnBlobStore)
    {
        StartCoroutine(PutOnBlobStorageCo(iConf, iFileNameOnBlobStore));
    }

    private IEnumerator PutOnBlobStorageCo(BuddyConf iConf, string iFileNameOnBlobStore)
    {
        string lFullPath = iFileNameOnBlobStore;
        string lData = "{\"container\": \"" + mContainerName + "\", \"blobName\": \"" + lFullPath + "\", \"permissions\": \"Write\"}";

        //1ST PART : Get the token to be able to access data from blob store
        Dictionary<string, string> lH = new Dictionary<string, string>();
        lH["Content-Type"] = "application/json";
        WWW lWWW = new WWW(mKeyUrl, System.Text.Encoding.UTF8.GetBytes(lData), lH);

        yield return lWWW;

        if (!string.IsNullOrEmpty(lWWW.error))
            Debug.Log("[WWW] Error on request : " + lWWW.error);
        KeyResponse lKey = JsonUtility.FromJson<KeyResponse>(lWWW.text);
        Debug.Log("Got token " + lKey.token);

        //2ND PART : Access the blob store
        //StreamReader lReader = new StreamReader("Assets/Others/" + iFileToRead);
        //string lPutData = lReader.ReadToEnd();
        //lReader.Close();
        //lPutData.Replace('\r', ' ');
        //lPutData.Replace('\t', ' ');
        iConf.version++;
        string lPutData = JsonUtility.ToJson(iConf);
        UnityWebRequest lAccessWWW = UnityWebRequest.Put(mContainerUrl + lFullPath + lKey.token,
                                    System.Text.Encoding.UTF8.GetBytes(lPutData));
        lAccessWWW.SetRequestHeader("Content-Type", "text/plain");
        lAccessWWW.SetRequestHeader("x-ms-blob-type", "BlockBlob");

        yield return lAccessWWW.Send();

        if (!string.IsNullOrEmpty(lAccessWWW.error))
            Debug.Log("[WWW] ERROR " + lAccessWWW.error);

        if (lAccessWWW.responseCode == 201)
            Debug.Log("SUCCESS ! File has been successfuly uploaded to blob store");
        else
            Debug.Log("[WWW] ERROR returned with code " + lAccessWWW.responseCode);
    }

    private BuddyConf ReadConfigFile()
    {
        StreamReader lstreamReader = new StreamReader(mConfigPath);
        string lTemp = lstreamReader.ReadToEnd();
        lstreamReader.Close();

        string lDebugStr = "";
        BuddyConf lConf = JsonUtility.FromJson<BuddyConf>(lTemp);

        foreach (Users lUser in lConf.users)
            lDebugStr += "User: " + lUser.firstname + ' ' + lUser.name + ' ' + lUser.age + " is a " + lUser.rights + "\n";

        lDebugStr += "House Map " + lConf.houseMap + "\n";
        lDebugStr += "Vocal signature " + lConf.vocalSignature + " and facial recognition file " + lConf.faceRecognition + '\n';

        foreach (Applications lApp in lConf.applications) {
            lDebugStr += "Application: " + lApp.name + "with parameters";
            foreach (string lParam in lApp.parameters)
                lDebugStr += " " + lParam;
            lDebugStr += '\n';
        }

        foreach (Event lEvent in lConf.events)
            lDebugStr += "Event: " + lEvent.title + " (" + lEvent.description + ")\n";

        Debug.Log("Buddy Configuration:\n" + lDebugStr);

        return lConf;
    }

    private void ExportToJson(BuddyConf iConf, string iFileName)
    {
        string lJSON = JsonUtility.ToJson(iConf, false);
        StreamWriter lStrWriter = new StreamWriter("Assets/Others/" + iFileName);
        lStrWriter.Write(lJSON);
        lStrWriter.Close();
    }

    private void ExportToJson(Users[] iUser, string iHouseMap, string iVocalS, string iFace, Applications[] iApp, Timetable[] iTable, Event[] iEv, string iFileName)
    {
        BuddyConf lConf = new BuddyConf()
        {
            users = iUser,
            houseMap = iHouseMap,
            vocalSignature = iVocalS,
            faceRecognition = iFace,
            applications = iApp,
            timetable = iTable,
            events = iEv
        };

        ExportToJson(lConf, iFileName);
    }
}
