using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public Vector3 target;
    private Vector3 startPos;
    public float travelTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        travelTime += Time.deltaTime * (Song.bpm / 60);
        transform.position = Vector3.Lerp(startPos, target, (travelTime - Song.timeSig + Song.timeSig / Song.scrollSpeed) / (Song.timeSig / Song.scrollSpeed));
        if(travelTime > 8.2f){
            SongHandler songHandler = GameObject.Find("Song Handler").GetComponent<SongHandler>();
            for(int i = 0; i < songHandler.noteList.Count; i++){
                if(songHandler.noteList[i].obj == gameObject){
                    songHandler.noteList.RemoveAt(i);
                    break;
                }
            }
            Destroy(gameObject);
        }
    }
}
