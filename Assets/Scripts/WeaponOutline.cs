using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WeaponOutline : MonoBehaviour
{
    XRRayInteractor rayInteractor;
    public bool lineActive;
    public GameObject gun;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lineActive) GetComponent<XRInteractorLineVisual>().enabled = true;
        else GetComponent<XRInteractorLineVisual>().enabled = false;

        RaycastHit res;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            Vector3 groundPt = res.point; // the coordinate that the ray hits
            // Debug.Log(" coordinates on the ground: " + groundPt);
            if (res.transform.gameObject.layer == 7)
            {
                //Debug.Log("OBJECT");
                gun = res.transform.gameObject;
                gun.GetComponent<Outline>().enabled = true;
            }
        }
        else if (gun != null)
        {
            gun.GetComponent<Outline>().enabled = false;
            gun = null;
        }
    }
}
