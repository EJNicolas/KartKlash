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
    GameObject player;
    public Transform spawnPosition;

    void Start() {
        if (currentType == PlayerType.VR) player = Instantiate(VRPlayer, spawnPosition.position, spawnPosition.rotation);
        else player = Instantiate(FPSPlayer, spawnPosition.position, Quaternion.identity);
    }

    void Update()
    {
        
    }

    void TransitionToNextScene()
    {
        if (currentType == PlayerType.VR) player.GetComponentInChildren<SceneTransitionBehaviour>().TransitionOut();
    }
}
