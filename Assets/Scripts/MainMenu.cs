using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject serverPre;
    public GameObject clientPre;
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
        gameObject.SetActive(false);
    }

    public void JoinServer()
    {
        client = Instantiate(clientPre).GetComponent<Client>();
        client.Activate(serverIp);
        gameObject.SetActive(false);
    }
}
