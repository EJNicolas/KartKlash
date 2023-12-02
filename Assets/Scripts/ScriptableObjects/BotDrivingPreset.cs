using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Bot Driving Preset", menuName = "Scriptables/KartKlash Bot Driving", order = 1)]
public class BotDrivingPreset : ScriptableObject
{
    public float minDefaultSpeed;
    public float maxDefaultSpeed;

    public float minDefaultAngular;
    public float maxDefaultAngular;

    public float minDefaultAccel;
    public float maxDefaultAccel;

    public float minStoppingDistance;
    public float maxStoppingDistance;

    public float speedRubberbandAmount;
    public float angularRubberbandFactor;
    public float accelRubberbandAmount;
}
