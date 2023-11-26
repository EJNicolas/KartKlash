using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRHand : MonoBehaviour
{
    public bool drivingHand;

    [Header("Model References")]
    public MeshFilter handMeshFilter;
    public Mesh openHand;
    public Mesh closedHand;
    public Mesh pointingHand;

    [Header("Driving Hand UI")]
    public GameObject uiElementsParent;
    public TextMeshProUGUI drivingText;

    private void Start() {
        if (drivingHand) uiElementsParent.SetActive(true);
        else uiElementsParent.SetActive(false);
    }

    public void SetOpenHand() {
        if(openHand != null) handMeshFilter.mesh = openHand; 
    }

    public void SetClosedHand() {
        if(closedHand != null) handMeshFilter.mesh = closedHand;
    }

    public void SetPointingHand() {
        if(pointingHand != null) handMeshFilter.mesh = pointingHand;
    }

    public void SetDrivingText(string message) {
        drivingText.SetText(message);
    }
}
