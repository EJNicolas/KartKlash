using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    [SerializeField] Transform head;
    public float damage;
    public Image healthBar;
    CarController car;
    bool takingDamage = false, healing = false;
    public float damageTime, healTime;

    public void Start()
    {
        if (health == 0) health = 100;
        car = GetComponent<CarController>();
    }

    public void FixedUpdate()
    {
        if(!takingDamage && health <= 0)
        {
            LeanTween.value(health, maxHealth, healTime)
                .setOnStart(() => { 
                    healing = true;
                    car.canDrive = false;
                })
                .setOnUpdate((float newHP) => { 
                    health = newHP;
                    healthBar.fillAmount = health / maxHealth;
                })
                .setOnComplete(() => { 
                    healing = false;
                    car.canDrive = true;
                });
        }
    }

    public override void TakeDamage(float damage)
    {
        if(!healing) base.TakeDamage(damage);
        LeanTween.value(healthBar.fillAmount, health / maxHealth, damageTime)
            .setOnStart(() => { takingDamage = true; })
            .setOnUpdate((float percentage) => { healthBar.fillAmount = percentage; })
            .setOnComplete(() => { takingDamage = false; });
        //Debug.Log(string.Format("Player health: {0}",health));
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }
}
