using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    bool crossed;
    public bool finishLine;

    void Start() {
        crossed = false;
    }

    private void OnEnable() {
        RaceManager.CompleteLapEvent += ResetCrossed;
    }

    private void OnDisable() {
        RaceManager.CompleteLapEvent -= ResetCrossed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            crossed = true;

            if (finishLine) {
                RaceManager.instance.CheckValidCompleteLap();
            }
        }
    }

    public bool GetCrossed() {
        return crossed;
    }

    public void SetCrossed(bool b) {
        crossed = b;
    }

    void ResetCrossed() {
        crossed = false;
    }

}
