using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public Vector3 target;
    private Vector3 startPos;
    private float travelTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        travelTime += Time.deltaTime * (Song.bpm / 60);
        transform.position = Vector3.Lerp(startPos, target, travelTime / 8);
        if(travelTime > 8.1f){
            Debug.Log("Note Missed");
            Destroy(gameObject);
        }
    }
}
