using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Client : MonoBehaviour {
    NetManager netManager;
    EventBasedNetListener netListener;

    // Start is called before the first frame update
    void Start() {
        netListener = new EventBasedNetListener();
        netListener.PeerConnectedEvent += (server) => {
            Debug.Log($"Connected to server: {server}");
        };
        netListener.PeerDisconnectedEvent += (server, info) => {
            Debug.Log($"Disconnected: {info.Reason}");
        };
        netListener.NetworkErrorEvent += (endPoint, socketError) => {
            Debug.LogError($"Network error: {socketError}");
        };

        netManager = new NetManager(netListener);
        netManager.Start(); // Don't forget to call .Start()!
        netManager.Connect("localhost", 9050, "");
    }

    // Update is called once per frame
    void Update() {
        netManager.PollEvents();
    }

    void OnDestroy() {
        netManager.Stop();
    }
}