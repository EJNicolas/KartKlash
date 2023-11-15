using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : Entity
{
    [Header("Navigation")]
    public GameObject[] listPos;

    //public GameObject botObj;
    public NavMeshAgent nma;
    public float rdThreshold;
    int currentPoint;

    void Start()
    {
        currentPoint = listPos.Length;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateNavigation();
    }

    void UpdateNavigation()
    {
        //navigation
        nma.enabled = true;
        if (nma.hasPath && nma.remainingDistance < nma.stoppingDistance + rdThreshold && currentPoint < listPos.Length)
            currentPoint++;
        else if (currentPoint >= listPos.Length) currentPoint = 0;
        if (currentPoint < listPos.Length)
            nma.SetDestination(new Vector3(listPos[currentPoint].transform.position.x,
                this.gameObject.transform.position.y,
                listPos[currentPoint].transform.position.z));
    }

    void Shoot()
    {

    }

    public void TakeDamage()
    {

    }
}
