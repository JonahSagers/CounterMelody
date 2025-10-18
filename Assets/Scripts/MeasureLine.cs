using UnityEngine;

public class MeasureLine : MonoBehaviour
{
    public Transform target;
    private Vector3 startPos;
    public float timestamp;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector3.Lerp(startPos, target.position, 1 - (timestamp - Song.elapsedRaw)/(8 * 60/Song.bpm));
        if(Song.elapsedRaw > timestamp){
            Destroy(gameObject);
        }
    }
}
