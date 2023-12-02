using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceParticles : MonoBehaviour
{
    public List<Transform> particlePositions;
    public ParticleSystem lapPS, winPS;

    // Start is called before the first frame update
    private void OnEnable() {
        RaceManager.StartRaceEvent += FireLapParticles;
        RaceManager.CompleteLapEvent += FireLapParticles;
        RaceManager.CompleteRaceEvent += FireEndParticles;
    }

    private void OnDisable() {
        RaceManager.StartRaceEvent += FireLapParticles;
        RaceManager.CompleteLapEvent += FireLapParticles;
        RaceManager.CompleteRaceEvent += FireEndParticles;
    }

    void FireLapParticles()
    {
        foreach(Transform t in particlePositions)
            Instantiate(lapPS, t).Play();
    }

    void FireEndParticles()
    {
        foreach (Transform t in particlePositions)
            Instantiate(winPS, t).Play();
    }
}
