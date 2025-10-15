using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject serverPre;
    public GameObject clientPre;
    public GameObject songHandler;
    public TMP_InputField ipField;
    private Client client;
    private Server server;
    public string serverIp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartServer()
    {
        server = Instantiate(serverPre).GetComponent<Server>();
        client = Instantiate(clientPre).GetComponent<Client>();
        client.Activate("localhost");
        SongHandler songScript = songHandler.GetComponent<SongHandler>();
        songScript.client = client;
        songScript.playerID = 1;
        songScript.Enable();
        gameObject.SetActive(false);
    }

    public void JoinServer()
    {
        if(ipField.text != ""){
            serverIp = ipField.text;
        }
        songHandler.SetActive(true);
        client = Instantiate(clientPre).GetComponent<Client>();
        client.Activate(serverIp);
        songHandler.GetComponent<SongHandler>().client = client;
        songHandler.GetComponent<SongHandler>().playerID = 2;
        gameObject.SetActive(false);
    }
}
