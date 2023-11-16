using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastOutlineVisual : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    BotDamageBehaviour bdb;
    public bool showLine;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        if (!showLine) GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit res;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            Vector3 groundPt = res.point; // the coordinate that the ray hits
            // Debug.Log(" coordinates on the ground: " + groundPt);
            if (res.transform.gameObject.layer == 6)
            {
                //Debug.Log("OBJECT");
                bdb = res.transform.gameObject.GetComponent<BotDamageBehaviour>();
                bdb.hovering = true;
            }
        }
        else if (bdb != null)
        {
            bdb.hovering = false;
            bdb = null;
        }
    }

    public void ShootOutline()
    {
        Debug.Log("pew");
        if (bdb != null) bdb.OnDamage();
    }
}
