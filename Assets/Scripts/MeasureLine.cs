using UnityEngine;

public class MeasureLine : MonoBehaviour
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
        travelTime += (Time.realtimeSinceStartup - lastTime) * (Song.bpm / 60);
        lastTime = Time.realtimeSinceStartup;
        transform.position = Vector3.Lerp(startPos, target.position, (travelTime - Song.timeSig + Song.timeSig / Song.scrollSpeed) / (Song.timeSig / Song.scrollSpeed));
        if(travelTime > 8){
            Destroy(gameObject);
        }
    }
}
