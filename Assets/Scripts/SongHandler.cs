using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SongHandler : MonoBehaviour
{
    public List<(GameObject obj, float timing, int lane)> noteList;
    [Header("Components")]
    public TextMeshProUGUI timingDisplay;
    public GameObject notePre;
    public List<GameObject> leftLanes;
    public List<GameObject> rightLanes;

    [Header("Parameters")]
    public float bpm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Song is a static class which is accessible from all scripts, but not the inspector
        //BPM here is just used to set the static version
        Song.bpm = bpm;
        StartCoroutine(Metronome());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Metronome()
    {
        while(true){
            SpawnNote();
            while(Song.elapsed < 8){
                timingDisplay.text = (Song.elapsed + 1).ToString();
                Song.elapsed = (Song.elapsed + Time.deltaTime * (Song.bpm / 60)) % 8;
                yield return 0;
            }
        }
    }

    public void SpawnNote()
    {
        GameObject note = Instantiate(notePre, rightLanes[0].transform.position, Quaternion.identity);
        note.GetComponent<NoteMove>().target = leftLanes[0].transform.position;
        Debug.Log("Note Spawned");
    }
}
