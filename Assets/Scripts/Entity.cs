using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        if (health == 0) health = 100;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void TakeDamage(float amount)
    {
        health -= amount;
    }
    
    void Shoot()
    {
        //implement generic shooting behaviour
    }
}
