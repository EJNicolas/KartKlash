using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Race Attributes")]
    public GameObject raceUIParent;
    public TextMeshProUGUI lapCountText;
    public TextMeshProUGUI placementText;
    public GameObject endOfRaceParent;
    public GameObject countdownParent;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI respawnText;
    public TextMeshProUGUI finishText;
    public TextMeshProUGUI endPlacementMessage;
    public TextMeshProUGUI endPlacementText;
    int currentLapCount = 1;

    [Header("Speed")]
    public Image speedBar;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI shootablesText;

    [Header("Health")]
    public TextMeshProUGUI healthText;

    [Header("Tutorial")]
    public bool tutorialMode;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countdown;
    public AudioClip lapCounter;
    public AudioClip lapFinish;

    string placement;
    void Start() {
        if (RaceManager.instance) tutorialMode = RaceManager.instance.tutorialMode;
        InitializeRaceUI();
    }

    private void OnEnable() {
        RaceManager.BeginTutorialEvent += SetTutorialMode;
        RaceManager.BeginCountdownEvent += StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent += IncreaseLapCount;
        RaceManager.CompleteRaceEvent += ShowEndScreen;
        RaceManager.SwitchingToNewScene += RemoveUI;
    }

    private void OnDisable() {
        RaceManager.BeginTutorialEvent -= SetTutorialMode;
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
        if(!tutorialMode) countdownParent.SetActive(true);
    } 

    void IncreaseLapCount() {
        currentLapCount++;
        SetLapCountText(currentLapCount);
        audioSource.PlayOneShot(lapCounter,  1);
    }

    public void UpdateSpeedUI(float speed)
    {
        speedText.text = "" + (int) speed * 4;
        speedBar.fillAmount = speed / 50f;
    }

    public void SetHealthText(float health)
    {
        if (tutorialMode) healthText.text = (int) health + "";
    }

    void SetLapCountText(int lapNum) {
        if (RaceManager.instance) lapCountText.text = "Lap " + lapNum.ToString() + " | " + RaceManager.instance.lapCompletion.ToString();
    }

    public void SetShootableCountText(int targets, int maxTargets)
    {
        shootablesText.text = targets.ToString() + " | " + maxTargets.ToString();
    }

    public void SetRespawnText(string s) {
        respawnText.text = s;
    }

    public void SetRespawnTextActive(bool b) {
        respawnText.gameObject.SetActive(b);
    }

    public void SetPlacementText(int num) {
        string placementString = "";
        switch (num) {
            case 1: 
                placementString = "1st";
                break;
            case 2: 
                placementString = "2nd";
                break;
            case 3: 
                placementString = "3rd";
                break;
            default:
                placementString = num.ToString() + "th";
                break;
        }
        placementText.text = placementString;
    }

    void SetTutorialMode()
    {
        tutorialMode = true;
        countdownParent.SetActive(false);
        placementText.text = "";
        lapCountText.text = "";
        healthText.gameObject.SetActive(true);
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

    void ShowEndScreen() {
        placement = placementText.text;
        StartCoroutine(EndOfRaceRoutine());
    }

    IEnumerator EndOfRaceRoutine() {
        endOfRaceParent.SetActive(true);
        raceUIParent.SetActive(false);
        endPlacementMessage.text = "";
        endPlacementText.text = "";
        yield return new WaitForSeconds(2f);
        finishText.text = "";
        endPlacementMessage.text = "You placed";
        yield return new WaitForSeconds(0.33f);
        endPlacementMessage.text = "You placed.";
        yield return new WaitForSeconds(0.33f);
        endPlacementMessage.text = "You placed..";
        yield return new WaitForSeconds(0.33f);
        endPlacementMessage.text = "You placed...";
        yield return new WaitForSeconds(0.33f);
        endPlacementText.text = placement + "!";
    }

}
