using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SongHandler : MonoBehaviour
{
    public List<(GameObject obj, float timing, int lane)> noteList = new List<(GameObject, float, int)>();
    [Header("Components")]
    public TextMeshProUGUI timingDisplay;
    public TextMeshProUGUI turnDisplay;
    public RectTransform metronomeDisplay;
    public GameObject notePre;
    public List<GameObject> leftLanes;
    public List<GameObject> rightLanes;
    private PlayerControls playerControls;

    [Header("Parameters")]
    public float bpm;
    public float timeSig;
    public float scrollSpeed;
    public int gameState;
    //Game state list:
        // 1: right player attack
        // 2: left player defend
        // 3: left player attack
        // 4: right player defend

    [Header("Data")]
    public float startTime;
    public int turns;
    public List<float> allowedSubsteps;


    void Start()
    {
        gameState = -1;
        //Song is a static class which is accessible from all scripts, but not the inspector
        //BPM here is just used to set the static version
        Song.bpm = bpm;
        Song.timeSig = timeSig;
        Song.scrollSpeed = scrollSpeed;

        playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.LeftLane0.performed += ctx => GetInput(ctx);
        playerControls.Player.LeftLane1.performed += ctx => GetInput(ctx);
        playerControls.Player.LeftLane2.performed += ctx => GetInput(ctx);
        playerControls.Player.LeftLane3.performed += ctx => GetInput(ctx);
        playerControls.Player.RightLane0.performed += ctx => GetInput(ctx);
        playerControls.Player.RightLane1.performed += ctx => GetInput(ctx);
        playerControls.Player.RightLane2.performed += ctx => GetInput(ctx);
        playerControls.Player.RightLane3.performed += ctx => GetInput(ctx);
        
        StartCoroutine(Metronome());
    }

    private void GetInput(InputAction.CallbackContext context)
    {
        // Get the key that was pressed
        string pressedKey = context.control.path;

        // Get the exact time the event occurred (in seconds since the game started)
        float pressRaw = (float)context.time - startTime;

        // Convert the time to milliseconds
        float pressTime = Mathf.Repeat((pressRaw / 60.0f * Song.bpm), Song.timeSig);

        string keyName = context.control.name;

        int lane = -1;
        if(keyName == "w"){
            lane = 0;
        } else if(keyName == "a"){
            lane = 1;
        } else if(keyName == "d"){
            lane = 2;
        } else if(keyName == "s"){
            lane = 3;
        }
        if(lane >= 0){
            if((gameState == 1 && Song.elapsed < Song.timeSig - 0.1f) || (gameState == 0 && Song.elapsed > Song.timeSig - 0.1f)){
                RegisterHit(lane, pressTime);
            } else if((gameState == 2 && Song.elapsed < Song.timeSig - 0.1f) || (gameState == 1 && Song.elapsed > Song.timeSig - 0.1f)){
                SpawnNote(1, lane, pressTime);
            }
            return;
        }
        if(keyName == "upArrow"){
            lane = 0;
        } else if(keyName == "leftArrow"){
            lane = 1;
        } else if(keyName == "rightArrow"){
            lane = 2;
        } else if(keyName == "downArrow"){
            lane = 3;
        }
        if(lane >= 0){
            if((gameState == 3 && Song.elapsed < Song.timeSig - 0.1f) || (gameState == 2 && Song.elapsed > Song.timeSig - 0.1f)){
                RegisterHit(lane, pressTime);
            } else if((gameState == 0 && Song.elapsed < Song.timeSig - 0.1f) || (gameState == 3 && Song.elapsed > Song.timeSig - 0.1f)){
                SpawnNote(2, lane, pressTime);
            }
            return;
        }
    }

    public IEnumerator Metronome()
    {
        //First six lines are just debug, remove later
        int countdown = 4;
        while(countdown > 0){
            turnDisplay.text = countdown.ToString();
            yield return new WaitForSeconds(0.5f);
            countdown -= 1;
        }
        turnDisplay.text = "Right Player Attack";
        
        startTime = Time.realtimeSinceStartup;
        bool newMeasure;
        gameState = 0;
        allowedSubsteps.Add(0.0f);
        allowedSubsteps.Add(1.0f);
        while(true){
            newMeasure = false;
            while(!newMeasure){
                timingDisplay.text = (Song.elapsed + 1).ToString();
                Song.elapsed = (Time.realtimeSinceStartup - startTime) / 60.0f * Song.bpm - Song.timeSig * turns;
                if(Song.elapsed > Song.timeSig){
                    Song.elapsed = Mathf.Repeat(Song.elapsed, Song.timeSig);
                    newMeasure = true;
                }
                float substepScale = 1.2f - (Song.elapsed - (int)Song.elapsed)/5;
                metronomeDisplay.localScale = new Vector3(substepScale, substepScale, 1);
                yield return 0;
            }
            turns += 1;
            gameState = (gameState + 1) % 4;
            if(gameState == 0){
                turnDisplay.text = "Right Player Attack";
            } else if(gameState == 1){
                turnDisplay.text = "Left Player Defend";
            } else if(gameState == 2){
                turnDisplay.text = "Left Player Attack";
            } else if(gameState == 3){
                turnDisplay.text = "Right Player Defend";
            }
        }
    }

    public void SpawnNote(int player, int lane, float pressTime)
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
        int bestIndex = 0;
        float bestDist = 1.0f;
        float substep = pressTime - (int)pressTime;
        for(int i = 0; i < allowedSubsteps.Count; i++){
            float tempDist = Mathf.Abs(substep - allowedSubsteps[i]);
            if(tempDist < bestDist && (int)pressTime + allowedSubsteps[i] <= Song.timeSig - 1){
                bestDist = tempDist;
                bestIndex = i;
            }
        }
        pressTime = allowedSubsteps[bestIndex] + (int)pressTime;

        Debug.Log("Spawned note with time: " + pressTime);
        GameObject note = Instantiate(notePre, spawnLanes[lane].transform.position, Quaternion.identity);
        note.GetComponent<NoteMove>().target = targetLanes[lane].transform.position;
        note.GetComponent<NoteMove>().travelTime = substep - allowedSubsteps[bestIndex];
        noteList.Add((note, pressTime, lane));
    }

    //In theory we shouldn't need to differentiate between players for this one
    public void RegisterHit(int lane, float pressTime)
    {
        for(int i = 0; i < noteList.Count; i++){
            //split into two if statements to potentially do judgements
            if(noteList[i].lane == lane){
                float error = Mathf.Abs(noteList[i].timing - pressTime);
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
