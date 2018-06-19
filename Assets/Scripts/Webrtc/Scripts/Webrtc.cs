using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

using Buddy;

/// <summary>
/// Handler of the connection to the Crossbar.IO server and the cycle of the WebRTC communication.
/// </summary>
public class Webrtc : MonoBehaviour
{
    public enum CONNECTION { CONNECTING = 0, DISCONNECTING = 1 };

    public CONNECTION ConnectionState { get { return mConnectionState; } }
    public bool Connected { get; private set; }
    public string ID { get { return mLocalUser; } }
    public string RemoteID { get { return mRemoteUser; } set { mRemoteUser = value; } }
    public string ConnectionInfo { get { return mConnectionInfos; } }

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
	private string mLocalUser;

    [SerializeField]
	private string mRemoteUser;

    /// <summary>
    /// Name of the object that will be called back from the Java WebRTC plugin.
    /// </summary>
    [SerializeField]
    private string mWebrtcReceiverObjectName;

    [SerializeField]
    private RemoteControl remoteControl;

	[SerializeField]
	private GoBack menuManager;

	[SerializeField]
	private GameObject joystick;

	[SerializeField]
	private GameObject remoteMessage;

    [Header("GUI")]
    // Raw image to display the remote video stream.
    public RawImage mRemoteRawImage = null;
    // Raw image to display the local video stream.
    public RawImage mLocalRawImage = null;
    public Text mTextLog = null;

    /// <summary>
    /// Android Texture object.
    /// </summary>
    public NativeTexture mRemoteNativeTexture = null;
    public NativeTexture mLocalNativeTexture = null;
    private CONNECTION mConnectionState = CONNECTION.DISCONNECTING;

	private Mutex mTextureMutex = new Mutex();
	private char[] mResolutionSeparator = new char[] {'*'};

    private string mConnectionInfos = "";

	private const int INIT_WIDTH = 640;
	private const int INIT_HEIGHT = 480;

    private bool mWasActive;

    // For now Startwebrtc is called at init but in the future it will only be 
    // called when receiving a call request or trying to call someone.
    // StartWebrtc tries to acquire the camera resource and so the camera
    // must be released beforehand.
    public void InitWebRTC()
    {
        // Setup and start webRTC
        SetupWebRTC();
        StartWebRTC();

        InitImages();
        mWasActive = true;
    }

	//void OnDisable()
	//{
	//	StopWebRTC();
	//}

    /// <summary>
    /// Initialize the local and remote textures for display on the remote control screen.
    /// </summary>
    public void InitImages()
    {
		InitLocalTexture(INIT_WIDTH, INIT_HEIGHT);
		InitRemoteTexture(INIT_WIDTH, INIT_HEIGHT);
    }

    /// <summary>
    /// Initialize the local texture.
    /// </summary>
    /// <param name="width">Width of the local texture.</param>
    /// <param name="height">Height of the local texture.</param>
	void InitLocalTexture(int width, int height)
	{
		Debug.Log("WebRTC.InitLocalTexture " + width + "*" + height);

        // Wait for the mutex before creating the texture.
		mTextureMutex.WaitOne();

        // If a texture from a previous call exists already, destroy it.
		if(mLocalNativeTexture != null)
		{
			mLocalNativeTexture.Destroy();
		}

        // Create the texture and assign it.
		mLocalNativeTexture = new NativeTexture(width, height, true);
		mLocalRawImage.texture = mLocalNativeTexture.texture;

        // Release the mutex.
		mTextureMutex.ReleaseMutex();
	}

    /// <summary>
    /// Initialize the local texture.
    /// </summary>
    /// <param name="width">Width of the local texture.</param>
    /// <param name="height">Height of the local texture.</param>
	void InitRemoteTexture(int width, int height)
	{
		Debug.Log("WebRTC.InitRemoteTexture " + width + "*" + height);

        // Wait for the mutex before creating the texture.
        mTextureMutex.WaitOne();

        // If a texture from a previous call exists already, destroy it.
        if (mRemoteNativeTexture != null)
		{
			mRemoteNativeTexture.Destroy();
		}

        // Create the texture and assign it.
        mRemoteNativeTexture = new NativeTexture(width, height, false);
		mRemoteRawImage.texture = mRemoteNativeTexture.texture;

        // Release the mutex.
        mTextureMutex.ReleaseMutex();
	}

