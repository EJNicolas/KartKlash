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
    private bool primaryStickPressed;

    [Header("Driving Parameters")]
    public float baseAcceleration = 0.5f;
    public float baseReverseAcceleration = 0.5f;
    private float currentSpeed = 0;
    public float turnSpeed = 0.3f;
    public float driftTurnSpeed;
    public bool canDrive = true;
    public bool reverseToggle = false;
    bool stickDownPrevFrame = false;

    void Start() {
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        VRPlayer.transform.position = this.transform.position;
        ReadControllerInputs();
    }

    void FixedUpdate()
    {
        DoAccelAndReverse();
        DoDrifting();
        ToggleReverse();
    }

    void DoAccelAndReverse()
    {

        if (gripPressed) {
            if (!reverseToggle) currentSpeed = baseAcceleration;
            else currentSpeed = -baseReverseAcceleration;
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
        if (primaryStickPressed && stickDownPrevFrame != primaryStickPressed) {
            reverseToggle = !reverseToggle;
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
        stickDownPrevFrame = primaryStickPressed;
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primaryStickPressed);
        
        //if(leftStickValue != Vector2.zero)
        //{
        //    Debug.Log($"Left Stick Value:, value {leftStickValue} ");
        //}

    }

}
