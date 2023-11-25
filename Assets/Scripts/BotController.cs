using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : Entity
{
    [Header("Navigation")]
    public GameObject[] listPos;

    public NavMeshAgent nma;
    public RaceManager rm;
    public float rdThreshold;
    public int currentPoint, checkpointsPassed;
    public int rubberbandThreshold;
    enum RubberbandState
    {
        SLOW,
        NORMAL,
        FAST
    }
    RubberbandState rs = RubberbandState.NORMAL;
    bool stateChanged = false;

    //navmeshagent fields
    private float defaultSpeed, defaultAngular, defaultAccel;
    public float speedRubberbandAmount, angularRubberbandFactor, accelRubberbandAmount;
    public bool raceStarted = false;

    //public GameObject trackedObj; //player

    void Start()
    {
        RaceManager.StartRaceEvent += ()=>{ raceStarted = true; };

        currentPoint = listPos.Length;

        //nma fields
        defaultSpeed = nma.speed;
        defaultAngular = nma.angularSpeed;
        defaultAccel = nma.acceleration;

        if (speedRubberbandAmount == 0) speedRubberbandAmount = 10;
        if (angularRubberbandFactor == 0) angularRubberbandFactor = 2;
        if (accelRubberbandAmount == 0) accelRubberbandAmount = 2;

        rm = FindAnyObjectByType<RaceManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (raceStarted) UpdateNavigation();
    }

    void UpdateNavigation()
    {
        //navigation
        nma.enabled = true;
        if (nma.hasPath && nma.remainingDistance < nma.stoppingDistance + rdThreshold && currentPoint < listPos.Length)
        {
            currentPoint++;
            checkpointsPassed++;
            //Debug.Log(this.gameObject.name + ", Checkpoints: " + checkpointsPassed);
        }
        else if (currentPoint >= listPos.Length) currentPoint = 0;
        if (currentPoint < listPos.Length)
            nma.SetDestination(new Vector3(listPos[currentPoint].transform.position.x,
                this.gameObject.transform.position.y,
                listPos[currentPoint].transform.position.z));

        if (Mathf.Abs(checkpointsPassed - rm.checkpointsPassed) > rubberbandThreshold || 
            rs == RubberbandState.SLOW && Mathf.Abs(checkpointsPassed - rm.checkpointsPassed) < rubberbandThreshold ||
            rs == RubberbandState.FAST && Mathf.Abs(checkpointsPassed - rm.checkpointsPassed) < rubberbandThreshold) 
            stateChanged = false;

        if(!stateChanged) NavRubberband();
    }

    void NavRubberband()
    {
        if (checkpointsPassed - rm.checkpointsPassed < -rubberbandThreshold) rs = RubberbandState.FAST;
        else if (checkpointsPassed - rm.checkpointsPassed > rubberbandThreshold) rs = RubberbandState.SLOW;
        else rs = RubberbandState.NORMAL;

        //Debug.Log(this.gameObject.name + ", rb value:" + (checkpointsPassed - rm.checkpointsPassed));

        stateChanged = true;

        switch (rs)
        {
            case RubberbandState.FAST:
                //Debug.Log(this.gameObject.name + ", Speeding Up");
                nma.speed = defaultSpeed + (speedRubberbandAmount * Mathf.Abs(checkpointsPassed - rm.checkpointsPassed));
                nma.angularSpeed = defaultAngular * angularRubberbandFactor;
                nma.acceleration = defaultAccel + (accelRubberbandAmount * Mathf.Abs(checkpointsPassed - rm.checkpointsPassed));
                return;

            case RubberbandState.NORMAL:
                //Debug.Log(this.gameObject.name + ", Returning to Normal");
                nma.speed = defaultSpeed;
                nma.angularSpeed = defaultAngular;
                nma.acceleration = defaultAccel;
                return;

            case RubberbandState.SLOW:
                //Debug.Log(this.gameObject.name + ", Slowing Down");
                nma.speed = defaultSpeed - (speedRubberbandAmount * Mathf.Abs(checkpointsPassed - rm.checkpointsPassed));
                return;

            default:
                nma.speed = defaultSpeed;
                nma.angularSpeed = defaultAngular;
                nma.acceleration = defaultAccel;
                return;
        }
    }

    void Shoot()
    {

    }

    public void TakeDamage()
    {

    }
}
