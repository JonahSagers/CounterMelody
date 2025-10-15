using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Server : MonoBehaviour {
    EventBasedNetListener netListener;
    NetManager netManager;
    NetDataWriter writer;
    private SongHandler songHandler;

    void Start() {
        SongHandler songHandler = GameObject.Find("Song Handler").GetComponent<SongHandler>();
        songHandler.turnDisplay.text = "Waiting for player... (Return to skip)";
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
            if (netManager.ConnectedPeersCount < 3 && songHandler.online){
                request.Accept();
                Debug.Log($"Player {netManager.ConnectedPeersCount} joined");
                if(netManager.ConnectedPeersCount == 2){
                    
                    songHandler.StartGame();
                }
            } else {
                request.Reject();
                Debug.Log($"Server full, player rejected");
            }
            
        };

        // netListener.PeerConnectedEvent += (client) => {
        //     //Debug.Log($"Client connected: {client}");
        // };

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
        if(Input.GetKeyDown(KeyCode.Return)){
            SongHandler songHandler = GameObject.Find("Song Handler").GetComponent<SongHandler>();
            if(songHandler.online){
                songHandler.online = false;
                songHandler.StartGame();
            }
            
        }
        netManager.PollEvents();
    }

    void OnDestroy() 
    {
        netManager.Stop(); // Clean shutdown
    }
}
