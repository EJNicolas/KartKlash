using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //tutorial for fov https://www.youtube.com/watch?v=rQG9aUWarwE
    public float viewRadius;
    [Range(0f, 360f)] public float viewAngle;

    public LayerMask targetMask;
    public GameObject visibleTarget = null;

    private void FixedUpdate()
    {
        FindVisibleTarget();
    }

    public void FindVisibleTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //assume only one player active
        if(targets.Length >= 1)
        {
            Vector3 dirTarget = (targets[0].transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirTarget) < viewAngle / 2)
                visibleTarget = targets[0].gameObject;
        }
    }

    public Vector3 DirFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
