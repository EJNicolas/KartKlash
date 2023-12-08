using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDamageBehaviour : DamageBehaviour
{
    BotController bc;
    public override void Start()
    {
        base.Start();
        bc = GetComponentInChildren<BotController>();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnDamage()
    {
        base.OnDamage();
        bc.TakeDamage(player.GetComponentInChildren<Player>().damage);
    }
}
