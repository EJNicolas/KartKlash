using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public int lapCompletion;
    private int currentLapCount = 0;
    public Checkpoint[] checkpoints;
    public static event Action CompleteLapEvent;
    public static event Action CompleteRaceEvent;

    void Start() {
        instance = this;
    }

    private void OnEnable() {
        CompleteLapEvent += CompleteLap;
    }

    private void OnDisable() {
        CompleteLapEvent -= CompleteLap;
    }

    void Update() {
        
    }

    public bool CheckValidCompleteLap() {
        foreach(Checkpoint checkpoint in checkpoints) {
            if (!checkpoint.GetCrossed()) return false;
        }

        CompleteLap();
        return true;
    }

    public void CompleteLap() {
        CompleteLapEvent?.Invoke();
        currentLapCount++;
        if (currentLapCount >= lapCompletion) CompleteRaceEvent?.Invoke();
    }
}
