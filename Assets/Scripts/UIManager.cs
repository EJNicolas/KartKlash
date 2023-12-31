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
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI[] endOfRaceTimers;
    int currentLapCount = 1;

    [Header("Speed")]
    public Image speedBar;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI shootablesText;

    [Header("Health")]
    public TextMeshProUGUI healthText;

    [Header("Hit HUD")]
    [SerializeField] CanvasGroup leftHUD;
    [SerializeField] CanvasGroup rightHUD;
    [SerializeField] CanvasGroup frontHUD;
    [SerializeField] CanvasGroup backHUD;

    [Header("Wipeout Post-Processing")]
    [SerializeField] UnityEngine.Rendering.Volume postVol;

    [Header("Tutorial")]
    public bool tutorialMode;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countdown;
    public AudioClip lapCounter;
    public AudioClip lapFinish;

    string placement;

    string[] lapTimes;
    float totalTime;
    float lapTime;
    bool startTimer = false;

    void Start() {
        if (RaceManager.instance) tutorialMode = RaceManager.instance.tutorialMode;
        InitializeRaceUI();
        lapTimes = new string[RaceManager.instance.lapCompletion];
        foreach(TextMeshProUGUI textMesh in endOfRaceTimers) {
            textMesh.text = "";
        }
    }

    private void Update() {
        if(!RaceManager.instance.tutorialMode) UpdateTimer();
    }

    private void OnEnable() {
        RaceManager.BeginTutorialEvent += SetTutorialMode;
        RaceManager.BeginCountdownEvent += StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent += IncreaseLapCount;
        RaceManager.CompleteLapEvent += LogLapTime;
        RaceManager.CompleteRaceEvent += ShowEndScreen;
        RaceManager.SwitchingToNewScene += RemoveUI;
    }

    private void OnDisable() {
        RaceManager.BeginTutorialEvent -= SetTutorialMode;
        RaceManager.BeginCountdownEvent -= StartCountdownUIRoutine;
        RaceManager.CompleteLapEvent -= IncreaseLapCount;
        RaceManager.CompleteLapEvent -= LogLapTime;
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

    void UpdateTimer() {
        if (startTimer) {
            totalTime += Time.deltaTime;
            lapTime += Time.deltaTime;
        }

        timerText.text = DisplayTime(lapTime);
    }

    void LogLapTime() {
        lapTimes[RaceManager.instance.currentLapCount] = timerText.text;
        lapTime = 0;
    }

    public void UpdateSpeedUI(float speed)
    {
        speedText.text = "" + (int)(speed * 4);
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

    public void SetWipeoutPostProcessing(bool wipedOut)
    {
        if (wipedOut) LeanTween.value(0f, 1f, 0.5f).setOnUpdate((float val) => { postVol.weight = val; });
        else LeanTween.value(1f, 0f, 0.5f).setOnUpdate((float val) => { postVol.weight = val; });
    }

    public void ResetShotHUD()
    {
        leftHUD.alpha = 0f;
        rightHUD.alpha = 0f;
        frontHUD.alpha = 0f;
        backHUD.alpha = 0f;
    }

    void RemoveUI() {
        endOfRaceParent.SetActive(false);
        raceUIParent.SetActive(false);
    }

    void StartCountdownUIRoutine() {
        countdownParent.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    private string DisplayTime(float timeToDisplay) {
        // get the total full seconds.
        var t0 = (int) timeToDisplay;

        // get the number of minutes.
        var m = t0/60;

        // get the remaining seconds.
        var s = (t0 - m*60);

        // get the 2 most significant values of the milliseconds.
        var ms = (int)( (timeToDisplay - t0)*100);

        return $"{m:00} : {s:00} : {ms:00}";
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
        startTimer = true;

        yield return new WaitForSeconds(0.5f);
        countdownParent.SetActive(false);
    }

    void ShowEndScreen() {
        placement = placementText.text;
        startTimer = false;
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

        yield return new WaitForSeconds(0.33f);
        endOfRaceTimers[0].text = "Lap 1 " + lapTimes[0];

        yield return new WaitForSeconds(0.33f);
        endOfRaceTimers[1].text = "Lap 2 " +lapTimes[1];

        yield return new WaitForSeconds(0.33f);
        endOfRaceTimers[2].text = "Lap 3 " +lapTimes[2];

        yield return new WaitForSeconds(0.33f);
        endOfRaceTimers[3].text = "Total " +DisplayTime(totalTime);
    }

}
