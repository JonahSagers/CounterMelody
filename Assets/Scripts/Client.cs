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
            Debug.LogError($"Connected to server: {server}");
        };

        netManager = new NetManager(netListener);
        netManager.Start(); // Don't forget to call .Start()!
        netManager.Connect("127.0.0.1", 9050, "");
    }

    // Update is called once per frame
    void Update() {
        netManager.PollEvents();
    }
}