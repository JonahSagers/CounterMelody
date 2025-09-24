using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    public Transform playerLeft;
    public Transform playerRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform target = null;
        if(Song.gameState == 0){
            target = playerRight;
        } else if(Song.gameState == 1){
            target = playerLeft;
        } else if(Song.gameState == 2){
            target = playerLeft;
        } else if(Song.gameState == 3){
            target = playerRight;
        }
        if(target != null){
            transform.position = target.transform.position + Vector3.up * (2 + ((Song.elapsed % 2 < 1) ? 0 : 0.2f));
        }
    }
}
