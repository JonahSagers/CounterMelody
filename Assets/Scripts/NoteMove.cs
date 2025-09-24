using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public Transform target;
    private Vector3 startPos;
    public float travelTime;
    private float lastTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        lastTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        float substep = Song.elapsed - (int)Song.elapsed;
        float substepScale = 1.2f - substep/5;
        transform.localScale = new Vector3(substepScale, substepScale, 1);
        float subSine = Mathf.Sin(Song.elapsed * Mathf.PI);
        transform.rotation = Quaternion.Euler(0, 0, subSine * Mathf.Abs(subSine) * 10);

        travelTime += (Time.realtimeSinceStartup - lastTime) * (Song.bpm / 60);
        lastTime = Time.realtimeSinceStartup;
        transform.position = Vector3.Lerp(startPos, target.position, (travelTime - Song.timeSig + Song.timeSig / Song.scrollSpeed) / (Song.timeSig / Song.scrollSpeed));
        if(travelTime > 8.5f){
            SongHandler songHandler = GameObject.Find("Song Handler").GetComponent<SongHandler>();
            for(int i = 0; i < songHandler.noteList.Count; i++){
                if(songHandler.noteList[i].obj == gameObject){
                    songHandler.noteList.RemoveAt(i);
                    break;
                }
            }
            target.parent.GetComponent<PlayerController>().TakeDamage(1f);
            Destroy(gameObject);
        }
    }
}
