using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresence : MonoBehaviour
{
    // Start is called before the first frame update
    public XRController leftController;
    public XRController rightController;

    void Start()
    {
        //List<InputDevice> devices = new List<InputDevice>();
        //InputDeviceCharacteristics rightControllerCharacteristic = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //InputDeviceCharacteristics leftControllerCharacteristic = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        //InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristic, devices);
        //InputDevices.GetDevices(devices);

        //foreach (var item in devices)
        //{
        //    Debug.Log(item.name + item.characteristics);
        //}

    }

    // Update is called once per frame
    void Update()
    {
        GetControllerInputs();

        

    }

    void GetControllerInputs()
    {
        //get & print the trigger value
        float triggerValue;
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        //if (triggerValue >= 0.1f) Debug.Log($"Trigger Pressed, value {triggerValue}");
 
        //get & print the grip value
        float gripValue;
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out gripValue);
        //if (gripValue >= 0.1f) Debug.Log($"Grip Pressed, value {gripValue}");

        Vector2 leftStickValue;
        leftController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStickValue);
        //if(leftStickValue != Vector2.zero)
        //{
        //    Debug.Log($"Left Stick Value:, value {leftStickValue} ");
        //}
    }
}
