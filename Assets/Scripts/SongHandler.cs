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
    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;
    public GameObject notePre;
    public List<GameObject> leftLanes;
    public List<GameObject> rightLanes;
    public PlayerController playerLeft;
    public PlayerController playerRight;
    private PlayerControls playerControls;
    public GameObject judgementPre;
    public GameObject measurePre;
    public Transform measureSpawnLeft;
    public Transform measureSpawnRight;
    public Animator anim;
    public Animator logoAnim;
    public Client client;

    [Header("Parameters")]
    public float bpm;
    public float timeSig;
    public float scrollSpeed;
    //Game state list:
        // 0: right player attack
        // 1: left player defend
        // 2: left player attack
        // 3: right player defend

    [Header("Data")]
    public List<AudioClip> sounds;
    public float startTime;
    public int turns;
    public List<float> allowedSubsteps;
    public List<float> allSteps;


    void Start()
    {
        Song.gameState = -1;
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
        
        HitPacket packet = new HitPacket();
        packet.timestamp = pressTime;
        packet.keyName = keyName;

        client.SendInputPacket(packet);

        int lane = -1;
        if(keyName == "w"){
            lane = 0;
            sfxPlayer.PlayOneShot(sounds[6]);
        } else if(keyName == "a"){
            lane = 1;
            sfxPlayer.PlayOneShot(sounds[5]);
        } else if(keyName == "d"){
            lane = 2;
            sfxPlayer.PlayOneShot(sounds[4]);
        } else if(keyName == "s"){
            lane = 3;
            sfxPlayer.PlayOneShot(sounds[3]);
        }
        if(lane >= 0){
            if((Song.gameState == 1 && Song.elapsed < Song.timeSig - 0.5f) || (Song.gameState == 0 && Song.elapsed > Song.timeSig - 0.5f)){
                RegisterHit(lane, pressTime);
            } else if((Song.gameState == 2 && Song.elapsed < Song.timeSig - 0.5f) || (Song.gameState == 1 && Song.elapsed > Song.timeSig - 0.5f)){
                if(playerLeft.SpendMana(1)){
                    SpawnNote(1, lane, pressTime);
                    playerLeft.attackTime = 2f;
                }
            }
            return;
        }
        if(keyName == "upArrow"){
            lane = 0;
            sfxPlayer.PlayOneShot(sounds[10]);
        } else if(keyName == "leftArrow"){
            lane = 1;
            sfxPlayer.PlayOneShot(sounds[9]);
        } else if(keyName == "rightArrow"){
            lane = 2;
            sfxPlayer.PlayOneShot(sounds[8]);
        } else if(keyName == "downArrow"){
            lane = 3;
            sfxPlayer.PlayOneShot(sounds[7]);
        }
        if(lane >= 0){
            if((Song.gameState == 3 && Song.elapsed < Song.timeSig - 0.5f) || (Song.gameState == 2 && Song.elapsed > Song.timeSig - 0.5f)){
                RegisterHit(lane, pressTime);
            } else if((Song.gameState == 0 && Song.elapsed < Song.timeSig - 0.5f) || (Song.gameState == 3 && Song.elapsed > Song.timeSig - 0.5f)){
                if(playerRight.SpendMana(1)){
                    SpawnNote(2, lane, pressTime);
                    playerRight.attackTime = 2f;
                }
            }
            return;
        }
    }

    public IEnumerator Metronome()
    {
        logoAnim.Play("LogoTop");
        yield return new WaitForSeconds(1f);
        anim.Play("SpawnLines");
        yield return new WaitForSeconds(1f);
        //First six lines are just debug, remove later
        int countdown = 16;
        bgmPlayer.PlayOneShot(sounds[0]);
        sfxPlayer.PlayOneShot(sounds[1]);
        while(countdown > 0){
            turnDisplay.text = countdown.ToString();
            yield return new WaitForSeconds(0.5f);
            sfxPlayer.PlayOneShot(sounds[1]);
            countdown -= 1;
        }
        turnDisplay.text = "Right Player Attack";
        
        startTime = Time.realtimeSinceStartup;
        bool newMeasure;
        bool metronomeLatch = true;
        Song.gameState = 0;
        allowedSubsteps.Add(0.0f);
        allowedSubsteps.Add(0.5f);
        while(true){
            newMeasure = false;
            
            allSteps = new List<float>();
            for(int i = 0; i < Song.timeSig; i++){
                foreach(float substep in allowedSubsteps){
                    allSteps.Add(i + substep);
                }
            }

            while(!newMeasure){
                // timingDisplay.text = (Song.elapsed + 1).ToString();
                Song.elapsed = (Time.realtimeSinceStartup - startTime) / 60.0f * Song.bpm - Song.timeSig * turns;
                if(Song.elapsed > Song.timeSig){
                    Song.elapsed = Mathf.Repeat(Song.elapsed, Song.timeSig);
                    newMeasure = true;
                }
                float substep = Song.elapsed - (int)Song.elapsed;
                float substepScale = 1.1f - substep/10;

                metronomeDisplay.localScale = new Vector3(substepScale, substepScale, 1);
                // if(substep < 0.5f && Song.elapsed < Song.timeSig){
                //     if(metronomeLatch){
                //         if(Song.gameState == 0){
                //             GameObject measureObj = Instantiate(measurePre, measureSpawnRight.position, Quaternion.identity);
                //             MeasureLine measureLine = measureObj.GetComponent<MeasureLine>();
                //             measureLine.target = measureSpawnLeft;
                //         } else if(Song.gameState == 2){
                //             GameObject measureObj = Instantiate(measurePre, measureSpawnLeft.position, Quaternion.identity);
                //             MeasureLine measureLine = measureObj.GetComponent<MeasureLine>();
                //             measureLine.target = measureSpawnRight;
                //         }
                //         // if(Song.elapsed < 0.5f){
                //         //     sfxPlayer.PlayOneShot(sounds[1]);
                //         // } else {
                //         //     sfxPlayer.PlayOneShot(sounds[2]);
                //         // }
                        
                //         metronomeLatch = false;
                //     }
                // } else {
                //     metronomeLatch = true;
                // }
                yield return 0;
            }
            turns += 1;
            Song.gameState = (Song.gameState + 1) % 4;
            //Mana is refreshed directly after the attack, but only visually shown during the next attack
            //This avoids being out of mana on your first hit
            if(Song.gameState == 0){
                turnDisplay.text = "Right Player Attack";
                StartCoroutine(playerRight.ManaBarUpdate());
            } else if(Song.gameState == 1){
                turnDisplay.text = "Left Player Defend";
                playerRight.mana = Mathf.Floor(playerRight.maxMana);
            } else if(Song.gameState == 2){
                turnDisplay.text = "Left Player Attack";
                StartCoroutine(playerLeft.ManaBarUpdate());
            } else if(Song.gameState == 3){
                turnDisplay.text = "Right Player Defend";
                playerLeft.mana = Mathf.Floor(playerLeft.maxMana);
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

        if(pressTime > 7 && pressTime < 7.5f){
            pressTime = 7;
        } else if(pressTime > 7.5f){
            pressTime -= 8;
        }
        
        for(int i = 0; i < allSteps.Count; i++){
            float tempDist = Mathf.Abs(pressTime - allSteps[i]);
            if(tempDist < bestDist){
                bestDist = tempDist;
                bestIndex = i;
            }
        }
        float offset = pressTime - allSteps[bestIndex];
        pressTime = allSteps[bestIndex];

        for(int i = 0; i < noteList.Count; i++){
            if(noteList[i].lane == lane && noteList[i].timing == pressTime){
                if(player == 1){
                    playerLeft.mana += 1;
                    playerLeft.manaBar.value += 1;
                } else {
                    playerRight.mana += 1;
                    playerRight.manaBar.value += 1;
                }
                return;
            }
        }
        

        Debug.Log("Spawned note with time: " + pressTime);
        GameObject note = Instantiate(notePre, spawnLanes[lane].transform.position, Quaternion.identity);
        note.GetComponent<NoteMove>().target = targetLanes[lane].transform;
        note.GetComponent<NoteMove>().travelTime = offset;
        noteList.Add((note, pressTime, lane));
    }

    //In theory we shouldn't need to differentiate between players for this one
    public void RegisterHit(int lane, float pressTime)
    {
        for(int i = 0; i < noteList.Count; i++){
            //split into two if statements to potentially do judgements
            if(noteList[i].lane == lane){
                float error = Mathf.Abs(noteList[i].timing - pressTime);
                error = (error < 1) ? error : -Song.timeSig + error;
                //really wish I knew how to do this in one check
                //lmk if you think of anything
                if(Mathf.Abs(error) < 1f){
                    Debug.Log("Note hit in lane " + lane + " with error " + error);
                    GameObject judgement = Instantiate(judgementPre, ((Song.gameState < 2) ? leftLanes[lane] : rightLanes[lane]).transform.position, Quaternion.identity);
                    if(Mathf.Abs(error) < 0.05f){
                        judgement.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Perfect";
                        if(Song.gameState < 2){
                            playerLeft.maxMana += 0.25f;
                            if(Mathf.Repeat(playerLeft.maxMana, 1) == 0){
                                playerLeft.mana += 1.0f;
                            }
                            
                            playerLeft.ManaBarScale();
                        } else {
                            playerRight.maxMana += 0.25f;
                            if(Mathf.Repeat(playerRight.maxMana, 1) == 0){
                                playerRight.mana += 1.0f;
                            }
                            playerRight.ManaBarScale();
                        }
                    } else if(Mathf.Abs(error) < 0.15f){
                        judgement.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Good";
                    } else {
                        judgement.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Bad";
                        judgement.transform.GetChild(0).GetComponent<TextMeshPro>().color = Color.red;
                        if(Song.gameState < 2){
                            playerLeft.TakeDamage(0.5f);
                        } else {
                            playerRight.TakeDamage(0.5f);
                        }
                    }
                    Destroy(noteList[i].obj);
                    noteList.RemoveAt(i);
                    break;
                }
            }
        }
        
    }
}
