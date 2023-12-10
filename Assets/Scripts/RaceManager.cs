using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public enum MapScenes { 
        SampleScene, 
        Island,
        SnowMountain,
        City,
        Tutorial,
        Playground,
    };

    public static RaceManager instance;
    public MapScenes currentScene;
    public MapScenes nextSceneToLoad;
    public int lapCompletion;
    private int currentLapCount = 0;
    public int totalCheckpoints = 0;
    public bool tutorialMode = false;

    public Checkpoint[] checkpoints;
    public BotController[] botDrivers;
    private int expectedCheckpointNumber;
    public static event Action BeginTutorialEvent;
    public static event Action BeginCountdownEvent;
    public static event Action StartRaceEvent;
    public static event Action CompleteLapEvent;
    public static event Action CompleteRaceEvent;
    public static event Action SwitchingToNewScene;

    public Transform playerSpawn;

    void Start() {
        instance = this;
        InitializeCheckpoints();
        if(!tutorialMode) BeginCountdown();
        else BeginTutorial();
    }

    public void PlayerPassedCheckpoint(int checkpointNumber) {
        if(checkpointNumber == expectedCheckpointNumber && !tutorialMode) {
            checkpoints[checkpointNumber].SetCrossed(true);
            expectedCheckpointNumber++;
            if (expectedCheckpointNumber >= checkpoints.Length) expectedCheckpointNumber = 0;
            totalCheckpoints++;
        }

        if(checkpointNumber == 0 && !tutorialMode) {
            if (CheckValidCompleteLap()) {
                CompleteLap();
                checkpoints[0].SetCrossed(true);
                expectedCheckpointNumber = 1;
            }
        }
    }

    public void MovePlayerToCheckpoint(CarController player){
        if (tutorialMode) {
            player.transform.position = playerSpawn.position;
            player.transform.rotation = playerSpawn.rotation;
            return;
        }

        int previousCheckpointIndex = expectedCheckpointNumber - 1;

        if(expectedCheckpointNumber == 0 && currentLapCount > 0) {
            previousCheckpointIndex = checkpoints.Length - 1;
        }

        if (previousCheckpointIndex == -1) return;
        else {
            player.transform.position = checkpoints[previousCheckpointIndex].transform.position;
            player.transform.rotation = checkpoints[previousCheckpointIndex].transform.rotation;
        }
    }

    public bool CheckValidCompleteLap() {
        foreach(Checkpoint checkpoint in checkpoints) {
            if (!checkpoint.GetCrossed()) return false;
        }

        return true;
    }

    public int FindPlayerPlacement(CarController player) {
        int playerPlacement = 1;
        foreach (BotController bot in botDrivers) {
            if(bot.GetLapCount() > currentLapCount) {
                playerPlacement++;
            } else {
                int botCheckpointCount = bot.GetCheckpointsPassed();
                int botCheckpointIndex = botCheckpointCount % checkpoints.Length;
                if (botCheckpointCount > totalCheckpoints) playerPlacement++;
                else if(botCheckpointCount == totalCheckpoints) {
                    float playerCheckpointDistance = Vector3.Distance(player.transform.position, checkpoints[expectedCheckpointNumber].transform.position);
                    float botCheckpointDistance = Vector3.Distance(bot.transform.position, checkpoints[botCheckpointIndex].transform.position);
                    if (playerCheckpointDistance > botCheckpointDistance) playerPlacement++;
                }
            } 
        }



        return playerPlacement;
    }

    public void LoadNextScene() {
        string sceneToLoad = "";

        switch (nextSceneToLoad) {
            case MapScenes.SampleScene:
                sceneToLoad = "SampleScene";
                break;
            case MapScenes.Island:
                sceneToLoad = "Island Map";
                break;
            case MapScenes.SnowMountain:
                sceneToLoad = "Snow Mountain Map";
                break;
            case MapScenes.Tutorial:
                sceneToLoad = "Tutorial Map";
                break;
            case MapScenes.Playground:
                sceneToLoad = "Playground Map";
                break;
            default:
                sceneToLoad = "SampleScene";
                break;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void CompleteLap() {
        CompleteLapEvent?.Invoke();
        currentLapCount++;
        if (currentLapCount >= lapCompletion) CompleteRace();
    }

    void CompleteRace() {
        CompleteRaceEvent?.Invoke();
        StartCoroutine(EndOfRaceRoutine());
    }

    void InitializeCheckpoints() {
        expectedCheckpointNumber = 0;
        for(int i=0; i<checkpoints.Length; i++) {
            checkpoints[i].SetCheckpointNum(i);
        }
    }

    void BeginTutorial()
    {
        StartCoroutine(TutorialRoutine());
    }

    void BeginRace() {
        StartRaceEvent?.Invoke();
    }

    void BeginCountdown() {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        BeginTutorialEvent?.Invoke();
        BeginRace();
    }

    IEnumerator CountdownRoutine() {
        yield return new WaitForSeconds(1.5f);

        BeginCountdownEvent?.Invoke();
        yield return new WaitForSeconds(1f);
        //Debug.Log("3");

        yield return new WaitForSeconds(1f);
        //Debug.Log("2");

        yield return new WaitForSeconds(1f);
        //Debug.Log("1");

        yield return new WaitForSeconds(1f);
        //Debug.Log("GO!");
        BeginRace();
    }

    IEnumerator EndOfRaceRoutine() {
        yield return new WaitForSeconds(6f);
        SwitchingToNewScene?.Invoke();
        yield return new WaitForSeconds(2f);

        if(!tutorialMode) LoadNextScene();
    }


}
