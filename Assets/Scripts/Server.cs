// Server.cs
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

public class Server : MonoBehaviour {
    EventBasedNetListener netListener;
    NetManager netManager;

    void Start() {
        Debug.Log("Starting server...");
        netListener = new EventBasedNetListener();

        // Accept all incoming connections
        netListener.ConnectionRequestEvent += (request) => {
            request.Accept();
        };

        netListener.PeerConnectedEvent += (client) => {
            Debug.Log($"Client connected: {client}");
        };

        netListener.PeerDisconnectedEvent += (client, info) => {
            Debug.Log($"Client disconnected: {info.Reason}");
        };

        netListener.NetworkErrorEvent += (endPoint, socketError) => {
            Debug.LogError($"Network error: {socketError}");
        };

        netManager = new NetManager(netListener);
        netManager.Start(9050); // <--- IMPORTANT: Start server on port 9050
    }

    void Update() {
        netManager.PollEvents();
    }

    void OnDestroy() {
        netManager.Stop(); // Clean shutdown
    }
}
