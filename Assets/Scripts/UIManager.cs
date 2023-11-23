using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject raceUIParent;
    public TextMeshProUGUI lapCountText;
    public TextMeshProUGUI placementText;
    public GameObject endOfRaceParent;
    int currentLapCount = 1;

    void Start() {
        InitializeRaceUI();
    }

    private void OnEnable() {
        RaceManager.CompleteLapEvent += IncreaseLapCount;
        RaceManager.CompleteRaceEvent += ShowEndScreen;
    }

    private void OnDisable() {
        
    }

    void InitializeRaceUI() {
        raceUIParent.SetActive(true);
        currentLapCount = 1;
        SetLapCountText(currentLapCount);
        endOfRaceParent.SetActive(false);
    } 

    void IncreaseLapCount() {
        currentLapCount++;
        SetLapCountText(currentLapCount);
    }

    void ShowEndScreen() {
        endOfRaceParent.SetActive(true);
        raceUIParent.SetActive(false);
    }

    void SetLapCountText(int lapNum) {
        lapCountText.text = lapNum.ToString() + "/" + RaceManager.instance.lapCompletion.ToString();
    }

}
