using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Bot Damage Preset", menuName = "Scriptables/KartKlash Bot Damage", order = 1)]
public class BotDamagePreset : ScriptableObject
{
    public float shotInterval;

    public float outlineHoverWidth;
    public float outlineShootWidth;

    public Color shotColor;
    public Color hoverColor;
}
