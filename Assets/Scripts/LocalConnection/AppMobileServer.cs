using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;

public delegate void FinishConnection();
public delegate void BuddyConnected(string iBuddyIP);
public delegate void BuddyDisconnected(string iBuddyIP);

/// <summary>
/// Class for custom messages between the phone and Buddy on a local connection
/// </summary>
public class BeginMessage : MessageBase
{
    public string mContent;
    public string mBuddyIP;
    public string mHostIP;
}

/// <summary>
/// This class' purpose is to manage all the network interface to detect a local Buddy
/// </summary>
/// <remarks>
/// Here is how it works more specifically :
///     - The mobile phone starts a broadcast server and an other local server
///     - When a Buddy hears the broadcast, it will get the IP of the phone and connect to its server
///     - When the user wishes to launch a remote control, the server will send a custom message
///         to the selected Buddy to ask him to start the remote control server
/// 
/// Of course, this method has flaws. The main one being that a Buddy can only connect to one local Phone server.
/// So it can be a problem when there are multiple phones trying to remotely control a Buddy on a same local network.
/// Why did I choose to implement it this way ? Mainly because Unity Network layer sucks, and also for not having to make some weird
/// broadcast server - client switching loop.
/// </remarks>
public class AppMobileServer : MonoBehaviour
{
    public string BuddyIP { get { return mClientBuddyIP; } set { mClientBuddyIP = value; } }

    //[SerializeField]
    //private NetworkDiscovery discovery;

	[SerializeField]
	private BroadcastServer discovery;

    private const short BEGIN_MESSAGE = 1004;
    private const short CHAT_MSG = 1010;

    private string mClientBuddyIP;
    private List<NetworkConnection> mConnectionsList = new List<NetworkConnection>();
    public FinishConnection OnConnectionEstablished;
    public BuddyConnected OnNewBudyConnected;
    public BuddyDisconnected OnBuddyDisconnected;
    
    void OnEnable()
    {
        //Start the network discovery of local Buddies
        discovery.gameObject.SetActive(true);
        StartServerMessage();
        StartBroadcast();
        Debug.Log("Broadcast Id " + discovery.hostId);
        Debug.Log("NetworkServer Id " + NetworkServer.serverHostId);
    }

    private void StartBroadcast()
    {
        //Self-explanatory
        discovery.Initialize();
        discovery.StartAsServer();
    }

    private void StopBroadcast()
    {
        //Self-explanatory
        Debug.Log("Broadcast id " + discovery.hostId);
        discovery.StopBroadcast();
    }

    public void StartServerMessage()
    {
        //Start a custom server for custom messages handling.
        //Local Buddies connect to it when they hear a broadcast and wait for a BeginMessage
        Debug.Log("Starting Server message");
        NetworkServer.Listen(48888);
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        NetworkServer.RegisterHandler(BEGIN_MESSAGE, OnReady);
        NetworkServer.RegisterHandler(CHAT_MSG, OnChatMessage);
    }

    /// <summary>
    /// Called when a new Buddy connects to the local listening server.
    /// </summary>
    /// <param name="iNetMsg">The network message received from a Buddy.</param>
    private void OnConnected(NetworkMessage iNetMsg)
    {
        //When a new Buddy connects to server, register its address to the list
        Debug.Log("Buddy " + iNetMsg.conn.address.Split(':')[3] + " connected.");
        bool lFound = false;

        //Check if Buddy already exists in the connection list
        foreach(NetworkConnection lNetCo in mConnectionsList) {
            if (lNetCo == iNetMsg.conn) {
                lFound = true;
                break;
            }
        }
        if(!lFound) {
            mConnectionsList.Add(iNetMsg.conn);
            OnNewBudyConnected(iNetMsg.conn.address.Split(':')[3]);
        }
    }

