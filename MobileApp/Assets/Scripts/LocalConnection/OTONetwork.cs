using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Net.Sockets;
#endif

public delegate void ToSendEventHandler(OTONetSender iSender, byte[] iData, int iNData);

public class OTONetSender : MonoBehaviour
{
    private event ToSendEventHandler ToSendEvent;

    public void SendData(byte[] iData, int iNdata)
    {
        if (ToSendEvent == null)
            throw new InvalidOperationException("Send event has not been defined");
        else
            ToSendEvent(this, iData, iNdata);
    }

    public void Subscribe(ToSendEventHandler iEvent)
    {
        ToSendEvent += iEvent;
    }

    public void Unsubscribe(ToSendEventHandler iEvent)
    {
        ToSendEvent -= iEvent;
    }
}

public class OTONetReceiver : MonoBehaviour
{
    public virtual void ReceiveData(byte[] iData, int iNData)
    {
        throw new NotImplementedException("ReceiveData not implemented");
    }
}

/// <summary>
/// Network server to handle the different channels : Video, command, sensor, accept (and sound streaming)
/// </summary>
public class OTONetwork : MonoBehaviour
{
    public string IP;
    public int Port;

    public List<int> channel_ids;
    public List<string> channel_name;
    public List<QosType> channel_type;
    public List<OTONetSender> channel_senders;
    public List<OTONetReceiver> channel_receivers;
    
    public bool Communicating { get; protected set; }
    public bool HasAPeer { get; protected set; }
    public bool IsServer;
    public bool IsClient { get { return !IsServer; } }
    public int NChannels { get { return channel_ids.Count; } }

    private const int MAX_FRAME_PER_UPDATE = 10;
    static ushort BUFFER_SIZE = 15000;

    private int mGenericHostId;
    private int mDistantHostID;
    private int mDistantConnectionID;
    private bool mIsCorou = false;
    private bool mIsConnected = false;
    byte[] recBuffer = new byte[BUFFER_SIZE];
    private ConnectionConfig mConfig = new ConnectionConfig();
    private GlobalConfig mGlobalConfig = new GlobalConfig();
    
    void OnEnable()
    {
        Configure();
        Connect();
    }

    void OnDisable()
    {
        Disconnect();
    }

    void Update()
    {
        if (!Communicating)
            return;
        else if (!mIsCorou) {
            StartCoroutine("UpdateOto");
            mIsCorou = true;
        }
    }

    IEnumerator UpdateOto()
    {
        while (true) {
            if (BUFFER_SIZE < (ushort.MaxValue - 1000) && mIsConnected)
                BUFFER_SIZE = ushort.MaxValue - 1;

            if (BUFFER_SIZE >= (ushort.MaxValue - 1000) && mIsConnected && !HasAPeer) {
                recBuffer = new byte[BUFFER_SIZE];
                HasAPeer = true;
            }

            ReceiveData();
            yield return null;
        }
    }

    private void Configure()
    {
        mConfig = new ConnectionConfig();

        // Verify the number of each lists
        if (channel_name.Count != NChannels
            || channel_type.Count != NChannels
            || channel_senders.Count != NChannels
            || channel_receivers.Count != NChannels)
            throw new IndexOutOfRangeException("Counter of channels lists are not equals");

        // Fill config
        string lDebugString = "";

        for (int i = 0; i < NChannels; i++) {
            channel_ids[i] = mConfig.AddChannel(channel_type[i]);

            if (channel_senders[i] != null)
                channel_senders[i].Subscribe(ToSendEvent);

            lDebugString += "Configure (" + i + ")";
            lDebugString += "\nid = " + channel_ids[i];
            lDebugString += "\nname = " + channel_name[i];
            lDebugString += "\ntype = " + channel_type[i];

            if (channel_receivers[i] != null)
                lDebugString += "\nreceiver = " + channel_receivers[i].gameObject.name;
            if (channel_senders[i] != null)
                lDebugString += "\nsender = " + channel_senders[i].gameObject.name;
            lDebugString += "\n\n";
        }
        Debug.Log(lDebugString);

        mConfig.PacketSize = ushort.MaxValue;
        mConfig.MaxSentMessageQueueSize = 1024;
        // Global config
        mGlobalConfig = new GlobalConfig();
        mGlobalConfig.MaxPacketSize = 65000;
        mGlobalConfig.ReactorMaximumReceivedMessages = 1;//valeur initiale a 256
        mGlobalConfig.ReactorMaximumSentMessages = 1;//valeur initiale a 256
    }

