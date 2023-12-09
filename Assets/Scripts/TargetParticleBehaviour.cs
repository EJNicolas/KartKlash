using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetParticleBehaviour : MonoBehaviour
{
    public Transform target;
    public float yOffset;
    public float speed = 5f;
    public float delayTime = 0.25f;

    private ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target) StartCoroutine(MoveParticlesTowardsPlayer(delayTime));
    }

    IEnumerator MoveParticlesTowardsPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int particleCount = particleSystem.GetParticles(particles);

        Vector3 playerPosition = target.position;

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 directionToPlayer = playerPosition - particles[i].position;
            directionToPlayer = new Vector3(directionToPlayer.x, directionToPlayer.y + yOffset, directionToPlayer.z); 
                //+ player.transform.forward * player.GetComponent<Rigidbody>().velocity.magnitude;
            particles[i].velocity += directionToPlayer.normalized * speed;
        }

        particleSystem.SetParticles(particles, particleCount);
    }
}
