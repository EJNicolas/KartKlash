using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    [SerializeField] protected float shootingForce;
    [SerializeField] protected Transform bulletSpawn;
    [SerializeField] private float recoilForce;
    [SerializeField] private float damage;

    public AudioSource audio;
    public AudioClip gunDamage;
    public AudioClip gunShoot;

    public ParticleSystem shootParticle;
    public Transform gunTip;

    private Rigidbody rigidBody;
    private XRGrabInteractable interactableWeapon;

    [Header("Raycast")]
    public XRRayInteractor rayInteractor;
    BotDamageBehaviour bdb;
    public WeaponOutline weaponOutline;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        rayInteractor = GetComponent<XRRayInteractor>();
        audio = GetComponent<AudioSource>();
        SetupInteractableWeaponEvents();
    }

    private void SetupInteractableWeaponEvents()
    {
        interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExited.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
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
                //audioSource.PlayOneShot(gunDamage,1 );
            }
        }
        else if (bdb != null)
        {
            bdb.hovering = false;
            bdb = null;
        }
    }

    private void PickUpWeapon(XRBaseInteractor interactor)
    {
        //interactor.GetComponent<MeshHidder>().Hide();
        rayInteractor.enabled = true;
        GetComponent<XRInteractorLineVisual>().enabled = true;

        weaponOutline = interactor.GetComponent<WeaponOutline>();
        weaponOutline.lineActive = false;
    }
 
    private void DropWeapon(XRBaseInteractor interactor)
    {
        //interactor.GetComponent<MeshHidder>().Show();
        rayInteractor.enabled = false;
        GetComponent<XRInteractorLineVisual>().enabled = false;

        weaponOutline.lineActive = true;
        weaponOutline = null;
    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void Shoot()
    {
        audio.Play();

        ParticleSystem ps = Instantiate(shootParticle, gunTip.position, Quaternion.LookRotation(transform.forward));
        ps.Play();

        if (bdb != null) {
            bdb.OnDamage();
            audio.PlayOneShot(gunDamage, 1);
        }
        ApplyRecoil();
        
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    public float GetDamage()
    {
        return damage;
    }
}
