using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : Entity
{
    [Header("Navigation")]
    public BotDrivingPreset botDrivePreset;
    public GameObject[] listPos;

    public AudioSource audio;
    public AudioClip cpuGunDamage;
    public float defaultVolume;

    NavMeshAgent nma;
    public RaceManager rm;
    public int currentPoint;   //checkpoint index of the checkpoint that will be crossed
    private int lapCount = -1;
    private int checkpointsPassed = 0;
    GameObject lastCheckpoint;
    int rubberbandThreshold;
    public enum RubberbandState
    {
        SLOW,
        NORMAL,
        FAST
    }
    public RubberbandState rs = RubberbandState.NORMAL;
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
    public BotShootPreset botShootPreset;

    public float rotationSpeed;
    public float recoveryTime;

    public bool canShoot = false;
    bool rotationOffset = false;

    public ParticleSystem fireParticle;
    public Transform gunTip;

    public bool raceStarted = false;

    public float fireRate;
    float shootInterval;
    float shootTimer = 0f;

    public float botDamage;
    [Range(0f, 100f)] public float hitChance;

    public ParticleSystem botShotParticles;
    public float botHitChanceMultiplier;
    public float botTargetHitChanceMultiplier;

    [Header("Tutorial")]
    public bool tutorialMode = false;
    public bool shooting;
    public bool stationary;

    void Start()
    {
        currentPoint = listPos.Length;
        audio = GetComponent<AudioSource>();
        nma = GetComponent<NavMeshAgent>();

        //randomize ranges from preset
        if (nma && !stationary)
        {
            nma.isStopped = true;
            nma.speed = Random.Range(botDrivePreset.minDefaultSpeed, botDrivePreset.maxDefaultSpeed);
            nma.angularSpeed = Random.Range(botDrivePreset.minDefaultAngular, botDrivePreset.maxDefaultAngular);
            nma.acceleration = Random.Range(botDrivePreset.minDefaultAccel, botDrivePreset.maxDefaultAccel);
            nma.stoppingDistance = Random.Range(botDrivePreset.minStoppingDistance, botDrivePreset.maxStoppingDistance);
        }

        //nma fields
        defaultSpeed = nma.speed;
        defaultAngular = nma.angularSpeed;
        defaultAccel = nma.acceleration;

        rubberbandThreshold = botDrivePreset.rubberbandThreshold;

        speedRubberbandAmount = botDrivePreset.speedRubberbandAmount;
        angularRubberbandFactor = botDrivePreset.speedRubberbandAmount;
        accelRubberbandAmount = botDrivePreset.accelRubberbandAmount;

        if (speedRubberbandAmount == 0) speedRubberbandAmount = 10;
        if (angularRubberbandFactor == 0) angularRubberbandFactor = 2;
        if (accelRubberbandAmount == 0) accelRubberbandAmount = 2;

        rm = FindAnyObjectByType<RaceManager>();

        //damage
        if (resumeDelay < 0.1f) resumeDelay = 0.1f;

        //ai shooting
        fov = GetComponent<FieldOfView>();

        if (gunShoulder) gunShoulderDefaultRotation = gunShoulder.localRotation;
        if (gunArm) gunArmDefaultRotation = gunArm.localRotation;
        if (neck) neckDefaultRotation = neck.localRotation;

        fireRate = Random.Range(botShootPreset.minFireRate, botShootPreset.maxFireRate);
        shootInterval = 1 / fireRate;

        botDamage = Random.Range(botShootPreset.minDamage, botShootPreset.maxDamage);
        hitChance = Random.Range(botShootPreset.minHitChance, botShootPreset.maxHitChance);

        botHitChanceMultiplier = Random.Range(botShootPreset.minBotHitChanceMult, botShootPreset.maxBotHitChanceMult);
        botTargetHitChanceMultiplier = Random.Range(botShootPreset.minTargetHitChanceMult, botShootPreset.maxTargetHitChanceMult);
        defaultVolume = audio.volume;
    }

    private void OnEnable(){
        RaceManager.StartRaceEvent += StartMoving;
        RaceManager.BeginTutorialEvent += SetTutorial;
    }

    private void OnDisable(){
        RaceManager.StartRaceEvent -= StartMoving;
        RaceManager.BeginTutorialEvent -= SetTutorial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Checkpoint") && lastCheckpoint != other.gameObject)
        {
            if (checkpointsPassed % listPos.Length == 0) lapCount++;
            checkpointsPassed++;
            lastCheckpoint = other.gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!stationary) UpdateNavigation();
        if (raceStarted && shooting)
        {
            UpdateTargeting();

            if (shootTimer < shootInterval) canShoot = false;

            if (canShoot) Shoot();
            else shootTimer += Time.deltaTime;
        }
    }

    void UpdateNavigation()
    {
        //navigation
        if (nma.hasPath && nma.remainingDistance < nma.stoppingDistance && currentPoint < listPos.Length)
        {
            currentPoint++;
            //checkpointsPassed++;
            //Debug.Log(this.gameObject.name + ", Checkpoints: " + checkpointsPassed);
        }
        else if (currentPoint >= listPos.Length)
        {
            currentPoint = 0;
        }
        if (currentPoint < listPos.Length)
            nma.SetDestination(new Vector3(listPos[currentPoint].transform.position.x,
                this.gameObject.transform.position.y,
                listPos[currentPoint].transform.position.z));

        if (Mathf.Abs(checkpointsPassed - rm.totalCheckpoints) > rubberbandThreshold || 
            rs == RubberbandState.SLOW && Mathf.Abs(checkpointsPassed - rm.totalCheckpoints) < rubberbandThreshold ||
            rs == RubberbandState.FAST && Mathf.Abs(checkpointsPassed - rm.totalCheckpoints) < rubberbandThreshold) 
            stateChanged = false;

        if(!stateChanged && !tutorialMode) NavRubberband();
    }

    void NavRubberband()
    {
        int checkpointDiff = checkpointsPassed - rm.totalCheckpoints;
        if (checkpointDiff < -rubberbandThreshold) rs = RubberbandState.FAST;
        else if (checkpointDiff > rubberbandThreshold) rs = RubberbandState.SLOW;
        else rs = RubberbandState.NORMAL;

        //Debug.Log(this.gameObject + " Ahead by checkpoints: " + (Mathf.Abs(checkpointDiff) - rubberbandThreshold));

        stateChanged = true;

        switch (rs)
        {
            case RubberbandState.FAST:
                //Debug.Log(this.gameObject.name + ", Speeding Up");
                nma.speed = defaultSpeed + (speedRubberbandAmount * (Mathf.Abs(checkpointDiff) - rubberbandThreshold));
                nma.angularSpeed = defaultAngular * angularRubberbandFactor;
                nma.acceleration = defaultAccel + (accelRubberbandAmount * (Mathf.Abs(checkpointDiff) - rubberbandThreshold));
                return;

            case RubberbandState.NORMAL:
                //Debug.Log(this.gameObject.name + ", Returning to Normal");
                nma.speed = defaultSpeed;
                nma.angularSpeed = defaultAngular;
                nma.acceleration = defaultAccel;
                return;

            case RubberbandState.SLOW:
                //Debug.Log(this.gameObject.name + ", Slowing Down");
                nma.speed = defaultSpeed - (speedRubberbandAmount * (Mathf.Abs(checkpointDiff) - rubberbandThreshold));
                return;

            default:
                nma.speed = defaultSpeed;
                nma.angularSpeed = defaultAngular;
                nma.acceleration = defaultAccel;
                return;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!stationary) nma.isStopped = true;
        if (raceStarted) StartCoroutine(ResumeAgentAfterDelay(resumeDelay));
    }

    IEnumerator ResumeAgentAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (!stationary) nma.isStopped = false;
    }

    //AI shooting
    void UpdateTargeting()
    {
        if (fov.visibleTarget != null)
        {
            rotationOffset = true;
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
            //Debug.Log(canShoot);
        }
        else if(rotationOffset)
        {
            rotationOffset = false;
            canShoot = false;
            //Debug.Log("rotating");
            LeanTween.rotateLocal(neck.gameObject, neckDefaultRotation.eulerAngles, recoveryTime);
            LeanTween.rotateLocal(gunArm.gameObject, gunArmDefaultRotation.eulerAngles, recoveryTime);
            LeanTween.rotateLocal(gunShoulder.gameObject, gunShoulderDefaultRotation.eulerAngles, recoveryTime);
        }
    }

    void Shoot()
    {
        //particle
        Vector3 targetDir = fov.visibleTarget.transform.position - transform.position;
        ParticleSystem ps = Instantiate(fireParticle, gunTip.position, Quaternion.LookRotation(targetDir), gunTip);

        ps.Play();

        if (fov.visibleTarget.layer == LayerMask.NameToLayer("Player")) audio.volume = defaultVolume;
        else audio.volume = defaultVolume / 4;

        audio.Play();

        if (fov.visibleTarget != null && hitChance > Random.Range(1, 100))
        {
            //if target is player
            if(fov.visibleTarget.layer == LayerMask.NameToLayer("Player"))
            {
                Player p = fov.visibleTarget.GetComponentInChildren<Player>();
                if (!p.healing)
                {
                    p.TakeDamage(botDamage, this.transform);
                    audio.PlayOneShot(cpuGunDamage, 1); //damage audio
                }
            }

            if (fov.visibleTarget.layer == LayerMask.NameToLayer("CPU") && botHitChanceMultiplier > Random.Range(0f, 1f))
            {
                fov.visibleTarget.GetComponentInParent<BotController>().TakeDamage(botDamage);

                RaycastHit res;
                if(Physics.Raycast(this.transform.position, targetDir, out res, fov.viewRadius, LayerMask.GetMask("CPU")))
                    Instantiate(botShotParticles, res.point, Quaternion.LookRotation(targetDir), this.transform);

                audio.PlayOneShot(cpuGunDamage, 0.5f); //damage audio
            }

            if (fov.visibleTarget.layer == LayerMask.NameToLayer("Shootable") && botTargetHitChanceMultiplier > Random.Range(0f, 1f))
            {
                fov.visibleTarget.GetComponentInParent<ShootableBehaviour>().EnemyDamage(this.transform);
                audio.PlayOneShot(cpuGunDamage, 0.5f); //damage audio
            }
        }
        shootTimer = 0;
    }

    void SetTutorial()
    {
        tutorialMode = true;
    }

    void StartMoving() {
        if (!stationary) nma.isStopped = false;
        raceStarted = true;
    }

    public int GetCurrentCheckpointIndex() {
        return currentPoint;
    }

    public int GetLapCount(){
        return lapCount;
    }

    public int GetCheckpointsPassed() {
        return checkpointsPassed;
    }
}