    private void Connect()
    {
        if (Communicating)
            Disconnect();
        
        if (IsServer) {
            NetworkTransport.Init(mGlobalConfig);
            HostTopology lTopology = new HostTopology(mConfig, 1);

            mGenericHostId = NetworkTransport.AddHost(lTopology, Port, null);
        }
        else {
            NetworkTransport.Init(mGlobalConfig);
            HostTopology lTopology = new HostTopology(mConfig, 1);
            mGenericHostId = NetworkTransport.AddHost(lTopology, 0);

            byte lError;
            mDistantConnectionID = NetworkTransport.Connect(mGenericHostId, IP, Port, 0, out lError);

            if (lError != 0) {
                throw new Exception("Error on connection : " + lError);
            }
        }
        Communicating = true;

        Debug.Log("Connected");
    }

    public void Disconnect()
    {
        if (!mIsConnected)
            return;

        //NetworkTransport.RemoveHost(_genericHostId);
        NetworkTransport.Shutdown();

        foreach (OTONetSender lOTOSender in channel_senders) {
            if (lOTOSender != null)
                lOTOSender.Unsubscribe(ToSendEvent);
        }

        Communicating = false;
        HasAPeer = false;
        mIsConnected = false;
        BUFFER_SIZE = 15000;
        mIsCorou = false;
        Debug.Log("Disconnected OTO");
    }

    private void ReceiveData()
    {
        int lRecChannelID;
        byte lError;
        int lDataSize;
        int lCount = 0;

        while (lCount++ < MAX_FRAME_PER_UPDATE) {
            NetworkEventType lRecData = NetworkTransport.Receive(out mDistantHostID, out mDistantConnectionID,
                                        out lRecChannelID, recBuffer, BUFFER_SIZE, out lDataSize, out lError);

            if (lError != 0)
                Debug.Log("Error = " + ((NetworkError)lError).ToString());

            if (lDataSize >= recBuffer.Length) {
                Debug.Log("Over Recbuffer : \nRecBuffer = " + recBuffer.Length + " Chanel = " + lRecChannelID + " Data size = " + lDataSize);
                return;
            }

            switch (lRecData) {

                case NetworkEventType.Nothing:
                    return;

                case NetworkEventType.ConnectEvent:
                    Debug.Log(string.Format("Connect from host {0} connection {1}", mDistantHostID, mDistantConnectionID));
                    mIsConnected = true;
                    break;

                case NetworkEventType.DataEvent:
                    HasAPeer = true;
                    for (int i = 0; i < NChannels; i++) {
                        if (channel_ids[i] == lRecChannelID) {
                            //  Debug.Log("recBufferSize = " + recBuffer.Length + "dataSize" + dataSize + " chanel" + recChannelID);
                            channel_receivers[i].ReceiveData(recBuffer, lDataSize);
                        }
                    }
                    break;

                case NetworkEventType.DisconnectEvent:
                    //HasAPeer = false;
                    //_isconnect = false;
                    //bufferSize = 15000;
                    //_isCorou = false;
                    //if (!IsServer)
                    //    Debug.Log(string.Format("DisConnect from host {0} connection {1}", _distantHostID, _distantConnectionID));
                    Disconnect();
                    break;
            }
        }
    }
    
    public int AddAChannel(string iName, QosType iType, OTONetReceiver iOTONetReceiver, OTONetSender iOTONetSender)
    {
        int lID = mConfig.AddChannel(iType);
        channel_ids.Add(lID);
        channel_type.Add(iType);
        channel_name.Add(iName);
        channel_receivers.Add(iOTONetReceiver);
        channel_senders.Add(iOTONetSender);

        return channel_ids.Count - 1;
    }

    public void DeleteAChannel(int iAt)
    {
        channel_ids.RemoveAt(iAt);
        channel_type.RemoveAt(iAt);
        channel_name.RemoveAt(iAt);
        channel_receivers.RemoveAt(iAt);
        channel_senders.RemoveAt(iAt);
    }

