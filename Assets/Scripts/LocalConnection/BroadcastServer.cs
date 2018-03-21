using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BroadcastServer : UnityEngine.Networking.NetworkDiscovery
{	
	void Start()
	{
        broadcastPort = 47777;
        broadcastKey = 1000;
        broadcastVersion = 1;
        broadcastSubVersion = 1;
        broadcastData = "HELLO";
    }
}