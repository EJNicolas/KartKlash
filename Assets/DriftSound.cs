using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftSound : MonoBehaviour
{

    public AudioSource audio;
    public AudioClip driftSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        audio.PlayOneShot(driftSound, 1); //damage audio
    }
}
