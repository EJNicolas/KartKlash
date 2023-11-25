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
    public GameObject countdownParent;
    public TextMeshProUGUI countdownText;
    int currentLapCount = 1;

    void Start() {
        InitializeRaceUI();
    }

    private void OnEnable() {
        RaceManager.BeginCountdownEvent += StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent += IncreaseLapCount;
        RaceManager.CompleteRaceEvent += ShowEndScreen;
    }

    private void OnDisable() {
        RaceManager.BeginCountdownEvent -= StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent -= IncreaseLapCount;
        RaceManager.CompleteRaceEvent -= ShowEndScreen;
    }

    void InitializeRaceUI() {
        raceUIParent.SetActive(true);
        currentLapCount = 1;
        SetLapCountText(currentLapCount);
        endOfRaceParent.SetActive(false);
        //countdownParent.SetActive(false);
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

    void StartCountdownUIRoutine() {
        countdownParent.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine() {
        yield return new WaitForSeconds(0.5f);
        countdownText.text = "3";

        yield return new WaitForSeconds(1f);
        countdownText.text = "2";

        yield return new WaitForSeconds(1f);
        countdownText.text = "1";

        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!";

        yield return new WaitForSeconds(0.5f);
        countdownParent.SetActive(false);
    }

}
