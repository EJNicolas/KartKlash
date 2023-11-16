using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class URPRaycastVisual : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;
    public XRRayInteractor rayInteractor;
    BotDamageBehaviour bdb;

    public Color validColor;
    public Color invalidColor;
    public bool showLine;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        //lineMaterial = GetComponent<LineRenderer>().material;
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
                if(lineMaterial != null) lineMaterial.SetColor("_RayColor", validColor);
                bdb = res.transform.gameObject.GetComponent<BotDamageBehaviour>();
                bdb.hovering = true;
            }
        }
        else
        {
            if (lineMaterial != null) lineMaterial.SetColor("_RayColor", invalidColor);
            if(bdb != null)
            {
                bdb.hovering = false;
                bdb = null;
            }
        }
    }

    public void ShootOutline()
    {
        Debug.Log("pew");
        if (bdb != null) bdb.OnDamage();
    }
}
