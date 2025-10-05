using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Server : MonoBehaviour {
    EventBasedNetListener netListener;
    NetManager netManager;
    NetDataWriter writer;

    void Start() {
        Debug.Log("Starting server...");
        netListener = new EventBasedNetListener();
        writer = new NetDataWriter();

        netListener.NetworkReceiveEvent += (peer, reader, method, channel) => {
            string msg = reader.GetString();
            Debug.Log($"[Client -> Server] {msg}");
            reader.Recycle();

            // Example: rebroadcast to other clients
            foreach (var p in netManager.ConnectedPeerList) {
                if (p != peer) {
                    writer.Reset();
                    writer.Put(msg);
                    p.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        };

        netListener.ConnectionRequestEvent += (request) => {
            //Check whether this includes the host client
            if (netManager.ConnectedPeersCount < 3){
                request.Accept();
                Debug.Log($"Player joined");
            } else {
                request.Reject();
                Debug.Log($"Server full, player rejected");
            }
            
        };

        netListener.PeerConnectedEvent += (client) => {
            //Debug.Log($"Client connected: {client}");
            PlayerJoin();
        };

        netListener.PeerDisconnectedEvent += (client, info) => {
            Debug.Log($"Client disconnected: {info.Reason}");
        };

        netListener.NetworkErrorEvent += (endPoint, socketError) => {
            Debug.LogError($"Network error: {socketError}");
        };

        netManager = new NetManager(netListener);
        netManager.Start(9050); //Start on port 9050
        Debug.Log("Server started!");
    }

    void Update()
     {
        netManager.PollEvents();
    }

    void OnDestroy() 
    {
        netManager.Stop(); // Clean shutdown
    }

    void PlayerJoin()
    {

    }
}
