using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
    SceneTransitionBehaviour stb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ping, " + other.gameObject.tag);
            other.gameObject.GetComponentInChildren<SceneTransitionBehaviour>().TransitionIn();
            //if (!stb) stb.TransitionIn();
        }
    }
}