    public void DeleteAllChannels()
    {
        channel_ids = new List<int>();
        channel_type = new List<QosType>();
        channel_name = new List<string>();
        channel_receivers = new List<OTONetReceiver>();
        channel_senders = new List<OTONetSender>();
    }

    public void ResetCom()
    {
        Disconnect();
        Configure();
        Connect();
    }

    public int GetSendRate(out byte iError)
    {
        return NetworkTransport.GetPacketSentRate(mGenericHostId, mDistantConnectionID, out iError);
    }

    public int GetReceivedRate(out byte iError)
    {
        return NetworkTransport.GetPacketReceivedRate(mGenericHostId, mDistantConnectionID, out iError);
    }

    void ToSendEvent(OTONetSender iSender, byte[] iData, int iNData)
    {
        if (!HasAPeer)
            throw new InvalidOperationException("Tried to send without being connected!");

        byte lError;

        for (int i = 0; i < NChannels; i++) {
            if (channel_senders[i] == iSender) {
                NetworkTransport.Send(mGenericHostId, mDistantConnectionID, channel_ids[i], iData, iData.Length, out lError);
                
                if (lError != 0)
                    throw new Exception("Error on send : " + ((NetworkError)lError).ToString()
                        + " Chanel = " + channel_ids[i] + " Data size = " + iData.Length
                        + " _genericHostId " + mGenericHostId + " _distantConnectionID " + mDistantConnectionID);
            }
        }
    }

    private string GetString(byte[] iBytes)
    {
        char[] chars = new char[iBytes.Length / sizeof(char)];
        Buffer.BlockCopy(iBytes, 0, chars, 0, iBytes.Length);
        return new string(chars);
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(OTONetwork))]
public class OTONetworkTransportEditor : Editor
{

    GUILayoutOption[] _layoutOption1;
    GUILayoutOption[] _layoutOption2;
    GUILayoutOption[] _layoutOption3;

    OTONetworkTransportEditor()
    {
        _layoutOption1 = new GUILayoutOption[1];
        _layoutOption1[0] = GUILayout.MaxWidth(50);

        _layoutOption2 = new GUILayoutOption[1];
        _layoutOption2[0] = GUILayout.MaxWidth(150);

        _layoutOption3 = new GUILayoutOption[1];
        _layoutOption3[0] = GUILayout.MaxWidth(100);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        OTONetwork script = (OTONetwork)target;

        EditorGUI.BeginDisabledGroup(script.Communicating);

        /*		script.discovery = (NetworkDiscovery) EditorGUILayout.ObjectField("network discovery", script.discovery, typeof(NetworkDiscovery), true);
                if (!script.discovery)
                    Debug.Log ("discovery null");*/
        script.IsServer = EditorGUILayout.Toggle("Server ", script.IsServer);


        if (script.IsClient)
        {

            script.IP = EditorGUILayout.TextField("IP adress", script.IP);

        }
        script.Port = EditorGUILayout.IntField("Port", script.Port);

        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(5) });

        if (GUILayout.Button("Reset channels"))
        {
            script.DeleteAllChannels();
        }

        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        for (int i = 0; i < script.NChannels; i++)
        {

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID : " + script.channel_ids[i] + ", name :", _layoutOption3);
            script.channel_name[i] = EditorGUILayout.TextField(script.channel_name[i]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            script.channel_type[i] = (QosType)EditorGUILayout.EnumPopup(script.channel_type[i]);
            if (GUILayout.Button("X", _layoutOption1))
            {
                script.DeleteAChannel(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Receiver", _layoutOption1);
            script.channel_receivers[i] = (OTONetReceiver)EditorGUILayout.ObjectField(script.channel_receivers[i], typeof(OTONetReceiver), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sender", _layoutOption1);
            script.channel_senders[i] = (OTONetSender)EditorGUILayout.ObjectField(script.channel_senders[i], typeof(OTONetSender), true);
            EditorGUILayout.EndHorizontal();


            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        EditorGUILayout.Separator();

        if (GUILayout.Button("Add a channel"))
        {
            int at = (int)script.AddAChannel("", QosType.Reliable, null, null);
            script.channel_name[at] = "Channel " + (int)script.channel_ids[at];
        }


        EditorGUI.EndDisabledGroup();

        if (GUI.changed)
            EditorUtility.SetDirty(script);

    }
}


#endif