    void Update()
    {
        // Wait for the mutex then perform the updates of both local and remote textures.
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
    /// Set the settings of WebRTC:
    /// Crossbar URI
    /// Realm
    /// Unity name object to get WebRTC messages.
    /// </summary>
    public void SetupWebRTC()
    {
        Debug.Log("DB " + dbManager.CurrentUser.LastName);
        // Log in to the Crossbar with the mail of the user as identifier.
        mLocalUser = dbManager.CurrentUser.Email;
        if (mTextLog)
            mTextLog.text += "setup webrtc" + "\n";

        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

                // Setup everything for the WebRTC communication.
                cls.CallStatic("SetupWebrtc", mCrossbarUri, mRealm, jo, mLocalUser, mWebrtcReceiverObjectName,
                    ResourceManager.StreamingAssetFilePath("client_cert.pem"));
                
            }
        }
    }

    /// <summary>
    /// Start the WebRTC communication.
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
    /// Stop the WebRTC communication.
    /// </summary>
    public void StopWebRTC()
    {
		mRemoteNativeTexture.Destroy();
		mLocalNativeTexture.Destroy();

        Debug.Log("Stop WebRTC");
        using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
        {
            Connected = false;
            cls.CallStatic("StopWebrtc");
        }
    }

    /// <summary>
    /// Call another a Buddy that has been selected on the "Buddy selection" screen.
    /// </summary>
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
    /// Close a connection with a remote user.
    /// </summary>
    public void HangUp()
    {
        Debug.Log("Hang Up : " + mRemoteUser);
        if (mTextLog)
            mTextLog.text += "Hang Up : " + mRemoteUser + "\n";
        if (mConnectionState == CONNECTION.CONNECTING)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("my.maylab.unitywebrtc.Webrtc"))
            {
                Connected = false;
                cls.CallStatic("Hangup", mRemoteUser);
            }
        }
    }

    /// <summary>
    /// Sends a message to another user through a webrtc datachannel
    /// (unreliable/unordered) or through the messaging service (reliable)
    /// If you try to send a message through the datachannel and there is no
    /// directly active WebRTC connection the message will not be sent.
    /// </summary>
    /// <param name="iMessage">The message to send</param>
    public void SendWithDataChannel(string iMessage)
    {
        bool iThroughDataChannel = true;
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
    /// Called from Java to set the connection status with a remote user.
    /// </summary>
    /// <param name="iValue">1 for opened, 0 for closed.</param>
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
            Connected = false;
            if (mTextLog)
                mTextLog.text += "Webrtc connection OFF" + "\n";
        }
    }

    /// <summary>
    /// Call or hangup the remote channel depending on the connection state.
    /// </summary>
    public void ToggleConnection()
    {
        if (mConnectionState == CONNECTION.DISCONNECTING)
            Call();
        else
            HangUp();
    }

    /// <summary>
    /// Called by the RTC Java library when it receives a message.
    /// </summary>
    /// <param name="iMessage">The message that has been received.</param>
    public void onMessage(string iMessage)
    {
        Debug.Log(iMessage);
        if (mTextLog)
            mTextLog.text += "Receive message : " + iMessage + "\n";
    }

    /// <summary>
    /// Called by the RTC Java library to log to the Unity console.
    /// </summary>
    /// <param name="iMessage">The message to log.</param>
    public void onAndroidDebugLog(string iMessage)
    {
        //Debug.Log(iMessage);
        if (mTextLog)
            mTextLog.text += "Android Debug : " + iMessage + "\n";

        if (iMessage == "CONNECTED")
            Connected = true;
        else if (iMessage.Contains("onStateChange: CLOSED") && Connected) {
			StopWebRTC();
            menuManager.PreviousMenu();
		}
    }

    /// <summary>
    /// Called when the WebRTC changes the resolution of the local video stream.
    /// </summary>
    /// <param name="size">The new resolution as a string, e.g. "320*240".</param>
	public void onLocalTextureSizeChanged(string size)
	{
		Debug.Log ("WebRTC.onLocalTextureSizeChanged " + size);

		string[] cuts = size.Split(mResolutionSeparator);
		int width = Int32.Parse(cuts[0]);
		int height = Int32.Parse(cuts[1]);

        // Another texture with a new resolution has to be initialized.
		InitLocalTexture(width, height);
	}

    /// <summary>
    /// Called when the WebRTC changes the resolution of the remote video stream.
    /// </summary>
    /// <param name="size">The new resolution as a string, e.g. "320*240".</param>
	public void onRemoteTextureSizeChanged(string size)
	{
		Debug.Log ("WebRTC.onRemoteTextureSizeChanged " + size);

		string[] cuts = size.Split (mResolutionSeparator);
		int width = Int32.Parse (cuts [0]);
		int height = Int32.Parse (cuts [1]);

        // Another texture with a new resolution has to be initialized.
        InitRemoteTexture(width, height);
	}

    /// <summary>
    /// Called when WebRTC stats are received from the Java plugin.
    /// </summary>
    /// <param name="data">
    /// Data corresponding to the statistics gathered by the WebRTC:
    /// Local connection level, remote connection level, device performance level.
    /// </param>
	public void onWebRTCStats(string data)
	{
        mConnectionInfos = data;

		bool controlsDisabled = false;

		string[] cuts = mConnectionInfos.Split('|');
		float local = float.Parse(cuts[0]);
		float remote = float.Parse(cuts[1]);

        // Cut the control of the robot if the local or remote connection level is too low.
		if(local != -1 && remote != -1) {
			float threshold = 0.3f;
			if(local < threshold || remote < threshold) {
				controlsDisabled = true;
			}
		}
        
		remoteControl.ControlsDisabled = controlsDisabled;
		if(controlsDisabled) {
			joystick.SetActive(false);
			remoteMessage.SetActive(true);
		} else {
			joystick.SetActive(true);
			remoteMessage.SetActive(false);
		}
    }
}

