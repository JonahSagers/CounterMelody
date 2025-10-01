using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Client : MonoBehaviour {
    NetManager netManager;
    EventBasedNetListener netListener;
    public string serverIp;

    // Start is called before the first frame update
    public void Activate(string ip) {
        serverIp = ip;
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
        netManager.Start();
        netManager.Connect(serverIp, 9050, "");
        StartCoroutine(ClientLoop());
    }

    // Update is called once per frame
    public IEnumerator ClientLoop() {
        //Replace true with a server check maybe?
        while(true){
            netManager.PollEvents();
            yield return 0;
        }
    }

    void OnDestroy() {
        netManager.Stop();
    }
}