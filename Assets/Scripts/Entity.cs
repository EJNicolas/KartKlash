using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    public float health;
    public float maxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        if (health == 0) health = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public virtual void TakeDamage(float amount)
    {
        if (health > 0) health -= amount;
    }
}
