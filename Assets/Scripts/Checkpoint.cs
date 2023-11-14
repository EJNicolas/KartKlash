using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    int checkpointNum;
    bool crossed;
    public bool finishLine;

    void Start() {
        crossed = false;
    }

    private void OnEnable() {
        RaceManager.CompleteLapEvent += ResetCheckpoint;
    }

    private void OnDisable() {
        RaceManager.CompleteLapEvent -= ResetCheckpoint;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            RaceManager.instance.PlayerPassedCheckpoint(checkpointNum);
        }
    }

    public bool GetCrossed() {
        return crossed;
    }

    public void SetCrossed(bool b) {
        crossed = b;
    }

    public int GetCheckpointNum() {
        return checkpointNum;
    }

    public void SetCheckpointNum(int i) {
        checkpointNum = i;
    }

    public void ResetCheckpoint() {
        crossed = false;
    }



}
