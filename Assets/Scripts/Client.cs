using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client : MonoBehaviour {
    NetManager netManager;
    NetPeer serverPeer;
    EventBasedNetListener netListener;
    NetDataWriter writer;
    public string serverIp;
    private SongHandler songHandler;

    // Start is called before the first frame update
    public void Activate(string ip)
    {
        serverIp = ip;
        netListener = new EventBasedNetListener();
        writer = new NetDataWriter();
        songHandler = GameObject.Find("Song Handler").GetComponent<SongHandler>();

        netListener.NetworkReceiveEvent += (peer, reader, method, channel) => {
            string msg = reader.GetString();
            Debug.Log($"[Server -> Client] {msg}");
            GenericPacket genericPacket = JsonUtility.FromJson<GenericPacket>(msg);

            if(genericPacket.type == "HitPacket"){
                HitPacket hitPacket = JsonUtility.FromJson<HitPacket>(msg);
                songHandler.PacketInput(hitPacket);
            } else if(genericPacket.type == "SongPacket"){
                SongPacket songPacket = JsonUtility.FromJson<SongPacket>(msg);
                StartCoroutine(songHandler.Metronome());
            }

            reader.Recycle();
        };
        netListener.PeerConnectedEvent += (server) => {
            Debug.Log($"Connected to server: {server}");
            serverPeer = server;
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
    public IEnumerator ClientLoop()
    {
        //Replace true with a server check maybe?
        while(true){
            netManager.PollEvents();
            yield return 0;
        }
    }

    public void SendPacket(string packet)
    {
        if (serverPeer != null && serverPeer.ConnectionState == ConnectionState.Connected) {
            writer.Reset();
            writer.Put(packet); 
            serverPeer.Send(writer, DeliveryMethod.Unreliable);
        }
    }

    void OnDestroy() {
        netManager.Stop();
    }
}