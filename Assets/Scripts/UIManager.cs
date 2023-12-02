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
    public TextMeshProUGUI respawnText;
    int currentLapCount = 1;
    public AudioSource audioSource;
    public AudioClip countdown;
    public AudioClip lapCounter;
    public AudioClip lapFinish;

    void Start() {
        InitializeRaceUI();
    }

    private void OnEnable() {
        RaceManager.BeginCountdownEvent += StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent += IncreaseLapCount;
        RaceManager.CompleteRaceEvent += ShowEndScreen;
        RaceManager.SwitchingToNewScene += RemoveUI;
    }

    private void OnDisable() {
        RaceManager.BeginCountdownEvent -= StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent -= IncreaseLapCount;
        RaceManager.CompleteRaceEvent -= ShowEndScreen;
        RaceManager.SwitchingToNewScene -= RemoveUI;
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
        audioSource.PlayOneShot(lapCounter,  1);
    }

    void ShowEndScreen() {
        endOfRaceParent.SetActive(true);
        raceUIParent.SetActive(false);
    }

    void SetLapCountText(int lapNum) {
        lapCountText.text = lapNum.ToString() + "/" + RaceManager.instance.lapCompletion.ToString();
    }

    public void SetRespawnText(string s) {
        respawnText.text = s;
    }

    public void SetRespawnTextActive(bool b) {
        respawnText.gameObject.SetActive(b);
    }

    void RemoveUI() {
        endOfRaceParent.SetActive(false);
        raceUIParent.SetActive(false);
    }

    void StartCountdownUIRoutine() {
        countdownParent.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine() {
        
        yield return new WaitForSeconds(1f);
        countdownText.text = "3";
        audioSource.PlayOneShot(countdown,  1);

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
