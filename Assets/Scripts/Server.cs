// Server.cs
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

public class Server : MonoBehaviour {
    EventBasedNetListener netListener;
    NetManager netManager;

    // Start is called before the first frame update
    void Start() {
        Debug.LogError("starting server");
        netListener = new EventBasedNetListener();

        netListener.ConnectionRequestEvent += (request) => {
            request.Accept();
        };

        netListener.PeerConnectedEvent += (client) => {
            Debug.LogError($"Client connected: {client}");
        };

        netManager = new NetManager(netListener);
    }

    void Update() {
        netManager.PollEvents();
    }
}