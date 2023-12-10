using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CarController : MonoBehaviour
{
    [Header("Player Components")]
    public GameObject VRPlayer;
    public GameObject carParent;
    public XRController leftController;
    public XRController rightController;
    public VRHand drivingHand;
    public Rigidbody carRb;
    public BoxCollider carBoxCollider;
    public UIManager UIManagerScript;

    [Header("Controls")]
    private bool triggerPressed;
    private float triggerValue;
    private bool gripPressed;
    private float gripValue;
    private Vector2 primaryStickValue;
    private bool primaryStickDown;
    private bool primaryStickPressed;
    private bool primaryButtonDown;
    private bool secondaryButtonDown;
    private bool menuButtonDown;

    [Header("Driving Parameters")]
    public float baseAcceleration = 0.5f;
    public float baseReverseAcceleration = 0.5f;
    public float driftAcceleration;
    private float currentSpeed = 0;
    public float turnSpeed = 0.3f;
    public float driftTurnSpeed;
    public bool canDrive = true;
    public bool reverseToggle = false;
    bool stickDownPrevFrame = false;

    public float resetTimer = 1.5f;
    float resetTimerCounter;
    bool forcedReset = false;
    public float absoluteResetAngleTreshold = 80;
    public float resetAngleTreshold;
    public float resetSpeedTreshold;

    public float cpuBumpForce = 50;

    [Header("Engine Sounds")]
    public AudioSource engineAudioSource;
    public AudioSource driftAudioSource;

    public float minimumPitch = 0.05f;
    public float maximumPitch = 0.1f;
    float engineSpeed;

    int racePlacement;

    [Header("Shootables")]
    public int targetsShot = 0;
    public int targetLimit;
    public float targetAccelBonus;


    void Start() {
        transform.parent = null;
        engineAudioSource.pitch = minimumPitch;

        engineAudioSource.volume = 0.3f;
        driftAudioSource.volume = 0f;
        resetTimerCounter = resetTimer;
    }

    private void OnEnable() {
        RaceManager.StartRaceEvent += StartRace;
        RaceManager.CompleteRaceEvent += EndRace;
    }

    private void OnDisable() {
        RaceManager.StartRaceEvent -= StartRace;
        RaceManager.CompleteRaceEvent -= EndRace;
    }

    // Update is called once per frame
    void Update() {
        VRPlayer.transform.position = this.transform.position;
        ReadControllerInputs();
        ToggleReverse();
        if(primaryButtonDown || secondaryButtonDown) ReallignCameraToCar();
        ChangeHandModels();
        EngineSoundPitch();
        UpdatePlayerPlacement();
        SetAboveGround();
        CheckAutomaticReset();
    } 

    void FixedUpdate() {
        if (canDrive && !menuButtonDown && !forcedReset) {
            DoAccelAndReverse();
            DoDrifting();
            UIManagerScript.UpdateSpeedUI(carRb.velocity.magnitude);
        }

        if (canDrive) {
            CheckTeleportToLastCheckpoint();
        }
            
    }

    void DoAccelAndReverse()
    {

        if (gripPressed) {
            if (reverseToggle) currentSpeed = -baseReverseAcceleration; 
            else if (triggerPressed) currentSpeed = driftAcceleration;
            else currentSpeed = baseAcceleration;
        }
        else currentSpeed = 0;

        carRb.AddForce(transform.forward * gripValue * currentSpeed);

        if (!triggerPressed) DoTurning();

    }

    void DoTurning()
    {
        if(carRb.velocity.magnitude > 1)
        {
            float newRotation = primaryStickValue.x * turnSpeed * Time.deltaTime;
            if (reverseToggle) newRotation *= -1;
            transform.Rotate(0, newRotation, 0);
            VRPlayer.transform.Rotate(0, newRotation, 0);
        }
    }

    void DoDrifting()
    {   
        if(triggerPressed && carRb.velocity.magnitude > 1) {
            driftAudioSource.volume = 1f;
            float newRotation = primaryStickValue.x * driftTurnSpeed * Time.deltaTime;
            transform.Rotate(0, newRotation, 0);
            VRPlayer.transform.Rotate(0, newRotation, 0);
        }
        else {
            driftAudioSource.volume = 0f;
        }
        
    }


    int layerMask = (1 << 8) | (1 << 11);
    float height = 0.5f;
    float damping = 2;
    void SetAboveGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 50, layerMask))
        {
            Vector3 point = hit.point;
            point.y += height;
            transform.position = Vector3.Lerp(transform.position, point, Time.deltaTime * damping);
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = slopeRotation;
        }
    }

    void ToggleReverse() {
        if (primaryStickPressed) {
            reverseToggle = !reverseToggle;
            if (reverseToggle) drivingHand.SetDrivingText("Reverse");
            else drivingHand.SetDrivingText("Drive");
        }
    }
    
    void ReallignCameraToCar() {
        if(primaryButtonDown || secondaryButtonDown) {
            VRPlayer.transform.rotation = transform.rotation;
        }
    }

    void ChangeHandModels() {
        if (triggerPressed) drivingHand.SetClosedHand();
        else if (gripPressed) drivingHand.SetPointingHand();
        else drivingHand.SetOpenHand();
    }

    void CheckTeleportToLastCheckpoint() {
        if (menuButtonDown || forcedReset) {
            UIManagerScript.SetRespawnTextActive(true);
            resetTimerCounter -= Time.deltaTime;
            if (resetTimerCounter < 0) {
                UIManagerScript.SetRespawnTextActive(false);
                UIManagerScript.ResetShotHUD();
                //UIManagerScript.SetWipeoutPostProcessing(false);

                RaceManager.instance.MovePlayerToCheckpoint(this);
                VRPlayer.transform.rotation = transform.rotation;
                carRb.angularVelocity = Vector3.zero;
                resetTimerCounter = resetTimer;
                forcedReset = false;
                return;
            }
            else if (resetTimerCounter < resetTimer / 3) {
                UIManagerScript.SetRespawnText("Respawning in: 1");
            }
            else if (resetTimerCounter < resetTimer / 3 * 2) {
                UIManagerScript.SetRespawnText("Respawning in: 2");
            }
            else if (resetTimerCounter <= resetTimer) {
                UIManagerScript.SetRespawnText("Respawning in: 3");
                //UIManagerScript.SetWipeoutPostProcessing(true);
            }
        }
        else {
            UIManagerScript.SetRespawnTextActive(false);
            resetTimerCounter = resetTimer;
        }
        
    }
    void CheckAutomaticReset() {
        float farEnd = (transform.eulerAngles.y + resetAngleTreshold) % 360;
        float backEnd = (transform.eulerAngles.y - resetAngleTreshold) % 360;
        if (backEnd < 0) backEnd = 360 - backEnd;
        //Debug.Log("backEnd: " + backEnd + "        farEnd: " + farEnd);
        //Debug.Log("player: " + VRPlayer.transform.eulerAngles.y);
        if(farEnd > backEnd) {
            if (!(VRPlayer.transform.eulerAngles.y > backEnd && VRPlayer.transform.eulerAngles.y < farEnd)) forcedReset = true;
        }
        else {
            if (!(VRPlayer.transform.eulerAngles.y > backEnd || VRPlayer.transform.eulerAngles.y < farEnd)) forcedReset = true;
        }

    }

    void UpdatePlayerPlacement() {
        int newPlacement = RaceManager.instance.FindPlayerPlacement(this);
        if(racePlacement != newPlacement) {
            racePlacement = newPlacement;
            UIManagerScript.SetPlacementText(racePlacement);
        }
    }

    void EngineSoundPitch() {
        engineSpeed = Mathf.Log10(carRb.velocity.magnitude);

        if(engineSpeed < minimumPitch){
            engineAudioSource.pitch = minimumPitch;
        }else if(engineSpeed > maximumPitch){
            engineAudioSource.pitch = maximumPitch;
        }else{
        engineAudioSource.pitch = engineSpeed;
        }
    }

    //private void OnCollisionEnter(Collision other) {
    //    if (other.gameObject.CompareTag("CPU")) {
    //        //other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * cpuBumpForce);
    //        Debug.Log("Points colliding: " + other.contacts.Length);
    //    }
    //}

    void ReadControllerInputs() {
        //get & print the trigger value
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        if (triggerValue != 0) triggerPressed = true;
        else triggerPressed = false;
        //if (triggerValue >= 0.1f) Debug.Log($"Trigger Pressed, value {triggerValue}");

        //get & print the grip value
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out gripValue);
        if (gripValue != 0) gripPressed = true;
        else gripPressed = false;
        //if (gripValue >= 0.1f) Debug.Log($"Grip Pressed, value {gripValue}");

        leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out primaryStickValue);
        stickDownPrevFrame = primaryStickDown;
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primaryStickDown);
        //primaryStickPressed true only when button is pressed down the first time
        if(primaryStickDown && !stickDownPrevFrame) {
            primaryStickPressed = true;
        } else if(primaryStickDown && stickDownPrevFrame) {
            primaryStickPressed = false;
        }

        leftController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonDown);
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonDown);

        leftController.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonDown);

        //if(leftStickValue != Vector2.zero)
        //{
        //    Debug.Log($"Left Stick Value:, value {leftStickValue} ");
        //}

    }

    public void ShootableShot()
    {
        if (targetsShot < targetLimit)
        {
            targetsShot++;
            baseAcceleration += targetAccelBonus;
            UIManagerScript.SetShootableCountText(targetsShot, targetLimit);
        }
    }

    void StartRace() {
        canDrive = true;
    }

    void EndRace() {
        canDrive = false;
    }

}
