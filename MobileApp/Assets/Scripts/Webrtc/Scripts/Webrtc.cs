﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;


public class Webrtc : MonoBehaviour
{
    public enum CONNECTION { CONNECTING = 0, DISCONNECTING = 1 };

    public CONNECTION ConnectionState { get { return mConnectionState; } }
    public string ID { get { return mLocalUser; } }
    public string RemoteID { get { return mRemoteUser; } set { mRemoteUser = "user1"; } }

    [SerializeField]
    private DBManager dbManager;

    [Header("WebRTC")]
    /// <summary>
    /// URI locating the crossbar server.
    /// </summary>
    [SerializeField]
    private string mCrossbarUri;

    /// <summary>
    /// Crossbar realm used.
    /// </summary>
    [SerializeField]
    private string mRealm;

    /// <summary>
    /// Channel to subscribe to on the messaging service.
    /// </summary>
    [SerializeField]
	private string mLocalUser = "user2";

    [SerializeField]
	private string mRemoteUser = "user1";

    [SerializeField]
    private string mWebrtcReceiverObjectName;

    [Header("GUI")]
    public RawImage mRemoteRawImage = null;
    public RawImage mLocalRawImage = null;
    public Text mTextLog = null;

    /// <summary>
    /// Android Texture object
    /// </summary>
    public RemoteNativeTexture mRemoteNativeTexture = null;
    public LocalNativeTexture mLocalNativeTexture = null;
    private CONNECTION mConnectionState = CONNECTION.DISCONNECTING;

	private Mutex mTextureMutex = new Mutex();
	private char[] mResolutionSeparator = new char[] {'*'};

	private const int INIT_WIDTH = 640;
	private const int INIT_HEIGHT = 480;

    //For now Startwebrtc is called at init but in the future it will only be 
    //called when receiving a call request or trying to call someone.
    // StartWebrtc tries to acquire the camera resource and so the camera
    // must be released beforehand.
    void OnEnable()
    {
        // Setup and start webRTC
        SetupWebRTC();
        StartWebRTC();

        mRemoteRawImage.transform.localScale = new Vector3(1, -1, 0);
        mLocalRawImage.transform.localScale = new Vector3(1, 1, 0);

		InitLocalTexture(INIT_WIDTH, INIT_HEIGHT);
		InitRemoteTexture(INIT_WIDTH, INIT_HEIGHT);
    }

	void OnDisable()
	{
		StopWebRTC();
	}

	void InitLocalTexture(int width, int height)
	{
		Debug.Log("WebRTC.InitLocalTexture " + width + "*" + height);

		mTextureMutex.WaitOne();

		if(mLocalNativeTexture != null)
		{
			mLocalNativeTexture.Destroy();
		}

		mLocalNativeTexture = new LocalNativeTexture(width, height);
		mLocalRawImage.texture = mLocalNativeTexture.texture;

		mTextureMutex.ReleaseMutex();
	}

	void InitRemoteTexture(int width, int height)
	{
		Debug.Log("WebRTC.InitRemoteTexture " + width + "*" + height);

		mTextureMutex.WaitOne();

		if(mRemoteNativeTexture != null)
		{
			mRemoteNativeTexture.Destroy();
		}

		mRemoteNativeTexture = new RemoteNativeTexture(width, height);
		mRemoteRawImage.texture = mRemoteNativeTexture.texture;

		mTextureMutex.ReleaseMutex();
	}

    void Update()
    {
        // Ask update of android texture
        
		mTextureMutex.WaitOne();

		if(mRemoteNativeTexture != null)
		{
			mRemoteNativeTexture.Update();
		}

		if(mLocalNativeTexture != null)
		{
			mLocalNativeTexture.Update();
		}

		mTextureMutex.ReleaseMutex();
    }

