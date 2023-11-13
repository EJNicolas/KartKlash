using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerType { VR, FPS };
    public PlayerType currentType;

    [Header("Player References")]
    public GameObject VRPlayer;
    public GameObject FPSPlayer;
    public Transform spawnPosition;

    void Start() {
        if (currentType == PlayerType.VR) Instantiate(VRPlayer, spawnPosition.position, Quaternion.identity);
        else Instantiate(FPSPlayer, spawnPosition.position, Quaternion.identity);
    }

    void Update()
    {
        
    }
}
