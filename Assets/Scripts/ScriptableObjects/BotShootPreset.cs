using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Bot Shooting Preset", menuName = "Scriptables/KartKlash Bot Shooting", order = 1)]
public class BotShootPreset : ScriptableObject
{
    [Header("Fire Rate")]
    public float minFireRate;
    public float maxFireRate;

    [Header("Damage")]
    public float minDamage;
    public float maxDamage;

    [Header("Hit Chance")]
    public float minHitChance;
    public float maxHitChance;

    [Header("Bot Hit Chance")]
    public float minBotHitChanceMult;
    public float maxBotHitChanceMult;

    [Header("Shootable Hit Chance")]
    public float minTargetHitChanceMult;
    public float maxTargetHitChanceMult;
}
