using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

        Vector3 viewAngleStart = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleEnd = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.color = Color.blue;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleStart * fov.viewRadius); //start
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleEnd * fov.viewRadius); //end

        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + fov.transform.forward * fov.viewRadius); //center

        Handles.color = Color.cyan;
        if(fov.visibleTarget != null) 
            Handles.DrawLine(fov.transform.position, fov.visibleTarget.transform.position);
    }
}
