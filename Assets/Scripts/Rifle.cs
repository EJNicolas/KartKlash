using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class Rifle : Weapon
{
    [SerializeField] private float fireRate;

    private WaitForSeconds wait;

    [Header("Raycast")]
    public XRRayInteractor rayInteractor;
    public BotDamageBehaviour bdb;
    public BotDamagePreset bdp;
    public bool showLine;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        fireRate = bdp.shotInterval;
        wait = new WaitForSeconds(1 / fireRate);
    }

    protected override void StartShooting(XRBaseInteractor interactor)
    {
        base.StartShooting(interactor);
        StartCoroutine(ShootingCO());
    }

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
                bdb.hitLocation = res.point;
            }
        }
        else if (bdb != null)
        {
            bdb.hovering = false;
            bdb = null;
        }
    }

    private IEnumerator ShootingCO()
    {
        while (true)
        {
            Shoot();
            yield return wait;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        if (bdb != null) bdb.OnDamage();
    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);
        StopAllCoroutines();
    }
}
