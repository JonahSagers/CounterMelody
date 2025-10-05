using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject serverPre;
    public GameObject clientPre;
    public GameObject songHandler;
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
        songHandler.SetActive(true);
        server = Instantiate(serverPre).GetComponent<Server>();
        client = Instantiate(clientPre).GetComponent<Client>();
        client.Activate("localhost");
        songHandler.GetComponent<SongHandler>().client = client;
        songHandler.GetComponent<SongHandler>().playerID = 1;
        gameObject.SetActive(false);
    }

    public void JoinServer()
    {
        songHandler.SetActive(true);
        client = Instantiate(clientPre).GetComponent<Client>();
        client.Activate(serverIp);
        songHandler.GetComponent<SongHandler>().client = client;
        songHandler.GetComponent<SongHandler>().playerID = 2;
        gameObject.SetActive(false);
    }
}
