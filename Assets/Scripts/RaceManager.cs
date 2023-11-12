using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceManager : MonoBehaviour
{
    static RaceManager instance;
    public int lapCompletion;
    private int currentLapCount = 0;
    public GameObject[] checkpoints;
    public static event Action CompleteLap;

    void Start() {
        instance = this;
    }

    
    void Update()
    {
        
    }
}
