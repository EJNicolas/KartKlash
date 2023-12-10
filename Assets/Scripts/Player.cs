using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] Transform head;
    public float damage;
    public Image healthBar;
    CarController car;
    public bool takingDamage = false, healing = false;
    public float damageTime, healTime;

    [Header("Hit HUD")]
    [SerializeField] CanvasGroup leftHUD;
    [SerializeField] CanvasGroup rightHUD;
    [SerializeField] CanvasGroup frontHUD;
    [SerializeField] CanvasGroup backHUD;

    LTDescr leftLT = null, rightLT = null, frontLT = null, backLT = null;
    public Camera playerCam;

    [Header("No Health Post-Processing")]
    [SerializeField] UnityEngine.Rendering.Volume postVol;

    public void Start()
    {
        if (health == 0) health = 100;
        car = GetComponent<CarController>();

        leftLT = LeanTween.alphaCanvas(leftHUD, 0f, 0f);
        rightLT = LeanTween.alphaCanvas(rightHUD, 0f, 0f);
        frontLT = LeanTween.alphaCanvas(frontHUD, 0f, 0f);
        backLT = LeanTween.alphaCanvas(backHUD, 0f, 0f);
    }

    public void FixedUpdate()
    {
        if (!takingDamage && health <= 0)
        {
            LeanTween.value(health, maxHealth, healTime)
                .setOnStart(() =>
                {
                    healing = true;
                    car.canDrive = false;

                    leftLT = LeanTween.alphaCanvas(leftHUD, 0f, 0.25f);
                    rightLT = LeanTween.alphaCanvas(rightHUD, 0f, 0.25f);
                    frontLT = LeanTween.alphaCanvas(frontHUD, 0f, 0.25f);
                    backLT = LeanTween.alphaCanvas(backHUD, 0f, 0.25f);

                    if (postVol) LeanTween.value(0f, 1f, 0.5f).setOnUpdate((float val) => { postVol.weight = val; });

                })
                .setOnUpdate((float newHP) =>
                {
                    health = newHP;
                    healthBar.fillAmount = health / maxHealth;
                })
                .setOnComplete(() =>
                {
                    healing = false;
                    car.canDrive = true;
                    if (postVol) LeanTween.value(1f, 0f, 0.5f).setOnUpdate((float val) => { postVol.weight = val; });
                });
        }
    }

    public override void TakeDamage(float damage, Transform source)
    {
        //damage
        if(!healing) base.TakeDamage(damage, source);
        LeanTween.value(healthBar.fillAmount, health / maxHealth, damageTime)
            .setOnStart(() => { takingDamage = true; })
            .setOnUpdate((float percentage) => { healthBar.fillAmount = percentage; })
            .setOnComplete(() => { takingDamage = false; });

        //directional HUD response
        Vector3 targetDir = (source.position - transform.position).normalized;
        float targetAngle = Vector3.SignedAngle(playerCam.transform.forward, targetDir, Vector3.up);

        switch (targetAngle)
        {
            case float a when (targetAngle < 45f && targetAngle > -45f): //forward
                ShotHUD(ref frontLT, ref frontHUD);
                return;
            case float a when (targetAngle < -45f && targetAngle > -135f): //left
                ShotHUD(ref leftLT, ref leftHUD);
                return;
            case float a when (targetAngle < -135f || targetAngle > 135f): //back
                ShotHUD(ref backLT, ref backHUD);
                return;
            case float a when (targetAngle < 135f && targetAngle > 45f): //right
                ShotHUD(ref rightLT, ref rightHUD);
                return;
            default:
                return;
        }
    }

    public void Heal(float healAmount)
    {
        if(health < maxHealth)
        {
            health += healAmount;
            LeanTween.value(healthBar.fillAmount, health / maxHealth, damageTime)
                .setOnUpdate((float percentage) => { healthBar.fillAmount = percentage; });
        }
    }

    void ShotHUD(ref LTDescr lt, ref CanvasGroup hud)
    {
        LeanTween.cancel(lt.id);
        hud.alpha = 1f;
        lt = LeanTween.alphaCanvas(hud, 0f, 0.5f);
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }
}
