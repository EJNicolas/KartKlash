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
    public int currentPoint;
    public int checkpointsPassed;
    public int rubberbandThreshold;
    enum RubberbandState
    {
        SLOW,
        NORMAL,
        FAST
    }
    RubberbandState rs = RubberbandState.NORMAL;
    bool stateChanged = false;

    [Header("Rubberband")] //navmeshagent fields
    public float speedRubberbandAmount;
    public float angularRubberbandFactor;
    public float accelRubberbandAmount;
    private float defaultSpeed, defaultAngular, defaultAccel;

    [Header("Damage")]
    [Range(0.1f, 1f)] public float resumeDelay;

    [Header("Targeting")]
    public Transform gunShoulder;
    public Transform gunArm;
    public Transform neck;

    private Quaternion gunShoulderDefaultRotation, gunArmDefaultRotation, neckDefaultRotation;
    FieldOfView fov;

    [Header("Shooting")]
    public float rotationSpeed;
    public float recoveryTime;

    public bool canShoot = false;

    public float shootInterval;
    float shootTimer = 0f;

    public ParticleSystem fireParticle;
    public Transform gunTip;

    //public GameObject trackedObj; //player

    void Start()
    {
        nma.isStopped = true;
        RaceManager.StartRaceEvent += () =>{ nma.isStopped = false; };

        currentPoint = listPos.Length;

        //nma fields
        defaultSpeed = nma.speed;
        defaultAngular = nma.angularSpeed;
        defaultAccel = nma.acceleration;

        if (speedRubberbandAmount == 0) speedRubberbandAmount = 10;
        if (angularRubberbandFactor == 0) angularRubberbandFactor = 2;
        if (accelRubberbandAmount == 0) accelRubberbandAmount = 2;

        rm = FindAnyObjectByType<RaceManager>();

        //damage
        if (resumeDelay < 0.1f) resumeDelay = 0.1f;

        //ai shooting
        fov = GetComponent<FieldOfView>();

        if (gunShoulder)
            gunShoulderDefaultRotation = gunShoulder.localRotation;

        if (gunArm)
            gunArmDefaultRotation = gunArm.localRotation;

        if (neck)
            neckDefaultRotation = neck.localRotation;

        //Debug.Log(neckDefaultPosition);
        //Debug.Log(gunShoulderDefaultPosition);
        //Debug.Log(gunArmDefaultPosition);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateNavigation();
        UpdateTargeting();

        if (shootTimer < shootInterval) canShoot = false;

        if (canShoot) Shoot();
        else shootTimer += Time.deltaTime;
    }

    void UpdateNavigation()
    {
        //navigation
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

    public void TakeDamage()
    {
        nma.isStopped = true;
        StartCoroutine(ResumeAgentAfterDelay(resumeDelay));
    }

    IEnumerator ResumeAgentAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        nma.isStopped = false;
    }

    //AI shooting
    void UpdateTargeting()
    {
        if (fov.visibleTarget != null)
        {
            Vector3 targetDir = fov.visibleTarget.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);

            neck.rotation = Quaternion.Slerp(neck.rotation, targetRot, rotationSpeed * Time.deltaTime);


            if (Vector3.Dot(transform.right, targetDir) < 0f) //only animate shoulder if target is on gun side
            {
                Quaternion gsTargetRot = Quaternion.Euler(targetRot.eulerAngles + new Vector3(0f, 90f - gunShoulderDefaultRotation.eulerAngles.y, 90f));
                gunShoulder.rotation = Quaternion.Slerp(gunShoulder.rotation, gsTargetRot, rotationSpeed * Time.deltaTime);
            }

            Quaternion gaTargetRot = Quaternion.Euler(targetRot.eulerAngles + new Vector3(0f, 90f, 90f));
            gunArm.rotation = Quaternion.Slerp(gunArm.rotation, gaTargetRot, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(gunArm.rotation, gaTargetRot) < 1.0f) canShoot = true;
            Debug.Log(canShoot);
        }
        else
        {
            canShoot = false;
            LeanTween.rotateLocal(neck.gameObject, neckDefaultRotation.eulerAngles, recoveryTime);
            LeanTween.rotateLocal(gunArm.gameObject, gunArmDefaultRotation.eulerAngles, recoveryTime);
            LeanTween.rotateLocal(gunShoulder.gameObject, gunShoulderDefaultRotation.eulerAngles, recoveryTime);
        }
    }

    void Shoot()
    {
        Vector3 targetDir = fov.visibleTarget.transform.position - transform.position;
        ParticleSystem ps = Instantiate(fireParticle, gunTip.position, Quaternion.LookRotation(targetDir));
        ps.Play();
        shootTimer = 0;
    }
}
