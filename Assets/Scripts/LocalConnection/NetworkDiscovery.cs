using UnityEngine;
using UnityEngine.Networking;

public delegate void IPconfig(string iIP);

/// <summary>
/// Detects local robots via a broadcast server that emits a signal every second
/// </summary>
public class NetworkDiscovery : MonoBehaviour
{
    public int hostId { get { return mHostId; } set { mHostId = value; } }

    // config data
    [SerializeField]
    private int broadcastPort = 47777;

    [SerializeField]
    private int broadcastKey = 1000;

    [SerializeField]
    private int broadcastVersion = 1;

    [SerializeField]
    private int broadcastSubVersion = 1;

    [SerializeField]
    private string broadcastData = "HELLO";

    [SerializeField]
    private bool isServer = false;

    public IPconfig changeIP;

    private const int MAX_BROADCAST_SIZE = 1024;

    private bool mIsRunning = false;
    private int mHostId = -1;
    private byte[] msgOutBuffer = null;
    private byte[] msgInBuffer = null;

    private byte[] GetBytes(string iString)
    {
        byte[] lBytes = new byte[iString.Length * sizeof(char)];
        System.Buffer.BlockCopy(iString.ToCharArray(), 0, lBytes, 0, lBytes.Length);
        return lBytes;
    }

    private string GetString(byte[] iBytes)
    {
        char[] lChars = new char[iBytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(iBytes, 0, lChars, 0, iBytes.Length);
        return new string(lChars);
    }

    public bool Initialize()
    {
        if (broadcastData.Length >= MAX_BROADCAST_SIZE) {
            Debug.LogError("NetworkDiscovery Initialize - data too large. max is " + MAX_BROADCAST_SIZE);
            return false;
        }

        if (!NetworkTransport.IsStarted)
            NetworkTransport.Init();

        if (NetworkManager.singleton != null)
            broadcastData = "NetworkManager:" + NetworkManager.singleton.networkAddress + ":" + NetworkManager.singleton.networkPort;

        //DontDestroyOnLoad(gameObject);
        msgOutBuffer = GetBytes(broadcastData);
        msgInBuffer = new byte[MAX_BROADCAST_SIZE];
        return true;
    }

    // listen for broadcasts
    public bool StartAsClient()
    {
        if (mHostId != -1 || mIsRunning) {
            Debug.LogWarning("NetworkDiscovery StartAsClient already started");
            return false;
        }

        ConnectionConfig lCC = new ConnectionConfig();
        lCC.AddChannel(QosType.Unreliable);
        HostTopology lDefaultTopology = new HostTopology(lCC, 1);
        mHostId = NetworkTransport.AddHost(lDefaultTopology, broadcastPort);

        if (mHostId == -1) {
            Debug.LogError("NetworkDiscovery StartAsClient - addHost failed");
            return false;
        }

        byte lError;
        NetworkTransport.SetBroadcastCredentials(mHostId, broadcastKey, broadcastVersion, broadcastSubVersion, out lError);

        mIsRunning = true;
        isServer = false;
        Debug.Log("StartAsClient Discovery listening");
        return true;
    }

    public bool StartAsClient(int iGenericHost)
    {
        mHostId = iGenericHost;

        if (mHostId != -1 || mIsRunning) {
            Debug.LogWarning("NetworkDiscovery StartAsClient already started");
            return false;
        }

        byte lError;
        NetworkTransport.SetBroadcastCredentials(mHostId, broadcastKey, broadcastVersion, broadcastSubVersion, out lError);

        mIsRunning = true;
        isServer = false;
        Debug.Log("StartAsClient Discovery listening");
        return true;
    }

    // perform actual broadcasts
    public bool StartAsServer()
    {
        if (mHostId != -1 || mIsRunning) {
            Debug.LogWarning("NetworkDiscovery StartAsServer already started");
            return false;
        }

        ConnectionConfig lCC = new ConnectionConfig();
        lCC.AddChannel(QosType.Unreliable);
        HostTopology lDefaultTopology = new HostTopology(lCC, 1);

        mHostId = NetworkTransport.AddHost(lDefaultTopology, 0);
        Debug.Log("hostId value " + mHostId);

        if (mHostId == -1) {
            Debug.LogError("NetworkDiscovery StartAsServer - addHost failed");
            return false;
        }

        byte lError;

        if (!NetworkTransport.StartBroadcastDiscovery(mHostId, broadcastPort,
                broadcastKey, broadcastVersion, broadcastSubVersion, msgOutBuffer, msgOutBuffer.Length, 1000, out lError)) {
            Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + lError);
            return false;
        }

        mIsRunning = true;
        isServer = true;
        Debug.Log("StartAsServer Discovery broadcasting");
        return true;
    }

