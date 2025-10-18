using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public Transform target;
    private Vector3 startPos;
    private Vector3 halfway;
    public float timestamp;
    private float timeFirst;
    private float timeSecond;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        halfway = Vector3.Lerp(startPos, target.position, 0.5f);
        timeSecond = 0.5f / PlayerData.scrollSpeed;
        timeFirst = 1 - timeSecond;
        Debug.Log("Time First:" + timeFirst);
        Debug.Log("Time Second:" + timeSecond);
    }

    // Update is called once per frame
    void Update()
    {
        float substep = Song.elapsed - (int)Song.elapsed;
        float substepScale = 1.2f - substep/5;
        transform.localScale = new Vector3(substepScale, substepScale, 1);
        float subSine = Mathf.Sin(Song.elapsed * Mathf.PI);
        transform.rotation = Quaternion.Euler(0, 0, subSine * Mathf.Abs(subSine) * 10);
        //TravelTime ranges 0 to 1
        float travelTime = 1 - (timestamp - Song.elapsedRaw)/(Song.timeSig * 60/Song.bpm);
        if(travelTime < timeFirst){
            transform.position = Vector3.Lerp(startPos, halfway, travelTime / timeFirst);
        } else {
            transform.position = Vector3.Lerp(halfway, target.position, (travelTime - timeFirst) / timeSecond);
        }
        if(Song.elapsedRaw > timestamp + 0.5f){
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
