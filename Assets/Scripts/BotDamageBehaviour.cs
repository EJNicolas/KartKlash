using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDamageBehaviour : MonoBehaviour
{
    BotController bc;

    [Header("Damage")]
    public BotDamagePreset bdp;
    float shotInterval;
    float outlineHoverWidth, outlineShootWidth;
    Color shotColor, hoverColor;

    public float shotTimer = 0;
    public bool shot;

    public Outline outline;
    public bool hovering;

    [Header("Particles")]
    public ParticleSystem shotParticle;
    [SerializeField] GameObject player;
    public Vector3 hitLocation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        bc = GetComponent<BotController>();
        outline = GetComponent<Outline>();

        //carry over scriptableobject values
        shotInterval = bdp.shotInterval;
        outlineHoverWidth = bdp.outlineHoverWidth;
        outlineShootWidth = bdp.outlineShootWidth;
        shotColor = bdp.shotColor; 
        hoverColor = bdp.hoverColor;

        shotTimer = shotInterval;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateOutline();
    }

    void UpdateOutline()
    {
        shot = shotTimer < shotInterval;
        if (shot)
        {
            outline.OutlineColor = Color.Lerp(shotColor, hoverColor, shotTimer / shotInterval);
            outline.OutlineWidth = Mathf.Lerp(outlineShootWidth, outlineHoverWidth, shotTimer / shotInterval);
            shotTimer += Time.deltaTime;
        }
        else if (hovering)
        {
            outline.OutlineWidth = outlineHoverWidth;
            outline.OutlineColor = hoverColor;
        }
        else outline.OutlineWidth = 0;
    }

    public void OnDamage()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        if (!shot) EmitShotParticle();
        ShootHighlight();
        bc.TakeDamage(player.GetComponentInChildren<Player>().damage);
    }

    void ShootHighlight()
    {
        if (!shot) shotTimer = 0;
    }

    void EmitShotParticle()
    {
        if(!player) player = GameObject.FindGameObjectWithTag("Player");
        Vector3 targetDir = this.gameObject.transform.position - player.transform.position;
        ParticleSystem ps = Instantiate(shotParticle, hitLocation, Quaternion.LookRotation(targetDir), this.transform);
        ps.Play();
    }
}