    /// <summary>
    /// Set the settings of webRTC
    /// Crossbar URI
    /// Realm
    /// Unity name object for get webrtc messages
    /// </summary>
    public void SetupWebRTC()
    {
        //Debug.Log("DB " + dbManager.CurrentUser.LastName);
        //mLocalUser = dbManager.CurrentUser.LastName + UnityEngine.Random.Range(1000, 9999).ToString();
        if (mTextLog)
            mTextLog.text += "setup webrtc" + "\n";

        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

                cls.CallStatic("SetupWebrtc", mCrossbarUri, mRealm, jo, mLocalUser, mWebrtcReceiverObjectName,
                    BuddyTools.Utils.GetStreamingAssetFilePath("client_cert.pem"));
            }
        }
    }

    /// <summary>
    /// Start WebRTC connection
    /// </summary>
    public void StartWebRTC()
    {
        if (mTextLog)
            mTextLog.text += "Starting webRTC" + "\n";
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("StartWebrtc");
        }
    }

    /// <summary>
    /// Stop WebRTC connection
    /// </summary>
    public void StopWebRTC()
    {
        mRemoteRawImage.transform.localScale = new Vector3(1, 1, 0);
        mLocalRawImage.transform.localScale = new Vector3(1, 1, 0);

		mRemoteNativeTexture.Destroy();
		mLocalNativeTexture.Destroy();

        Debug.Log("Stop WebRTC");
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            cls.CallStatic("StopWebrtc");
        }
    }

    /// <summary>
    /// Used to call another user who is listening on channel iChannel.
    /// </summary>
    /// <param name="iChannel">The channel the user you want to call is subscribed to</param>
    public void Call()
    {

        Debug.Log("Call : " + mRemoteUser + "!");
        if (mTextLog)
            mTextLog.text += "Call : " + mRemoteUser + "\n";
        // mTextSend.text += "\nCall : " + iChannel;
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            Debug.Log("Starting call");
            cls.CallStatic("Call", mRemoteUser);
        }

    }

    /// <summary>
    /// Closes a connection to a user.
    /// </summary>
    /// <param name="iChannel">The channel the user you want to close the connection with is listening to</param>
    public void HangUp()
    {
        Debug.Log("Hang Up : " + mRemoteUser);
        if (mTextLog)
            mTextLog.text += "Hang Up : " + mRemoteUser + "\n";
        if (mConnectionState == CONNECTION.CONNECTING)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                cls.CallStatic("Hangup", mRemoteUser);
            }
        }
    }

    /// <summary>
    /// Sends a message to another user through a webrtc datachannel
    /// (unreliable/unordered) or through the messaging service (reliable)
    /// If you try to send a message through the datachannel and there is no
    /// Webrtc direct connection active the message will not be sent.
    /// </summary>
    /// <param name="iMessage">The message to send</param>
    /// <param name="iChannel">The channel of the user you want to send a message to</param>
    /// <param name="iThroughDataChannel">True if this message is to be sent
    /// through a webrtc datachannel, false for the messaging service.
    /// </param>
    public void SendWithDataChannel(string iMessage)//, string iChannel,bool iThroughDataChannel=true)
    {
        bool iThroughDataChannel = true;
        //Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser);
        if (mTextLog)
            mTextLog.text += "sending message : " + iMessage + " to : " + mRemoteUser + "\n";

        if ((mConnectionState == CONNECTION.CONNECTING) && iThroughDataChannel)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser + " through data channel");
                cls.CallStatic("SendMessage", iMessage, mRemoteUser);
            }
        }
        else if (!iThroughDataChannel)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                Debug.Log("sending message : " + iMessage + " to : " + mRemoteUser + " through messaging channel");
                cls.CallStatic("SendMessage", iMessage, mRemoteUser, false);
            }
        }
    }

    /// <summary>
    /// Used by the webrtc library to tell this script wether a direct webrtc connection
    /// has been opened or closed.
    /// </summary>
    /// <param name="iValue">1 for opened, 0 for closed.</param>
    //DO NOT TOUCH AT THE NAME OF THE FUNCTION, it has direct influence over the effective functioning of sending stuffs
    public void setMIsWebrtcConnectionActive(string iValue)
    {
        if (iValue.Equals("1"))
        {
            Debug.Log("Webrtc status : CONNECTING");
            mConnectionState = CONNECTION.CONNECTING;
            if (mTextLog)
                mTextLog.text += "Webrtc connection is ON" + "\n";
        }
        else {
            Debug.Log("Webrtc status : DISCONNECTING");
            mConnectionState = CONNECTION.DISCONNECTING;
            if (mTextLog)
                mTextLog.text += "Webrtc connection OFF" + "\n";
        }
    }

    /// <summary>
    /// Call or hangup the remote channel in function of the actual state
    /// </summary>
    public void ToggleConnection()
    {
        if (mConnectionState == CONNECTION.DISCONNECTING)
            Call();
        else
            HangUp();
    }

    /// <summary>
    /// This function is called by the RTC library when it receives a message.
    /// </summary>
    /// <param name="iMessage">The message that has been received.</param>
    public void onMessage(string iMessage)
    {
        Debug.Log(iMessage);
        if (mTextLog)
            mTextLog.text += "Receive message : " + iMessage + "\n";
    }

    /// <summary>
    /// This function is called by the RTC library when it receives a message.
    /// </summary>
    /// <param name="iMessage">The message that has been received.</param>
    public void onAndroidDebugLog(string iMessage)
    {
        Debug.Log(iMessage);
        if (mTextLog)
            mTextLog.text += "Android Debug : " + iMessage + "\n";
    }

	public void onLocalTextureSizeChanged(string size)
	{
		Debug.Log ("WebRTC.onLocalTextureSizeChanged " + size);

		string[] cuts = size.Split(mResolutionSeparator);
		int width = Int32.Parse(cuts[0]);
		int height = Int32.Parse(cuts[1]);

		InitLocalTexture(width, height);
	}

	public void onRemoteTextureSizeChanged(string size)
	{
		Debug.Log ("WebRTC.onRemoteTextureSizeChanged " + size);

		string[] cuts = size.Split(mResolutionSeparator);
		int width = Int32.Parse(cuts[0]);
		int height = Int32.Parse(cuts[1]);

		InitRemoteTexture(width, height);

	public void onWebRTCStats(string data)
	{
		Debug.Log("Unity " + data + "\n");
	}
}

