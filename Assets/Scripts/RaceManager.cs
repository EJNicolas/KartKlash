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
    public static event Action BeginCountdownEvent;
    public static event Action StartRaceEvent;
    public static event Action CompleteLapEvent;
    public static event Action CompleteRaceEvent;

    void Start() {
        instance = this;
        InitializeCheckpoints();
        BeginCountdown();
        
    }

    public void PlayerPassedCheckpoint(int checkpointNumber) {
        if(checkpointNumber == expectedCheckpointNumber) {
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

    public void MovePlayerToCheckpoint(CarController player){
        int previousCheckpointIndex = expectedCheckpointNumber - 1;

        if(expectedCheckpointNumber == 0 && currentLapCount > 0) {
            previousCheckpointIndex = checkpoints.Length - 1;
        }

        if (previousCheckpointIndex == -1) return;
        else {
            player.transform.position = checkpoints[previousCheckpointIndex].transform.position;
            player.transform.rotation = checkpoints[previousCheckpointIndex].transform.rotation;
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
        if (currentLapCount >= lapCompletion) CompleteRace();
    }

    void CompleteRace() {
        CompleteRaceEvent?.Invoke();
    }

    void InitializeCheckpoints() {
        expectedCheckpointNumber = 0;
        for(int i=0; i<checkpoints.Length; i++) {
            checkpoints[i].SetCheckpointNum(i);
        }
    }

    void BeginRace() {
        StartRaceEvent?.Invoke();
    }

    void BeginCountdown() {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine() {
        yield return new WaitForSeconds(1.5f);

        BeginCountdownEvent?.Invoke();
        yield return new WaitForSeconds(1f);
        //Debug.Log("3");

        yield return new WaitForSeconds(1f);
        //Debug.Log("2");

        yield return new WaitForSeconds(1f);
        //Debug.Log("1");

        yield return new WaitForSeconds(1f);
        //Debug.Log("GO!");
        BeginRace();
    }
}
