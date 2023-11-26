using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHand : MonoBehaviour
{
    public bool rightHand;

    [Header("Model References")]
    public MeshFilter handMeshFilter;
    public Mesh openHand;
    public Mesh closedHand;
    public Mesh pointingHand;

    public void SetOpenHand() {
        if(openHand != null) handMeshFilter.mesh = openHand; 
    }

    public void SetClosedHand() {
        if(closedHand != null) handMeshFilter.mesh = closedHand;
    }

    public void SetPointingHand() {
        if(pointingHand != null) handMeshFilter.mesh = pointingHand;
    }
}