    public bool StartAsServer(int iGenericHost)
    {
        Debug.Log("start server broadcast");
        mHostId = iGenericHost;

        if (mHostId == -1) {
            Debug.LogError("NetworkDiscovery StartAsServer - addHost failed");
            return false;
        }

        byte lError;

        if (!NetworkTransport.StartBroadcastDiscovery(mHostId, broadcastPort, broadcastKey,
                broadcastVersion,broadcastSubVersion, msgOutBuffer, msgOutBuffer.Length, 1000, out lError)) {
            Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + lError);
            return false;
        }

        mIsRunning = true;
        isServer = true;
        Debug.Log("StartAsServer Discovery broadcasting");
        return true;
    }

    public void StopBroadcast()
    {
        if (mHostId == -1) {
            Debug.LogError("NetworkDiscovery StopBroadcast not initialized");
            return;
        }

        if (!mIsRunning) {
            Debug.LogWarning("NetworkDiscovery StopBroadcast not started");
            return;
        }

        if (isServer)
            NetworkTransport.StopBroadcastDiscovery();

        NetworkTransport.RemoveHost(mHostId);
        NetworkTransport.Shutdown();
        mHostId = -1;
        mIsRunning = false;
        isServer = false;
        Debug.Log("Stopped Discovery broadcasting");
    }

    void Update()
    {
        if (mHostId == -1)
            return;

        if (isServer)
            return;

        int lConnectionId;
        int lChannelId;
        int lReceivedSize;
        byte lError;

        NetworkEventType lNetworkEvent = NetworkEventType.DataEvent;

        do {
            lNetworkEvent = NetworkTransport.ReceiveFromHost(mHostId, out lConnectionId, out lChannelId,
                msgInBuffer, MAX_BROADCAST_SIZE, out lReceivedSize, out lError);

            if (lNetworkEvent == NetworkEventType.BroadcastEvent) {
                NetworkTransport.GetBroadcastConnectionMessage(mHostId, msgInBuffer, MAX_BROADCAST_SIZE, out lReceivedSize, out lError);
                string lSenderAddr;
                int lSenderPort;
                NetworkTransport.GetBroadcastConnectionInfo(mHostId, out lSenderAddr, out lSenderPort, out lError);
                OnReceivedBroadcast(lSenderAddr, GetString(msgInBuffer));
            }

            else if(lNetworkEvent == NetworkEventType.DataEvent) {
                string lIP = GetString(msgInBuffer);
                Debug.Log("Received message " + lIP);
            }

        } while (lNetworkEvent != NetworkEventType.Nothing);
    }

    public virtual void OnReceivedBroadcast(string iFromAddress, string iData)
    {
        //Debug.Log("Got broadcast from [" + fromAddress + "] " + data);
        string[] items = iFromAddress.Split(':');

        for (int i = 0; i < items.Length; i++)
            Debug.Log("item" + i + " : " + items[i]);

        changeIP(items[3]);
    }

    void OnDestroy()
    {
        if (isServer)
            NetworkTransport.StopBroadcastDiscovery();
    }
}