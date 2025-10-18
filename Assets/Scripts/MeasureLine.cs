using UnityEngine;

public class MeasureLine : MonoBehaviour
{
    public Transform target;
    private Vector3 startPos;
    public float timestamp;
    private Vector3 halfway;
    private float timeFirst;
    private float timeSecond;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        halfway = Vector3.Lerp(startPos, target.position, 0.5f);
        timeSecond = 0.5f / PlayerData.scrollSpeed;
        timeFirst = 1 - timeSecond;
    }

    // Update is called once per frame
    void Update()
    {

        float travelTime = 1 - (timestamp - Song.elapsedRaw)/(Song.timeSig * 60/Song.bpm);
        if(travelTime < timeFirst){
            transform.position = Vector3.Lerp(startPos, halfway, travelTime / timeFirst);
        } else {
            transform.position = Vector3.Lerp(halfway, target.position, (travelTime - timeFirst) / timeSecond);
        }

        if(Song.elapsedRaw > timestamp){
            Destroy(gameObject);
        }
    }
}
