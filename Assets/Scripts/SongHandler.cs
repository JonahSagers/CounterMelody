using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SongHandler : MonoBehaviour
{
    public List<(GameObject obj, float timing, int lane)> noteList = new List<(GameObject, float, int)>();
    [Header("Components")]
    public TextMeshProUGUI timingDisplay;
    public GameObject notePre;
    public List<GameObject> leftLanes;
    public List<GameObject> rightLanes;

    [Header("Parameters")]
    public float bpm;
    public float timeSig;
    public float scrollSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Song is a static class which is accessible from all scripts, but not the inspector
        //BPM here is just used to set the static version
        Song.bpm = bpm;
        Song.timeSig = timeSig;
        Song.scrollSpeed = scrollSpeed;
        StartCoroutine(Metronome());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)){
            RegisterHit(0);
        }
        if(Input.GetKeyDown(KeyCode.D)){
            RegisterHit(1);
        }
        if(Input.GetKeyDown(KeyCode.A)){
            RegisterHit(2);
        }
        if(Input.GetKeyDown(KeyCode.S)){
            RegisterHit(3);
        }
    }

    public IEnumerator Metronome()
    {
        while(true){
            SpawnNote(2, Random.Range(0, 4));
            while(Song.elapsed < Song.timeSig){
                timingDisplay.text = (Song.elapsed + 1).ToString();
                Song.elapsed = (Song.elapsed + Time.deltaTime * (Song.bpm / 60));
                yield return 0;
            }
            Song.elapsed %= Song.timeSig;
        }
    }

    public void SpawnNote(int player, int lane)
    {
        List<GameObject> spawnLanes;
        List<GameObject> targetLanes;
        if(player == 1){
            spawnLanes = leftLanes;
            targetLanes = rightLanes;
        } else {
            targetLanes = leftLanes;
            spawnLanes = rightLanes;
        }
        GameObject note = Instantiate(notePre, spawnLanes[lane].transform.position, Quaternion.identity);
        note.GetComponent<NoteMove>().target = targetLanes[lane].transform.position;
        noteList.Add((note, Song.elapsed, lane));
    }

    //In theory we shouldn't need to differentiate between players for this one
    public void RegisterHit(int lane)
    {
        for(int i = 0; i < noteList.Count; i++){
            //split into two if statements to potentially do judgements
            if(noteList[i].lane == lane){
                float error = Mathf.Abs(noteList[i].timing - Song.elapsed);
                Debug.Log(error);
                //really wish I knew how to do this in one check
                //lmk if you think of anything
                if(error < 0.2f || error > Song.timeSig - 0.2f){
                    Debug.Log("Note hit in lane " + lane + " with error " + ((error < 1) ? error : Song.timeSig - error));
                    noteList.RemoveAt(i);
                    break;
                }
            }
        }
        
    }
}
