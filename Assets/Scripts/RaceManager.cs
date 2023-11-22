using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public int lapCompletion;
    private int currentLapCount = 0;
    public int checkpointsPassed = 0;

    public Checkpoint[] checkpoints;
    private int expectedCheckpointNumber;
    public static event Action StartRaceEvent;
    public static event Action CompleteLapEvent;
    public static event Action CompleteRaceEvent;

    void Start() {
        instance = this;
        InitializeCheckpoints();
    }

    public void PlayerPassedCheckpoint(int checkpointNumber) {
        if(checkpointNumber == expectedCheckpointNumber) {
            Debug.Log("player passed through checkpoint " + checkpointNumber);
            checkpoints[checkpointNumber].SetCrossed(true);
            expectedCheckpointNumber++;
            checkpointsPassed++;
        }

        if(checkpointNumber == 0) {
            if (CheckValidCompleteLap()) {
                CompleteLap();
                checkpoints[0].SetCrossed(true);
                expectedCheckpointNumber = 1;
            }
        }
    }

    public bool CheckValidCompleteLap() {
        foreach(Checkpoint checkpoint in checkpoints) {
            if (!checkpoint.GetCrossed()) return false;
        }

        return true;
    }

    public void CompleteLap() {
        CompleteLapEvent?.Invoke();
        currentLapCount++;
        Debug.Log("LAP COMPLETED");
        if (currentLapCount >= lapCompletion) CompleteRace();
    }

    void CompleteRace() {
        CompleteRaceEvent?.Invoke();
        Debug.Log("YOU FINISHED THE RACE");
    }

    void InitializeCheckpoints() {
        expectedCheckpointNumber = 0;
        for(int i=0; i<checkpoints.Length; i++) {
            checkpoints[i].SetCheckpointNum(i);
        }
    }
}
