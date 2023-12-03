using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Bot Shooting Preset", menuName = "Scriptables/KartKlash Bot Shooting", order = 1)]
public class BotShootPreset : ScriptableObject
{
    public float minFireRate;
    public float maxFireRate;

    public float minDamage;
    public float maxDamage;

    public float minHitChance;
    public float maxHitChance;

    public float minBotHitChanceMult;
    public float maxBotHitChanceMult;
}
