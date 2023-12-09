using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Bot Driving Preset", menuName = "Scriptables/KartKlash Bot Driving", order = 1)]
public class BotDrivingPreset : ScriptableObject
{
    [Header("Speed")]
    public float minDefaultSpeed;
    public float maxDefaultSpeed;

    [Header("Angular Speed")]
    public float minDefaultAngular;
    public float maxDefaultAngular;

    [Header("Acceleration")]
    public float minDefaultAccel;
    public float maxDefaultAccel;

    [Header("Stopping Distance")]
    public float minStoppingDistance;
    public float maxStoppingDistance;

    [Header("Rubberbanding Settings")]
    public int rubberbandThreshold;

    public float speedRubberbandAmount;
    public float angularRubberbandFactor;
    public float accelRubberbandAmount;
}
