using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBehaviour : DamageBehaviour
{
    CarController cc;

    [Header("Dissolve")]
    public float minCutoff;
    public float maxCutoff;

    public float disableDuration;
    public float fadeDuration;

    Material dissolveMaterial;
    Collider col;

    Transform damageSource;

    public override void Start()
    {
        base.Start();
        dissolveMaterial = this.GetComponent<Renderer>().material;
        col = GetComponent<Collider>();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnDamage()
    {
        damageSource = player.transform;
        base.OnDamage();

        player.GetComponentInChildren<Player>().health += 5f;
        cc = player.GetComponentInChildren<CarController>();
        cc.ShootableShot();

        outline.OutlineWidth = 0f;
        col.enabled = false;

        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), minCutoff, fadeDuration).setOnUpdate(UpdateMaterial);
        StartCoroutine(RespawnAfterDelay(fadeDuration + disableDuration));
    }
    public override void EmitShotParticle()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        ParticleSystem ps = Instantiate(shotParticle, transform.position, transform.rotation, this.transform);
        ps.GetComponent<TargetParticleBehaviour>().target = damageSource;
        ps.Play();
    }

    public void EnemyDamage(Transform src)
    {
        damageSource = src;
        if (!shot)
        {
            shotTimer = 0;
            EmitShotParticle();
        }

        outline.OutlineWidth = 0f;
        col.enabled = false;

        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), minCutoff, fadeDuration).setOnUpdate(UpdateMaterial);
        StartCoroutine(RespawnAfterDelay(fadeDuration + disableDuration));
    }

    void UpdateMaterial(float cutoff)
    {
        dissolveMaterial.SetFloat("_CutoffHeight", cutoff);
    }

    IEnumerator RespawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        LeanTween.value(dissolveMaterial.GetFloat("_CutoffHeight"), maxCutoff, fadeDuration)
            .setOnUpdate(UpdateMaterial)
            .setOnComplete(() => { col.enabled = true; });
    }
}
