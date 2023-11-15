using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootingTest : MonoBehaviour
{
    public XRController rightController;
    public RaycastOutlineVisual rov;
    private bool triggerPressed;
    private float triggerValue;

    public float shootInterval;
    public bool shot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadControllerInputs();
    }

    private void FixedUpdate()
    {
        TestShoot();
    }

    void ReadControllerInputs()
    {
        rightController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        if (triggerValue != 0) triggerPressed = true;
        else triggerPressed = false;
        //if (triggerValue >= 0.1f) Debug.Log($"Trigger Pressed, value {triggerValue}");
    }

    void TestShoot()
    {
        if(triggerPressed) rov.ShootOutline();
    }
}
