using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBehaviour : DamageBehaviour
{
    CarController cc;

    public override void Start()
    {
        base.Start();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnDamage()
    {
        base.OnDamage();
        player.GetComponentInChildren<Player>().health += 5f;
        cc = player.GetComponentInChildren<CarController>();
        cc.ShootableShot();
    }
}