    /// <summary>
    /// Called when a Buddy discconnects from the local server.
    /// </summary>
    /// <param name="iNetMsg">The disconnection message received from a Buddy.</param>
    private void OnDisconnected(NetworkMessage iNetMsg)
    {
        //Remove disconnected Buddy from the list
        Debug.Log("Buddy " + iNetMsg.conn.address + " disconnected");
        mConnectionsList.Remove(iNetMsg.conn);
        OnBuddyDisconnected(iNetMsg.conn.address);
        //This is some voodoo level magic. Implemented by Unity, we can't access to the disconnected IP ...
        //Maybe this is the reason why there are duplicates in the IP list ?
    }

    private void OnReady(NetworkMessage iNetMsg)
    {
        //Useless function on Phone side but necessary for proper functionning of NetworkServer
        Debug.Log("Received ready from "
            + iNetMsg.ReadMessage<StringMessage>());
    }

    private void OnChatMessage(NetworkMessage iNetMsg)
    {
        //Useless function on Phone side but necessary for proper functionning of NetworkServer
        StringMessage lChatMsg = iNetMsg.ReadMessage<StringMessage>();
        Debug.Log("Received message: " + lChatMsg.value);
    }

    /// <summary>
    /// Get the list of locally connected Buddies.
    /// </summary>
    /// <returns>The list of locally connected Buddies.</returns>
    public List<string> GetBuddyConnectedList()
    {
        List<string> lBuddyList = new List<string>();

        foreach (NetworkConnection lCo in NetworkServer.connections) {
            if (lCo == null)
                continue;
            lBuddyList.Add(lCo.address.Split(':')[3]);
            Debug.Log("Found connected Buddy " + lCo.address.Split(':')[3]);
        }

        return lBuddyList;
    }

    private void CutNetwork()
    {
        //Self-explanatory
        NetworkServer.Shutdown();
        StopBroadcast();
    }

    public void StartTelepresence()
    {
        Debug.Log("Connecting to Buddy " + mClientBuddyIP);
        int lBuddyID = 0;

        //Find the remote Buddy we want to connect to
        foreach (NetworkConnection lCo in NetworkServer.connections) {
            if (lCo == null)
                continue;
            Debug.Log("Buddy registered " + lCo.address);
            if (lCo.address.Contains(mClientBuddyIP)) {
                Debug.Log("Found Buddy to connect. ID : " + lCo.connectionId);
                lBuddyID = lCo.connectionId;
                break;
            }
        }

        //Send selected robot a message to start remote control server
        Debug.Log("Sending custom message to Buddy " + lBuddyID);
        BeginMessage lMsg = new BeginMessage();
        lMsg.mContent = "Hey !";
        lMsg.mBuddyIP = mClientBuddyIP;
        lMsg.mHostIP = Network.player.ipAddress;
        
        //Do not cut the broadcast before sending the message to Buddy.
        //Or message won't be sent, and everything crashes ...
        Debug.Log("Network server Id " + NetworkServer.serverHostId);
        NetworkServer.SendToClient(lBuddyID, BEGIN_MESSAGE, lMsg);

        StartCoroutine(LaunchTelepresence());
    }

    private IEnumerator LaunchTelepresence()
    {
        //We have to wait a bit to make sure the message is sent before stopping Broadcast and Network servers
        yield return new WaitForSeconds(1f);

        CutNetwork();
        discovery.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        Debug.Log("Broadcast shutdown. Starting OTONetwork telepresence");
        OnConnectionEstablished();
    }

    public void SendChatMessage(string iMsg)
    {
        //Self-explanatory
        int lBuddyID = 0;

        //Find the remote Buddy we want to send a message to
        foreach (NetworkConnection lCo in NetworkServer.connections)
        {
            if (lCo == null)
                continue;
            //Debug.Log("Buddy registered " + lCo.address);
            if (lCo.address.Contains(mClientBuddyIP))
            {
                lBuddyID = lCo.connectionId;
                break;
            }
        }

        StringMessage lChatMsg = new StringMessage(iMsg);
        Debug.Log("Sending chat message :" + lChatMsg.value);
        NetworkServer.SendToClient(lBuddyID, CHAT_MSG, lChatMsg);
    }

    private byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }
}
