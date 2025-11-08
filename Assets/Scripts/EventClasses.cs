using UnityEngine;
using System.Collections.Generic;

public abstract class SongEvent
{
    public string name { get; protected set; }
    public int measure { get; protected set; }

    public virtual void Activate()
    {
        
    }
}

// Define derived classes
public class AddSteps : SongEvent
{
    public float[] steps;
    public AddSteps(int measureIn, float[] stepsIn)
    {
        name = "AddSteps";
        measure = measureIn;
        steps = stepsIn;
    }

    public override void Activate()
    {
        Debug.Log("Adding substeps");
        SongHandler songHandler = Song.songHandler;
        foreach(float substep in steps){
            songHandler.allowedSubsteps.Add(substep);
        }
        List<float> allSteps = new List<float>();
        for(int i = 0; i < Song.timeSig - 1; i++){
            foreach(float substep in songHandler.allowedSubsteps){
                allSteps.Add(i + substep);
            }
            allSteps.Add(Song.timeSig - 1);
        }
        songHandler.allSteps = allSteps;
    }
}

public class Choreograph : SongEvent
{
    public float[] timestamps;
    public int[] lanes;

    public Choreograph(float[] timestampsIn, int[] lanesIn){
        timestamps = timestampsIn;
        lanes = lanesIn;
    }

    public override void Activate()
    {
        Debug.Log("Spawning choreographed notes");
    }
}