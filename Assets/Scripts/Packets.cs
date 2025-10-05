using UnityEngine;

[System.Serializable]
public class GenericPacket {
    public string type;
}

[System.Serializable]
public struct HitPacket {
    public string type;
    public int player;
    public float timestamp;
    public string keyName;
}
