using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class URPRaycastVisual : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;
    public XRRayInteractor rayInteractor;
    Outline o;

    Color validColor;
    Color invalidColor;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit res;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            Vector3 groundPt = res.point; // the coordinate that the ray hits
            // Debug.Log(" coordinates on the ground: " + groundPt);
            if (res.transform.gameObject.layer == 6)
            {
                Debug.Log("OBJECT");
                lineMaterial.SetColor("_RayColor", validColor);
                o = res.transform.gameObject.GetComponent<Outline>();
                o.OutlineWidth = 8;
            }
        }
        else
        {
            lineMaterial.SetColor("_RayColor", invalidColor);
            if(o != null)
            {
                o.OutlineWidth = 0;
                o = null;
            }
        }
    }
}
