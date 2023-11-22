using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CarController : MonoBehaviour
{
    [Header("Player Components")]
    public GameObject VRPlayer;
    public GameObject carParent;
    public XRController leftController;
    public XRController rightController;
    public Rigidbody carRb;
    public BoxCollider carBoxCollider;

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

    void Start() {
        transform.parent = null;
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
    void Update()
    {
        VRPlayer.transform.position = this.transform.position;
        ReadControllerInputs();
        ToggleReverse();
        if(primaryButtonDown || secondaryButtonDown) ReallignCameraToCar();
    }

    void FixedUpdate()
    {
        if (canDrive) {
            DoAccelAndReverse();
            DoDrifting();
        }
            
    }

    void DoAccelAndReverse()
    {

        if (gripPressed) {
            if (triggerPressed) currentSpeed = driftAcceleration;
            else if (reverseToggle) currentSpeed = -baseReverseAcceleration;
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
            transform.Rotate(0, newRotation, 0);
            VRPlayer.transform.Rotate(0, newRotation, 0);
        }
    }

    void DoDrifting()
    {   
        if(triggerPressed && carRb.velocity.magnitude > 1)
        {
            float newRotation = primaryStickValue.x * driftTurnSpeed * Time.deltaTime;
            transform.Rotate(0, newRotation, 0);
            VRPlayer.transform.Rotate(0, newRotation, 0);
        }
        
    }

    void ToggleReverse() {
        if (primaryStickPressed) {
            reverseToggle = !reverseToggle;
        }
    }
    
    void ReallignCameraToCar() {
        if(primaryButtonDown || secondaryButtonDown) {
            VRPlayer.transform.rotation = transform.rotation;
        }
    }

    void ReadControllerInputs()
    {
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

        //if(leftStickValue != Vector2.zero)
        //{
        //    Debug.Log($"Left Stick Value:, value {leftStickValue} ");
        //}

    }

    void StartRace() {
        canDrive = true;
    }

    void EndRace() {
        canDrive = false;
    }

}
